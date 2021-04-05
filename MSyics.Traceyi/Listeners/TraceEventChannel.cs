using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MSyics.Traceyi.Listeners
{
    internal class TraceEventChannel
    {
        readonly CancellationTokenSource cts = new();
        readonly Channel<TraceEventArgs> channel;
        readonly Action<TraceEventArgs> consume;
        readonly Task[] consumers;

        public TraceEventChannel(Action<TraceEventArgs> consume, int concurrency = 1)
        {
            this.consume = consume;
            channel = Channel.CreateUnbounded<TraceEventArgs>(new UnboundedChannelOptions
            {
                AllowSynchronousContinuations = false,
                SingleReader = false,
                SingleWriter = false,
            });

            //consumers = CreateReader(channel.Reader, concurrency).Select(reader => ReadAsync(reader)).ToArray();
            consumers = Enumerable.Range(1, Math.Max(1, concurrency)).Select(i => ReadAsync(channel.Reader)).ToArray();
        }

        private ChannelReader<TraceEventArgs>[] CreateReader(ChannelReader<TraceEventArgs> reader, int concurrency)
        {
            var channels = Enumerable.Range(1, Math.Max(1, concurrency)).Select(i => Channel.CreateUnbounded<TraceEventArgs>()).ToArray();

            Task.Run(async () =>
            {
                await Task.Yield();
                var index = 0;
                while (await reader.WaitToReadAsync(cts.Token))
                {
                    while (reader.TryRead(out var item))
                    {
                        index = (index + 1) % concurrency;
                        await channels[index].Writer.WriteAsync(item);
                    }
                }

                foreach (var item in channels)
                {
                    item.Writer.TryComplete();
                }
            });

            return channels.Select(x => x.Reader).ToArray();
        }

        private async Task ReadAsync(ChannelReader<TraceEventArgs> reader)
        {
            await Task.Yield();
            try
            {
                while (await reader.WaitToReadAsync().ConfigureAwait(false))
                {
                    while (reader.TryRead(out var item))
                    {
                        if (cts.IsCancellationRequested) return;
                        consume?.Invoke(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                channel.Writer.TryComplete(ex);
            }
        }

        public ValueTask WriteAsync(TraceEventArgs e) => channel.Writer.WriteAsync(e);

        public async Task CloseAsync(TimeSpan timeout)
        {
            if (consumers.Length == 0) return;

            channel.Writer.TryComplete();
            cts.CancelAfter(timeout);
            try
            {
                //await Task.WhenAll(consumers);
                await Task.WhenAll(consumers.Select(reader => reader.ContinueWith(_ => { }, cts.Token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default)));
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
