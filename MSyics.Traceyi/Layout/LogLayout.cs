using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public readonly static string DefaultFormatting = "{dateTime:yyyy-MM-ddTHH:mm:ss.fffffffzzz}{tab}{scopeId|_,16:R}{tab}{scopeParentId|_,16:R}{tab}{scopeDepth}{tab}{threadId}{tab}{activityId}{tab}{machineName}{tab}{processId}{tab}{processName}{tab}{action}{tab}{elapsed:d\\.hh\\:mm\\:ss\\.fffffff}{tab}{operationId}{tab}{message}{tab}{extensions}";

        private readonly IFormatProvider formatProvider = new LogLayoutFormatProvider();
        private string format;
        private bool makedFormat;
        private bool useExtensions;

        /// <summary>
        /// TextLayout クラスのインスタンスを初期化します。
        /// </summary>
        public LogLayout(string formatting) => Formatting = formatting;

        /// <summary>
        /// TextLayout クラスのインスタンスを初期化します。
        /// </summary>
        public LogLayout() : this(DefaultFormatting)
        {
        }

        /// <summary>
        /// レイアウトを取得または設定します。
        /// </summary>
        public string Formatting
        {
            get => _formatting;
            set
            {
                if (_formatting == value) { return; }
                _formatting = value;
                makedFormat = false;
            }
        }
        private string _formatting;

        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        public string NewLine { get; set; }


        class MyClass
        {
            [JsonExtensionData]
            public IDictionary<string, object> Items { get; set; } = new Dictionary<string, object>();
        }

        /// <summary>
        /// ログにフォーマットした情報を書き込みます。
        /// </summary>
        /// <param name="e">トレースイベントデータ</param>
        public string Format(TraceEventArgs e)
        {
            try
            {
                MyClass myClass = new();
                myClass.Items["Traced"] = e.Traced;
                myClass.Items["action"] = e.Action;
                myClass.Items["elapsed"] = e.Elapsed;
                if (e.Message is not null) { myClass.Items["message"] = e.Message; }
                if (e.ActivityId is not null) { myClass.Items["activityId"] = e.ActivityId; }
                if (e.Scope?.OperationId is not null) { myClass.Items["operationId"] = e.Scope.OperationId; }
                if (e.Scope?.Id is not null) { myClass.Items["scopeId"] = e.Scope.Id; }
                if (e.Scope?.ParentId is not null) { myClass.Items["scopeParentId"] = e.Scope.ParentId; }
                if (e.Scope?.Depth is not null) { myClass.Items["scopeDepth"] = e.Scope.Depth; }
                myClass.Items["threadId"] = e.ThreadId;
                myClass.Items["processId"] = e.ProcessId;
                myClass.Items["processName"] = e.ProcessName;
                myClass.Items["machineName"] = e.MachineName;
                foreach (var ex in e.Extensions.Where(x => x.Value is not null))
                {
                    myClass.Items[ex.Key] = ex.Value;
                }

                return JsonSerializer.Serialize(myClass, options);
            }
            catch (Exception ex)
            {
                Debug.Print($"Failed to Json conversion of extensions. {ex}");
                return e.Extensions.ToString();
            }

            MakeFormat();
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
                e.Scope.OperationId,
                e.Scope.Id,
                e.Scope.ParentId,
                e.Scope.Depth,
                e.ThreadId,
                e.ProcessId,
                e.ProcessName,
                e.MachineName,
                GetExtensionsJson(e));
        }

        private void MakeFormat()
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
                new LogLayoutPart { Name = "scopeParentId", CanFormat = true },
                new LogLayoutPart { Name = "scopeDepth", CanFormat = true },
                new LogLayoutPart { Name = "threadId", CanFormat = true },
                new LogLayoutPart { Name = "processId", CanFormat = true },
                new LogLayoutPart { Name = "processName", CanFormat = true },
                new LogLayoutPart { Name = "machineName", CanFormat = true },
                new LogLayoutPart { Name = "extensions", CanFormat = false });

            format = converter.Convert(Formatting.Trim());
            useExtensions = converter.IsPartPlaced("extensions");
            makedFormat = true;
        }

        readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter(),
                new TimeSpanToStringJsonConverter(),
                new ExceptionToStringJsonConverter(),
            }
        };

        private string GetExtensionsJson(TraceEventArgs e)
        {
            if (!useExtensions) { return null; }
            if (e.Extensions.Count == 0) { return null; }

            try
            {
                return JsonSerializer.Serialize(e.Extensions, options);
            }
            catch (Exception ex)
            {
                Debug.Print($"Failed to Json conversion of extensions. {ex}");
                return e.Extensions.ToString();
            }
        }
    }
}
