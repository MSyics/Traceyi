/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    internal sealed class AsyncLock
    {
        private readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private readonly Task<IDisposable> Releaser;

        public AsyncLock()
        {
            Releaser = Task.FromResult((IDisposable)new Releasable(this));
        }

        public Task<IDisposable> LockAsync()
        {
            var wait = Semaphore.WaitAsync();
            return wait.IsCompleted ?
                Releaser :
                wait.ContinueWith(
                    (_, state) => (IDisposable)state,
                    Releaser.Result, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        private class Releasable : IDisposable
        {
            private readonly AsyncLock Target;
            public Releasable(AsyncLock target) { Target = target; }
            public void Dispose() { Target.Semaphore.Release(); }
        }
    }
}
