using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    internal sealed class AsyncLock
    {
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        readonly Task<IDisposable> releaser;

        public AsyncLock()
        {
            releaser = Task.FromResult((IDisposable)new Releasable(this));
        }

        public Task<IDisposable> LockAsync()
        {
            var wait = semaphore.WaitAsync();
            return wait.IsCompleted 
                ? releaser 
                : wait.ContinueWith(
                    (_, state) => (IDisposable)state,
                    releaser.Result, 
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, 
                    TaskScheduler.Default);
        }

        private class Releasable : IDisposable
        {
            readonly AsyncLock target;
            public Releasable(AsyncLock target) { this.target = target; }
            public void Dispose() { target.semaphore.Release(); }
        }
    }
}
