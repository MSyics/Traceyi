﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
//#if NETCOREAPP
using System.Threading.Channels;
//#else
using System.Collections.Concurrent;
//#endif

namespace MSyics.Traceyi.Listeners
{
    /// <summary>
    /// トレースデータを記録します。これは抽象クラスです。
    /// </summary>
    public abstract class Logger : IDisposable, ITraceListener
    {
        #region Static Members
        protected static readonly object GlobalLock = new();
        #endregion

        private readonly CancellationTokenSource cts = new();

//#if NETCOREAPP
        private readonly Task consumer;
        private readonly Channel<TraceEventArgs> channel;

        public Logger()
        {
            channel = Channel.CreateUnbounded<TraceEventArgs>(new UnboundedChannelOptions { SingleReader = true });
            consumer = ReadAsync();
        }

        private async Task ReadAsync()
        {
            await Task.Yield();
            try
            {
                while (await channel.Reader.WaitToReadAsync().ConfigureAwait(false))
                {
                    while (channel.Reader.TryRead(out var item))
                    {
                        Write(item);
                    }
                }
            }
            catch (Exception e)
            {
                channel.Writer.TryComplete(e);
            }
        }
//#endif

        /// <summary>
        /// ロックを使用するかどうかを示す値を取得または設定します。
        /// </summary>
        public bool UseLock { get; set; } = false;

        /// <summary>
        /// 非同期 I/O または同期 I/O のどちらを使用するかを示す値を取得または設定します。
        /// </summary>
        public bool UseAsync { get; set; } = true;

        /// <summary>
        /// 終了を待機する時間間隔を取得または設定します。
        /// </summary>
        public TimeSpan CloseTimeout { get; set; } = TimeSpan.FromMilliseconds(-1);

        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        public string Name { get; protected internal set; }

        /// <summary>
        /// トレースイベントを処理します。
        /// </summary>
        public void OnTracing(object sender, TraceEventArgs e)
        {
            if (cts.IsCancellationRequested) { return; }
            if (UseAsync)
            {
                try
                {
                    _ = WriteAsync(e);
                }
                catch (TaskCanceledException ex)
                {
                    Debug.Print($"{ex}");
                }
                catch (Exception ex)
                {
                    Debug.Print($"{ex}");
                }
            }
            else
            {
                Write(e);
            }
        }

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        protected internal abstract void WriteCore(TraceEventArgs e);

        /// <summary>
        /// トレースイベント情報を書き込みます。
        /// </summary>
        private void Write(TraceEventArgs e)
        {
            if (UseLock)
            {
                lock (GlobalLock)
                {
                    try
                    {
                        WriteCore(e);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print($"{ex}");
                    }
                }
            }
            else
            {
                try
                {
                    WriteCore(e);
                }
                catch (Exception ex)
                {
                    Debug.Print($"{ex}");
                }
            }
        }

//#if NETCOREAPP
        /// <summary>
        /// トレースイベント情報を書き込みます。
        /// </summary>
        private ValueTask WriteAsync(TraceEventArgs e) => channel.Writer.WriteAsync(e);
//#else
//        private readonly ConcurrentQueue<TraceEventArgs> traceEvents = new();
//        private int dequeuing = 0;

//        /// <summary>
//        /// トレースイベント情報を書き込みます。
//        /// </summary>
//        private async Task WriteAsync(TraceEventArgs e)
//        {
//            if (cts.IsCancellationRequested) { return; }
//            await Task.Yield();
//            traceEvents.Enqueue(e);
//            if (Interlocked.CompareExchange(ref dequeuing, 1, 0) == 0)
//            {
//                await Task.Run(() =>
//                    {
//                        while (traceEvents.TryDequeue(out var item) && !cts.IsCancellationRequested)
//                        {
//                            Write(item);
//                        }
//                        Interlocked.Exchange(ref dequeuing, 0);
//                    });
//            }
//        }
//#endif

        /// <summary>
        /// リソースを破棄したかどうかを示す値を取得します。
        /// </summary>
        public bool Disposed { get; private set; } = false;

        /// <summary>
        /// 使用しているリソースを破棄します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed) { return; }
            Disposed = true;

//#if NETCOREAPP
            channel.Writer.TryComplete();
            if (consumer != null)
            {
                consumer.Wait(CloseTimeout);
                consumer.Dispose();
            }
//#else
//            Task.Run(() => { while (traceEvents.Count != 0) ; }).Wait(CloseTimeout);
//#endif
            cts.Cancel(false);
            cts.Dispose();

            if (disposing) { DisposeManagedResources(); }
            DisposeUnmanagedResources();
        }

        /// <summary>
        /// マネージリソースを破棄します。
        /// </summary>
        protected virtual void DisposeManagedResources() { }

        /// <summary>
        /// アンマネージリソースを破棄します。
        /// </summary>
        protected virtual void DisposeUnmanagedResources() { }
    }
}
