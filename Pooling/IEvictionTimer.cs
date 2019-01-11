using System;

namespace Pure.Data.Pooling
{
    public interface IEvictionTimer : IDisposable
    {
        void Schedule(Evictor task, TimeSpan delay, TimeSpan period);

        void Cancel(Evictor task);
    }
}
