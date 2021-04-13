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
        readonly Action<TraceEventArgs, int> action;
        readonly List<Task> tasks = new();
        readonly int demux;
        private CancellationTokenSource cts;
        private Channel<TraceEventArgs> channel;

        private async Task ReadAsync(ChannelReader<TraceEventArgs> reader, int index, CancellationToken token)
        {
            try
            {
                while (await reader.WaitToReadAsync(token))
                {
                    while (reader.TryRead(out var item))
                    {
                        if (token.IsCancellationRequested) return;
                        action?.Invoke(item, index);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public TraceEventChannel(Action<TraceEventArgs, int> action, int demux = 1)
        {
            this.action = action;
            this.demux = Math.Max(1, demux);
        }

        public bool Running { get; private set; }

        public void Open()
        {
            if (Running) return;
            Running = true;

            cts = new();
            channel = Channel.CreateUnbounded<TraceEventArgs>();

            if (demux > 1)
            {
                tasks.AddRange(Split(channel.Reader, demux).Select((channel, index) => ReadAsync(channel.Reader, index, cts.Token)));
            }
            else
            {
                tasks.Add(ReadAsync(channel.Reader, 0, cts.Token));
            }
        }

        public void Close(TimeSpan timeout)
        {
            if (!Running) return;
            Running = false;
         
            channel.Writer.TryComplete();
            cts.CancelAfter(timeout);
            try
            {
                Task.WhenAll(tasks).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            tasks.Clear();
        }

        private Channel<TraceEventArgs>[] Split(ChannelReader<TraceEventArgs> reader, int demux)
        {
            var channels = Enumerable.Range(1, Math.Max(1, demux)).Select(i => Channel.CreateUnbounded<TraceEventArgs>()).ToArray();

            Task.Run(async () =>
            {
                try
                {
                    int index = 0;
                    try
                    {
                        while (await reader.WaitToReadAsync(cts.Token))
                        {
                            while (reader.TryRead(out var item))
                            {
                                if (cts.Token.IsCancellationRequested) return;
                                await channels[index].Writer.WriteAsync(item);
                                index = (index + 1) % demux;
                            }
                        }
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
                finally
                {
                    foreach (var item in channels)
                    {
                        item.Writer.TryComplete();
                    }
                }
            });

            return channels;
        }

        public ValueTask WriteAsync(TraceEventArgs e) => channel.Writer.WriteAsync(e);
    }
}
