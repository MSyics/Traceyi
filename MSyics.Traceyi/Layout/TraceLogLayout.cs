using System;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// トレースデータをテキストで記録するときのレイアウトを表します。
    /// </summary>
    public sealed class TraceLogLayout : ITraceLogLayout
    {
        /// <summary>
        /// 初期レイアウトを示す固定値です。
        /// </summary>
        public const string DefaultLayout = "{dateTime:yyyy/MM/dd}{tab}{dateTime:HH:mm:ss.ffff}{tab}{action}{tab}{operationId}{tab}{activityId}{tab}{class}{tab}{member}{tab}{threadId}{tab}{processId}{tab}{processName}{tab}{machineName}{tab}{message}";

        /// <summary>
        /// TextLayout クラスのインスタンスを初期化します。
        /// </summary>
        public TraceLogLayout(string layout) => this.Layout = layout;

        /// <summary>
        /// TextLayout クラスのインスタンスを初期化します。
        /// </summary>
        public TraceLogLayout()
            : this(DefaultLayout)
        {
        }

        /// <summary>
        /// ログにフォーマットした情報を書き込みます。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="dateTime">日時</param>
        /// <param name="action">トレース動作</param>
        /// <param name="cacheData">トレースイベントデータ</param>
        public string Format(TraceEventArg e)
        {
            SetFormattedLayout();

            return string.Format(
                    this.FormatProvider,
                    this.FormattedLayout,
                    "\t",
                    this.NewLine,
                    e.Traced,
                    e.Action,
                    e.Message,
                    e.ActivityId,
                    e.OperationId,
                    e.Class,
                    e.Member,
                    e.ThreadId,
                    e.ProcessId,
                    e.ProcessName,
                    e.MachineName);
        }

        private void SetFormattedLayout()
        {
            if (this.IsMakeFormattedLayout) { return; }

            var converter = new TraceLogLayoutConverter(
                new TraceLogLayoutItem { Name = "tab", CanFormat = false },
                new TraceLogLayoutItem { Name = "newLine", CanFormat = false },
                new TraceLogLayoutItem { Name = "dateTime", CanFormat = true },
                new TraceLogLayoutItem { Name = "action", CanFormat = true },
                new TraceLogLayoutItem { Name = "message", CanFormat = true },
                new TraceLogLayoutItem { Name = "activityId", CanFormat = true },
                new TraceLogLayoutItem { Name = "operationId", CanFormat = true },
                new TraceLogLayoutItem { Name = "class", CanFormat = true },
                new TraceLogLayoutItem { Name = "member", CanFormat = true },
                new TraceLogLayoutItem { Name = "threadId", CanFormat = true },
                new TraceLogLayoutItem { Name = "processId", CanFormat = true },
                new TraceLogLayoutItem { Name = "processName", CanFormat = true },
                new TraceLogLayoutItem { Name = "machineName", CanFormat = true });

            this.FormattedLayout = converter.Convert(this.Layout.Trim());
            this.IsMakeFormattedLayout = true;
        }

        /// <summary>
        /// レイアウトを取得または設定します。
        /// </summary>
        public string Layout
        {
            get { return _layout; }
            set
            {
                if (_layout == value) { return; }
                _layout = value;
                this.IsMakeFormattedLayout = false;
            }
        }
        private string _layout;

        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        public string NewLine { get; set; }

        private IFormatProvider FormatProvider { get; set; } = new TraceLogLayoutFormat();
        private string FormattedLayout { get; set; }
        private bool IsMakeFormattedLayout { get; set; }
    }
}
