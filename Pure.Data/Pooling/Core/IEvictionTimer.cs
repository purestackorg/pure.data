using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Pure.Data.Pooling 
{
    public class EvictionSettings
    {

        public static DateTime GetCurrentTime() {
            return DateTime.Now;
        }

        /// <summary>
        ///   Default eviction settings.
        /// </summary>
        public static EvictionSettings Default { get; } = new EvictionSettings();

        /// <summary>
        ///   The delay specified when an eviction job is scheduled. Default value is <see cref="TimeSpan.Zero"/>.
        /// </summary>
        public TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <summary>
        ///   Whether eviction is enabled or not. By default, eviction is not enabled.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        ///   How frequent should be the eviction job. Default value is one minute.
        /// </summary>
        public TimeSpan Period { get; set; } = TimeSpan.FromMinutes(1);
    }
    public interface IEvictionTimer : IDisposable
    {
        /// <summary>
        ///   Cancels a scheduled evicton action using a ticket returned by <see
        ///   cref="Schedule(Action, TimeSpan, TimeSpan)"/>.
        /// </summary>
        /// <param name="actionTicket">
        ///   An eviction action ticket, which has been returned by <see cref="Schedule(Action,
        ///   TimeSpan, TimeSpan)"/>.
        /// </param>
        void Cancel(Guid actionTicket);

        /// <summary>
        ///   Schedules an eviction action.
        /// </summary>
        /// <param name="action">Eviction action.</param>
        /// <param name="delay">Start delay.</param>
        /// <param name="period">Schedule period.</param>
        /// <returns>
        ///   A ticket which identifies the scheduled eviction action, it can be used to cancel the
        ///   scheduled action via <see cref="Cancel(Guid)"/> method.
        /// </returns>
        Guid Schedule(Action action, TimeSpan delay, TimeSpan period);
    }

    public sealed class EvictionTimer : IEvictionTimer, IDisposable
    {
         
        private readonly Dictionary<Guid, Timer> _actionMap = new Dictionary<Guid, Timer>();
        private volatile bool _disposed;

        /// <summary>
        ///   Finalizer for <see cref="EvictionTimer"/>.
        /// </summary>
        ~EvictionTimer()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Cancels a scheduled evicton action using a ticket returned by <see
        ///   cref="Schedule(Action, TimeSpan, TimeSpan)"/>.
        /// </summary>
        /// <param name="actionTicket">
        ///   An eviction action ticket, which has been returned by <see cref="Schedule(Action,
        ///   TimeSpan, TimeSpan)"/>.
        /// </param>
        public void Cancel(Guid actionTicket)
        {
            ThrowIfDisposed();
            lock (_actionMap)
            {
                if (_actionMap.TryGetValue(actionTicket, out var timer))
                {
                    _actionMap.Remove(actionTicket);
                    timer.Dispose();
                }
            }
        }

        /// <summary>
        ///   Disposes the eviction timer, making it unusable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Schedules an eviction action.
        /// </summary>
        /// <param name="action">Eviction action.</param>
        /// <param name="delay">Start delay.</param>
        /// <param name="period">Schedule period.</param>
        /// <returns>
        ///   A ticket which identifies the scheduled eviction action, it can be used to cancel the
        ///   scheduled action via <see cref="Cancel(Guid)"/> method.
        /// </returns>
        public Guid Schedule(Action action, TimeSpan delay, TimeSpan period)
        {
            ThrowIfDisposed();

            if (action == null)
            {
                return Guid.Empty;
            }

            lock (_actionMap)
            {
                void timerCallback(object _)
                {
                
                    action(); 
                }

                var actionTicket = Guid.NewGuid();
                _actionMap[actionTicket] = new Timer(_ => timerCallback(_), null, delay, period);
                return actionTicket;
            }
        }

        /// <summary>
        ///   Disposes all action timers.
        /// </summary>
        /// <param name="disposing">False if called by the finalizer, true otherwise.</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // Mark this object as completely disposed.
                _disposed = true;

                if (disposing && _actionMap != null)
                {
                    var timers = _actionMap.Values.ToArray() ?? Enumerable.Empty<Timer>();
                    _actionMap.Clear();
                    foreach (var t in timers)
                    {
                        t.Dispose();
                    }
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
