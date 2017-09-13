using System;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// トレースデータをテキストで記録するときのレイアウトを表します。
    /// </summary>
    public sealed class TextLogLayout : ILogLayout
    {
        /// <summary>
        /// 初期レイアウトを示す固定値です。
        /// </summary>
        public const string DefaultLayout = "{dateTime:yyyy/MM/dd}{tab}{dateTime:HH:mm:ss.ffff}{tab}{action}{tab}{operationId}{tab}{activityId}{tab}{class}{tab}{member}{tab}{threadId}{tab}{processId}{tab}{processName}{tab}{machineName}{tab}{message}";

        /// <summary>
        /// TextLayout クラスのインスタンスを初期化します。
        /// </summary>
        public TextLogLayout(string layout) => this.Layout = layout;

        /// <summary>
        /// TextLayout クラスのインスタンスを初期化します。
        /// </summary>
        public TextLogLayout()
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
        public string Format(object message, DateTime dateTime, TraceAction action, TraceEventCacheData cacheData)
        {
            SetFormattedLayout();

            return string.Format(
                    this.FormatProvider,
                    this.FormattedLayout,
                    "\t",
                    this.NewLine,
                    dateTime,
                    action,
                    message,
                    cacheData.ActivityId,
                    cacheData.OperationId,
                    cacheData.Member.ReflectedType,
                    cacheData.Member,
                    cacheData.ThreadId,
                    cacheData.ProcessId,
                    cacheData.ProcessName,
                    cacheData.MachineName);
        }

        private void SetFormattedLayout()
        {
            if (this.IsMakeFormattedLayout) { return; }

            var converter = new TextLogLayoutConverter(
                new TextLogLayoutItem { Name = "tab", UseFormat = false },
                new TextLogLayoutItem { Name = "newLine", UseFormat = false },
                new TextLogLayoutItem { Name = "dateTime", UseFormat = true },
                new TextLogLayoutItem { Name = "action", UseFormat = true },
                new TextLogLayoutItem { Name = "message", UseFormat = true },
                new TextLogLayoutItem { Name = "activityId", UseFormat = true },
                new TextLogLayoutItem { Name = "operationId", UseFormat = true },
                new TextLogLayoutItem { Name = "class", UseFormat = true },
                new TextLogLayoutItem { Name = "member", UseFormat = true },
                new TextLogLayoutItem { Name = "threadId", UseFormat = true },
                new TextLogLayoutItem { Name = "processId", UseFormat = true },
                new TextLogLayoutItem { Name = "processName", UseFormat = true },
                new TextLogLayoutItem { Name = "machineName", UseFormat = true });

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

        private IFormatProvider FormatProvider { get; set; } = new TextLogLayoutFormat();
        private string FormattedLayout { get; set; }
        private bool IsMakeFormattedLayout { get; set; }
    }
}
