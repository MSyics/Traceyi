using System;

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
        public readonly static string DefaultLayout = "{dateTime:yyyy/MM/dd}{tab}{dateTime:HH:mm:ss.ffff}{tab}{action}{tab}{operationId}{tab}{activityId}{tab}{threadId}{tab}{processId}{tab}{processName}{tab}{machineName}{tab}{message}";

        /// <summary>
        /// TextLayout クラスのインスタンスを初期化します。
        /// </summary>
        public LogLayout(string layout) => Layout = layout;

        /// <summary>
        /// TextLayout クラスのインスタンスを初期化します。
        /// </summary>
        public LogLayout() : this(DefaultLayout)
        {
        }

        /// <summary>
        /// ログにフォーマットした情報を書き込みます。
        /// </summary>
        /// <param name="e">トレースイベントデータ</param>
        public string Format(TraceEventArg e)
        {
            SetFormattedLayout();

            return string.Format(
                FormatProvider,
                FormattedLayout,
                "\t",
                NewLine,
                e.Traced,
                e.Action,
                e.Message,
                e.ActivityId,
                e.OperationId,
                e.ThreadId,
                e.ProcessId,
                e.ProcessName,
                e.MachineName);
        }

        private void SetFormattedLayout()
        {
            if (IsMakeFormattedLayout) { return; }

            var converter = new LogLayoutConverter(
                new LogLayoutPart { Name = "tab", CanFormat = false },
                new LogLayoutPart { Name = "newLine", CanFormat = false },
                new LogLayoutPart { Name = "dateTime", CanFormat = true },
                new LogLayoutPart { Name = "action", CanFormat = true },
                new LogLayoutPart { Name = "message", CanFormat = true },
                new LogLayoutPart { Name = "activityId", CanFormat = true },
                new LogLayoutPart { Name = "operationId", CanFormat = true },
                new LogLayoutPart { Name = "threadId", CanFormat = true },
                new LogLayoutPart { Name = "processId", CanFormat = true },
                new LogLayoutPart { Name = "processName", CanFormat = true },
                new LogLayoutPart { Name = "machineName", CanFormat = true });

            FormattedLayout = converter.Convert(Layout.Trim());
            IsMakeFormattedLayout = true;
        }

        /// <summary>
        /// レイアウトを取得または設定します。
        /// </summary>
        public string Layout
        {
            get => _layout;
            set
            {
                if (_layout == value) { return; }
                _layout = value;
                IsMakeFormattedLayout = false;
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
