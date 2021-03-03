using Microsoft.Extensions.Logging;
using MSyics.Traceyi.Layout;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MSyics.Traceyi
{
    /// <summary>
    /// ILogger 実装の TraceyiLogger のメッセージを表します。
    /// </summary>
    public sealed class TraceyiLoggerMessage
    {
        private static readonly ConcurrentDictionary<string, string> formats = new ConcurrentDictionary<string, string>();

        internal static TraceyiLoggerMessage Create<TState>(EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = new TraceyiLoggerMessage
            {
                EventId = eventId,
                Exception = exception,
                Formatted = formatter?.Invoke(state, exception),
                State = state,
            };

            if (state is IEnumerable<KeyValuePair<string, object>> placeholders)
            {
                message.Placeholders = placeholders?.ToArray();
            }

            return message;
        }

        internal static TraceyiLoggerMessage Create<TState>(TState state)
        {
            var message = new TraceyiLoggerMessage
            {
                State = state,
            };

            if (state is IEnumerable<KeyValuePair<string, object>> placeholders)
            {
                message.Placeholders = placeholders?.ToArray();

                var original = message.Placeholders.FirstOrDefault(x => x.Key == "{OriginalFormat}");
                if (original.Key != null)
                {
                    var items = message.Placeholders.Where(x => x.Key != original.Key).ToArray();
                    var format = formats.GetOrAdd(original.Key, _ =>
                    {
#if NETCOREAPP
                        var parts = items.Select(x => new LogLayoutPart
                        {
                            Name = x.Key.AsSpan().TrimStart('{').TrimEnd('}').ToString(),
                            CanFormat = true
                        }).ToArray();
#else
                            var parts = items.Select(x => new LogLayoutPart
                            {
                                Name = x.Key.TrimStart('{').TrimEnd('}'),
                                CanFormat = true
                            }).ToArray();
#endif
                        try
                        {
                            return new LogLayoutConverter(parts).Convert(original.Value?.ToString());
                        }
                        catch (Exception ex)
                        {
                            Debug.Print($"{ex}");
                            return null;
                        }
                    });

                    if (format != null)
                    {
                        try
                        {
                            message.Formatted = string.Format(format, items.Select(x => x.Value).ToArray());
                        }
                        catch (Exception ex)
                        {
                            Debug.Print($"{ex}");
                        }
                    }
                }
            }

            return message;
        }

        private TraceyiLoggerMessage()
        {
        }

        /// <summary>
        /// EventId を取得します。
        /// </summary>
        public EventId EventId { get; private set; }

        /// <summary>
        /// 例外オブジェクトを取得します。
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// フォーマット済みのメッセージを取得します。
        /// </summary>
        public string Formatted { get; private set; }

        /// <summary>
        /// 書き込むエントリーオブジェクトを取得します。
        /// </summary>
        public object State { get; private set; }

        /// <summary>
        /// プレースホルダーのキーと値の一覧を取得します。
        /// </summary>
        public KeyValuePair<string, object>[] Placeholders { get; private set; }

        /// <summary>
        /// 例外オブジェクトがあるかどうかを示す値を取得します。
        /// </summary>
        public bool HasException => Exception != null;

        /// <summary>
        /// フォーマット済みのメッセージがあるかどうかを示す値を取得します。
        /// </summary>
        public bool HasFormatted => !string.IsNullOrWhiteSpace(Formatted);

        /// <summary>
        /// 書き込むエントリーオブジェクトがあるかどうかを示す値を取得します。
        /// </summary>
        public bool HasState => State != null;

        /// <summary>
        /// プレースホルダーのキーと値の一覧があるかどうかを示す値を取得します。
        /// </summary>
        public bool HasPlaceholders => Placeholders?.Length > 0;

        /// <inheritdoc/>
        public override string ToString()
        {
            var message = $"{(HasFormatted ? Formatted : State?.ToString())}";
            if (HasException)
            {
                message += Environment.NewLine + Exception.ToString();
            }
            return message;
        }
    }
}