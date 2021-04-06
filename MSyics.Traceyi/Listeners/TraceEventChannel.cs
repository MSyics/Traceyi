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
        readonly List<ChannelReader<(TraceEventArgs e, int index)>> readers = new();
        readonly CancellationTokenSource cts = new();
        readonly Channel<TraceEventArgs> channel = Channel.CreateUnbounded<TraceEventArgs>();
        readonly List<Task> consumers = new();
        readonly Action<TraceEventArgs, int> consume;

        public TraceEventChannel(Action<TraceEventArgs, int> consume, int divide = 1)
        {
            this.consume = consume;
            readers.AddRange(Split(channel.Reader, divide));
            consumers.AddRange(readers.Select(reader => ReadAsync(reader)));
        }

        public void Close(TimeSpan timeout)
        {
            channel.Writer.TryComplete();
            cts.CancelAfter(timeout);
            try
            {
                Task.WhenAll(consumers).Wait(timeout);
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            readers.Clear();
            consumers.Clear();
            cts.Dispose();
        }

        private ChannelReader<(TraceEventArgs, int)>[] Split(ChannelReader<TraceEventArgs> reader, int divide)
        {
            var channels = Enumerable.Range(1, Math.Max(1, divide)).Select(i => Channel.CreateUnbounded<(TraceEventArgs, int)>()).ToArray();

            Task.Run(async () =>
            {
                var index = 0;
                while (await reader.WaitToReadAsync(cts.Token).ConfigureAwait(false))
                {
                    while (reader.TryRead(out var item))
                    {
                        if (cts.IsCancellationRequested) return;
                        await channels[index].Writer.WriteAsync((item, index));
                        index = (index + 1) % divide;
                    }
                }

                foreach (var item in channels)
                {
                    item.Writer.TryComplete();
                }
            });

            return channels.Select(x => x.Reader).ToArray();
        }

        private async Task ReadAsync(ChannelReader<(TraceEventArgs, int)> reader)
        {
            try
            {
                while (await reader.WaitToReadAsync().ConfigureAwait(false))
                {
                    while (reader.TryRead(out var item))
                    {
                        if (cts.IsCancellationRequested) return;
                        consume?.Invoke(item.Item1, item.Item2);
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
    }
}
