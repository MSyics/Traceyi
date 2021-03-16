using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public readonly static string DefaultLayout = "{dateTime:O}{tab}{scopeNumber}{tab}{scopeId|_,16:R}{tab}{parentId|_,16:R}{tab}{threadId}{tab}{activityId}{tab}{machineName}{tab}{processId}{tab}{processName}{tab}{action}{tab}{elapsed:d\\.hh\\:mm\\:ss\\.fffffff}{tab}{operationId}{tab}{message}{tab}{extensions}";

        private readonly IFormatProvider formatProvider = new LogLayoutFormatProvider();
        private string format;
        private bool makedFormat;
        private bool useExtensions;

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
        /// レイアウトを取得または設定します。
        /// </summary>
        public string Layout
        {
            get => _layout;
            set
            {
                if (_layout == value) { return; }
                _layout = value;
                makedFormat = false;
            }
        }
        private string _layout;

        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        public string NewLine { get; set; }

        /// <summary>
        /// ログにフォーマットした情報を書き込みます。
        /// </summary>
        /// <param name="e">トレースイベントデータ</param>
        public string Format(TraceEventArgs e)
        {
            SetFormattedLayout();

            return string.Format(
                formatProvider,
                format,
                "\t",
                NewLine,
                e.Traced,
                e.Action,
                e.Elapsed,
                e.Message,
                e.ActivityId,
                e.Operation.Id,
                e.Operation.ScopeId,
                e.Operation.ParentId,
                e.Operation.ScopeNumber,
                e.ThreadId,
                e.ProcessId,
                e.ProcessName,
                e.MachineName,
                GetExtensionsJson(e));
        }

        private void SetFormattedLayout()
        {
            if (makedFormat) { return; }

            var converter = new LogLayoutConverter(
                new LogLayoutPart { Name = "tab", CanFormat = false },
                new LogLayoutPart { Name = "newLine", CanFormat = false },
                new LogLayoutPart { Name = "dateTime", CanFormat = true },
                new LogLayoutPart { Name = "action", CanFormat = true },
                new LogLayoutPart { Name = "elapsed", CanFormat = true },
                new LogLayoutPart { Name = "message", CanFormat = true },
                new LogLayoutPart { Name = "activityId", CanFormat = true },
                new LogLayoutPart { Name = "operationId", CanFormat = true },
                new LogLayoutPart { Name = "scopeId", CanFormat = true },
                new LogLayoutPart { Name = "parentId", CanFormat = true },
                new LogLayoutPart { Name = "scopeNumber", CanFormat = true },
                new LogLayoutPart { Name = "threadId", CanFormat = true },
                new LogLayoutPart { Name = "processId", CanFormat = true },
                new LogLayoutPart { Name = "processName", CanFormat = true },
                new LogLayoutPart { Name = "machineName", CanFormat = true },
                new LogLayoutPart { Name = "extensions", CanFormat = false });

            format = converter.Convert(Layout.Trim());
            useExtensions = converter.IsPartPlaced("extensions");
            makedFormat = true;
        }


        JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = false,
            Converters = { new JsonTimeSpanConverter(), new JsonStringEnumConverter() }
        };

        private string GetExtensionsJson(TraceEventArgs e)
        {
            if (!useExtensions) { return null; }
            if (e.Extensions.Count == 0) { return null; }

            try
            {
                return JsonSerializer.Serialize(e.Extensions, options);
            }
            catch (Exception)
            {
                Debug.Print("Failed to Json conversion of extensions.");
                return e.Extensions.ToString();
            }
        }
    }
}
