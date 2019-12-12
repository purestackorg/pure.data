﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pure.Data
{
    /*
     public async Task ExceptionShouldFlow()
		{
			var _lock = new AsyncLock();

			try
			{
				using (await _lock)
				{
					throw new ExpectedException("Hello World");
				}
				throw new Exception("Should never hit this line of code");
			}
			catch (ExpectedException e)
			{
				Assert.Equal("Hello World", e.Message);
			}
		}
         */


    [DebuggerDisplay("HasLock = {HasLock}, Waiting = {WaitingCount}")]
    public class AsyncLock : IAwaitable<IDisposable>
    {
        private readonly ConcurrentQueue<WaiterBase> _waiters;
        private object _current;

        public AsyncLock() => _waiters = new ConcurrentQueue<WaiterBase>();

        public bool HasLock => _current != null;

        // only used in debug view
        private int WaitingCount => _waiters.Count;

        public IAwaiter<IDisposable> GetAwaiter()
        {
            WaiterBase waiter;
            if (TryTakeControl())
            {
                waiter = new NonBlockedWaiter(this);
                RunWaiter(waiter);
            }
            else
            {
                waiter = new AsyncLockWaiter(this);
                _waiters.Enqueue(waiter);
                TryNext();
            }
            return waiter;
        }

        public override string ToString() => "AsyncLock: " + (HasLock ? "Locked with " + WaitingCount + " queued waiters" : "Unlocked");

        internal void Done(WaiterBase waiter)
        {
            var oldWaiter = Interlocked.Exchange(ref _current, null);
            Debug.Assert(oldWaiter == waiter, "Invalid end state", string.Format("Expected current waiter to be {0} but was {1}", waiter, oldWaiter));
            TryNext();
        }

        private void ReleaseControl()
        {
            if (Interlocked.Exchange(ref _current, null) != Sentinel.Value)
            {
                Debug.Assert(false, "Invalid revert state", string.Format("Expected current waiter to be {0} but was {1}", Sentinel.Value, _current));
            }
        }

        private void RunWaiter(WaiterBase waiter)
        {
            Debug.Assert(_current == Sentinel.Value, "Invalid start state", string.Format("Expected current waiter to be {0} but was {1}", Sentinel.Value, _current));
            _current = waiter;
            waiter.Ready();
        }

        private void TryNext()
        {
            if (TryTakeControl())
            {
                WaiterBase waiter;
                if (_waiters.TryDequeue(out waiter))
                {
                    RunWaiter(waiter);
                }
                else
                {
                    ReleaseControl();
                }
            }
        }

        private bool TryTakeControl() => Interlocked.CompareExchange(ref _current, Sentinel.Value, null) == null;
    }
    internal sealed partial class AsyncLockWaiter
    {
        private struct State
        {
            public const int Waiting = 0;
            public const int Running = 1;
            public const int Done = 2;
        }

        partial void ChangeState(int expectedState, int newState, string unexpectedStateMessage);

#if DEBUG
        private int _state;

        partial void ChangeState(int expectedState, int newState, string unexpectedStateMessage)
        {
            var oldState = Interlocked.Exchange(ref _state, newState);
            Debug.Assert(oldState == expectedState, unexpectedStateMessage);
        }
#endif
    }
    internal sealed partial class AsyncLockWaiter : WaiterBase
    {
        private static readonly Action _marker = () => { };
        private Action _continuation;
        private ExecutionContext _executionContext;

        public AsyncLockWaiter(AsyncLock @lock) : base(@lock)
        { }

        // since this is the async waiter, we will never be complete here, and even if we would be, the code would still behave correct
        public override bool IsCompleted => false;

        public override void Dispose()
        {
            ChangeState(State.Running, State.Done, "Unexpected state: Should be Running");
            base.Dispose();
        }

        public override void Ready()
        {
            ChangeState(State.Waiting, State.Running, "Unexpected state: Should be Waiting");
            var continuation = Interlocked.Exchange(ref _continuation, _marker);
            ScheduleContinuation(_executionContext, continuation);
        }

        public override string ToString() => "AsyncWaiter: " + base.ToString();

        protected override void OnCompleted(Action continuation, bool captureExecutionContext)
        {
            if (captureExecutionContext)
            {
                _executionContext = ExecutionContext.Capture();
            }

            var placeholder = Interlocked.Exchange(ref _continuation, continuation);
            if (placeholder == _marker)
            {
                // Between start of this method and $here, the Ready() method have been called from another thread, we should schedule the
                // continuation directly
                ScheduleContinuation(_executionContext, continuation);
            }
        }

        private static void ContinuationCallback(object state)
        {
            var c = (ContextAndAction)state;
            if (c.Context != null)
            {
                ExecutionContext.Run(c.Context, x => ((Action)x)(), c.Continuation);
            }
            else
            {
                c.Continuation();
            }
        }

        private static void ScheduleContinuation(ExecutionContext executionContext, Action continuation)
        {
            if (continuation == null || continuation == _marker)
                return;

            var callbackState = new ContextAndAction(executionContext, continuation);
            ThreadPool.QueueUserWorkItem(ContinuationCallback, callbackState);
        }

        private class ContextAndAction
        {
            public ContextAndAction(ExecutionContext context, Action continuation)
            {
                Context = context;
                Continuation = continuation;
            }

            public ExecutionContext Context { get; }
            public Action Continuation { get; }
        }
    }
    public interface IAwaitable<out TResult>
    {
        IAwaiter<TResult> GetAwaiter();
    }

    public interface IAwaiter<out TResult> : ICriticalNotifyCompletion
    {
        bool IsCompleted { get; }

        TResult GetResult();
    }

    internal sealed class NonBlockedWaiter : WaiterBase
    {
        public NonBlockedWaiter(AsyncLock @lock)
            : base(@lock)
        {
        }

        public override bool IsCompleted => true;

        public override string ToString() => "NonBlockingWaiter: " + base.ToString();

        protected override void OnCompleted(Action continuation, bool captureExecutionContext) => continuation?.Invoke();
    }
    internal class Sentinel
    {
        public static readonly object Value = new Sentinel();

        public override string ToString() => GetType().Name;
    }

    internal abstract class WaiterBase : IAwaiter<IDisposable>, IDisposable
    {
        protected readonly AsyncLock _lock;

        protected WaiterBase(AsyncLock @lock) => _lock = @lock;

        public abstract bool IsCompleted { get; }

        public virtual void Dispose() => _lock.Done(this);

        public IDisposable GetResult() => this;

        public void OnCompleted(Action continuation) => OnCompleted(continuation, true);

        public virtual void Ready() { }

        public override string ToString() => GetHashCode().ToString("x8", CultureInfo.InvariantCulture);

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation, false);

        protected abstract void OnCompleted(Action continuation, bool captureExecutionContext);
    }
}
