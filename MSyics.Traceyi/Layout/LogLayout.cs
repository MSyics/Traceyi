using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MSyics.Traceyi.Layout
{
    public sealed class LogLayoutPartValueSetSettings
    {
        public bool UseAction { get; set; } = true;
        public bool UseTraced { get; set; } = true;
        public bool UseElapsed { get; set; } = true;
        public bool UseActivityId { get; set; } = true;
        public bool UseScopeLabel { get; set; } = true;
        public bool UseScopeId { get; set; } = true;
        public bool UseScopeParentId { get; set; } = true;
        public bool UseScopeDepth { get; set; } = true;
        public bool UseThreadId { get; set; } = true;
        public bool UseProcessId { get; set; } = true;
        public bool UseProcessName { get; set; } = true;
        public bool UseMachineName { get; set; } = true;
        public bool UseMessage { get; set; } = true;
        public bool UseExtensions { get; set; } = true;
        public bool UsePartValueSet { get; set; } = true;
    }

    /// <summary>
    /// 指定したレイアウトで書式設定されたログを取得します。
    /// </summary>
    public sealed class LogLayout : ILogLayout
    {
        /// <summary>
        /// 初期レイアウトを示す固定値です。
        /// </summary>
        public readonly static string DefaultFormat = "{action| ,8:L}{tab}{dateTime:yyyy-MM-ddTHH:mm:ss.fffffffzzz}{tab}{elapsed:d\\.hh\\:mm\\:ss\\.fffffff}{tab}{scopeId|-,16:R}{tab}{scopeParentId|-,16:R}{tab}{scopeDepth}{tab}{scopeLabel}{tab}{activityId}{tab}{threadId}{tab}{processId}{tab}{processName}{tab}{machineName}{tab}{message}{tab}{extensions=>json}{newLine}{@=>json,indent}";

        private readonly IFormatProvider formatProvider = new LogLayoutFormatProvider();
        private bool initialized;
        private string actualFormat;
        private bool hasExtensions;
        private bool hasPartValueSet;

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

        public bool UseAction { get; set; } = true;
        public bool UseTraced { get; set; } = true;
        public bool UseElapsed { get; set; } = true;
        public bool UseActivityId { get; set; } = true;
        public bool UseScopeLabel { get; set; } = true;
        public bool UseScopeId { get; set; } = true;
        public bool UseScopeParentId { get; set; } = true;
        public bool UseScopeDepth { get; set; } = true;
        public bool UseThreadId { get; set; } = true;
        public bool UseProcessId { get; set; } = true;
        public bool UseProcessName { get; set; } = true;
        public bool UseMachineName { get; set; } = true;
        public bool UseMessage { get; set; } = true;
        public bool UseExtensions { get; set; } = true;
        public bool UsePartValueSet { get; set; } = true;

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
        public string NewLine { get; set; }

        public LogLayoutPartValueSetSettings PartValueSetSettings { get; } = new();

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
                GetExtensions(e),
                CreatePartValueSet(e)).TrimEnd('\r', '\n');
        }
        #endregion

        private void Initialize()
        {
            if (initialized) { return; }

            var converter = new LogLayoutConverter(
                new LogLayoutPart { Name = "tab", CanFormat = false },
                new LogLayoutPart { Name = "newLine", CanFormat = false },
                new LogLayoutPart { Name = "action", CanFormat = true },
                new LogLayoutPart { Name = "dateTime", CanFormat = true },
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

            Console.WriteLine(actualFormat);

            hasExtensions = converter.IsPartPlaced("extensions");
            hasPartValueSet = converter.IsPartPlaced("@");

            initialized = true;
        }

        private IDictionary<string, object> GetExtensions(TraceEventArgs e)
        {
            if (!hasExtensions) { return null; }

            return e.Extensions.Count == 0 ? null : e.Extensions;
        }

        private LogState CreatePartValueSet(TraceEventArgs e)
        {
            if (!hasPartValueSet) { return null; }

            return new LogStateBuilder().
                SetValue("action", e.Action, PartValueSetSettings.UseAction, false).
                SetValue("traced", e.Traced, PartValueSetSettings.UseTraced).
                SetValue("elapsed", e.Elapsed, PartValueSetSettings.UseElapsed).
                SetNullableValue("activityId", e.ActivityId, PartValueSetSettings.UseActivityId).
                SetNullableValue("scopeLabel", e.ScopeLabel, PartValueSetSettings.UseScopeLabel).
                SetNullableValue("scopeId", e.ScopeId, PartValueSetSettings.UseScopeId).
                SetNullableValue("scopeParentId", e.ScopeParentId, PartValueSetSettings.UseScopeParentId).
                SetValue("scopeDepth", e.ScopeDepth, PartValueSetSettings.UseScopeDepth).
                SetValue("threadId", e.ThreadId, PartValueSetSettings.UseThreadId).
                SetValue("processId", e.ProcessId, PartValueSetSettings.UseProcessId).
                SetNullableValue("processName", e.ProcessName, PartValueSetSettings.UseProcessName).
                SetNullableValue("machineName", e.MachineName, PartValueSetSettings.UseMachineName).
                SetNullableValue("message", e.Message, PartValueSetSettings.UseMessage).
                SetExtensions(e.Extensions, PartValueSetSettings.UseExtensions).
                Build();
        }
    }
}
