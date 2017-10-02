using System;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// 指定したレイアウトで書式設定されたログを取得します。
    /// </summary>
    public sealed class LogFormatter : ILogFormatter
    {
        /// <summary>
        /// 初期レイアウトを示す固定値です。
        /// </summary>
        public const string DefaultLayout = "{dateTime:yyyy/MM/dd}{tab}{dateTime:HH:mm:ss.ffff}{tab}{action}{tab}{operationId}{tab}{activityId}{tab}{class}{tab}{member}{tab}{threadId}{tab}{processId}{tab}{processName}{tab}{machineName}{tab}{message}";

        /// <summary>
        /// TextLayout クラスのインスタンスを初期化します。
        /// </summary>
        public LogFormatter(string layout) => this.Layout = layout;

        /// <summary>
        /// TextLayout クラスのインスタンスを初期化します。
        /// </summary>
        public LogFormatter()
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
                    e.ClassName,
                    e.MemberName,
                    e.ThreadId,
                    e.ProcessId,
                    e.ProcessName,
                    e.MachineName);
        }

        private void SetFormattedLayout()
        {
            if (this.IsMakeFormattedLayout) { return; }

            var converter = new LogLayoutConverter(
                new LogLayoutPart { Name = "tab", CanFormat = false },
                new LogLayoutPart { Name = "newLine", CanFormat = false },
                new LogLayoutPart { Name = "dateTime", CanFormat = true },
                new LogLayoutPart { Name = "action", CanFormat = true },
                new LogLayoutPart { Name = "message", CanFormat = true },
                new LogLayoutPart { Name = "activityId", CanFormat = true },
                new LogLayoutPart { Name = "operationId", CanFormat = true },
                new LogLayoutPart { Name = "class", CanFormat = true },
                new LogLayoutPart { Name = "member", CanFormat = true },
                new LogLayoutPart { Name = "threadId", CanFormat = true },
                new LogLayoutPart { Name = "processId", CanFormat = true },
                new LogLayoutPart { Name = "processName", CanFormat = true },
                new LogLayoutPart { Name = "machineName", CanFormat = true });

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

        private IFormatProvider FormatProvider { get; set; } = new LogLayoutFormatProvider();
        private string FormattedLayout { get; set; }
        private bool IsMakeFormattedLayout { get; set; }
    }
}
