using System;
using System.Collections.Generic;
using System.Linq;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// 指定したレイアウトで書式設定されたログを取得します。
    /// </summary>
    public sealed class LogLayout : ILogLayout
    {
        /// <summary>
        /// 初期レイアウトを示す固定値です。
        /// </summary>
        public readonly static string DefaultFormat = "{action| ,8:L}{tab}{traced:yyyy-MM-ddTHH:mm:ss.fffffffzzz}{tab}{elapsed:d\\.hh\\:mm\\:ss\\.fffffff}{tab}{scopeId|-,16:R}{tab}{scopeParentId|-,16:R}{tab}{scopeDepth}{tab}{scopeLabel}{tab}{activityId}{tab}{threadId}{tab}{processId}{tab}{processName}{tab}{machineName}{tab}{message}{tab}{extensions=>json}";

        private readonly IFormatProvider formatProvider = new LogLayoutFormatProvider();
        private bool initialized;
        private string actualFormat;
        private bool hasExtensions;
        private bool hasLogState;

        /// <summary>
        /// TextLayout クラスのインスタンスを初期化します。
        /// </summary>
        public LogLayout(string format) => Format = format;

        /// <summary>
        /// TextLayout クラスのインスタンスを初期化します。
        /// </summary>
        public LogLayout() : this(DefaultFormat)
        {
        }

        /// <summary>
        /// フォーマットを取得または設定します。
        /// </summary>
        public string Format
        {
            get => _format;
            set
            {
                if (_format == value) { return; }
                _format = value;
                initialized = false;
            }
        }
        private string _format;

        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        public string NewLine { get; set; } = Environment.NewLine;

        #region ILogLayout Members
        /// <inheritdoc/>>
        public string GetLog(TraceEventArgs e)
        {
            Initialize();

            return string.Format(
                formatProvider,
                actualFormat,
                "\t",
                NewLine,
                e.Action,
                e.Traced,
                e.Elapsed,
                e.ActivityId,
                e.ScopeLabel,
                e.ScopeId,
                e.ScopeParentId,
                e.ScopeDepth,
                e.ThreadId,
                e.ProcessId,
                e.ProcessName,
                e.MachineName,
                e.Message,
                GetExtensions(ref e),
                GetEvnetArgs(ref e)).
                TrimEnd('\r', '\n');
        }
        #endregion

        private void Initialize()
        {
            if (initialized) { return; }

            var converter = new LogLayoutConverter(
                new LogLayoutPart { Name = "tab", CanFormat = false },
                new LogLayoutPart { Name = "newLine", CanFormat = false },
                new LogLayoutPart { Name = "action", CanFormat = true },
                new LogLayoutPart { Name = "traced", CanFormat = true },
                new LogLayoutPart { Name = "elapsed", CanFormat = true },
                new LogLayoutPart { Name = "activityId", CanFormat = true },
                new LogLayoutPart { Name = "scopeLabel", CanFormat = true },
                new LogLayoutPart { Name = "scopeId", CanFormat = true },
                new LogLayoutPart { Name = "scopeParentId", CanFormat = true },
                new LogLayoutPart { Name = "scopeDepth", CanFormat = true },
                new LogLayoutPart { Name = "threadId", CanFormat = true },
                new LogLayoutPart { Name = "processId", CanFormat = true },
                new LogLayoutPart { Name = "processName", CanFormat = true },
                new LogLayoutPart { Name = "machineName", CanFormat = true },
                new LogLayoutPart { Name = "message", CanFormat = true },
                new LogLayoutPart { Name = "extensions", CanFormat = true },
                new LogLayoutPart { Name = "@", CanFormat = true });

            actualFormat = converter.Convert(Format.Trim());

            hasExtensions = converter.IsPartPlaced("extensions");
            hasLogState = converter.IsPartPlaced("@");

            initialized = true;
        }

        private IDictionary<string, object> GetExtensions(ref TraceEventArgs e)
        {
            if (!hasExtensions) { return null; }
            return e.Extensions.Count == 0 ? null : e.Extensions;
        }

        private TraceEventArgs GetEvnetArgs(ref TraceEventArgs e)
        {
            if (!hasLogState) { return null; }
            return e;
        }
    }
}
