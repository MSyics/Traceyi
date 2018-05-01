/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Layout;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Traceyi.Listeners
{
    public class AsyncLogger<T> : ITraceListener
        where T : ITraceListener
    {
        public T Listener { get; set; }
        public static int Count = 0;

        public void Dispose()
        {
            while (Count != 0)
            {
            }
            Console.WriteLine(this);
        }

        public async void OnTracing(object sender, TraceEventArg e)
        {
            Interlocked.Increment(ref Count);
            using (await m_asyncLock.LockAsync())
            {
                await Task.Run(() =>
                {
                    Listener.OnTracing(sender, e);
                });
            }
            Interlocked.Decrement(ref Count);
        }

        private static AsyncLock m_asyncLock = new AsyncLock();
    }

    public sealed class AsyncLock
    {
        private readonly SemaphoreSlim m_semaphore = new SemaphoreSlim(1, 1);
        private readonly Task<IDisposable> m_releaser;

        public AsyncLock()
        {
            m_releaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        public Task<IDisposable> LockAsync()
        {
            var wait = m_semaphore.WaitAsync();
            return wait.IsCompleted ?
                        m_releaser :
                        wait.ContinueWith((_, state) => (IDisposable)state,
                            m_releaser.Result, CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        private sealed class Releaser : IDisposable
        {
            private readonly AsyncLock m_toRelease;
            internal Releaser(AsyncLock toRelease) { m_toRelease = toRelease; }
            public void Dispose() { m_toRelease.m_semaphore.Release(); }
        }
    }
}
