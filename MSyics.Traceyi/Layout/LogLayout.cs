﻿using System;
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
        public readonly static string DefaultLayout = "{dateTime:yyyy-MM-ddTHH:mm:ss.fffffffzzz}{tab}{scopeId|_,16:R}{tab}{scopeParentId|_,16:R}{tab}{scopeDepth}{tab}{threadId}{tab}{activityId}{tab}{machineName}{tab}{processId}{tab}{processName}{tab}{action}{tab}{elapsed:d\\.hh\\:mm\\:ss\\.fffffff}{tab}{operationId}{tab}{message}{tab}{extensions}";

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

            //try
            //{
            //    return JsonSerializer.Serialize(e, options);
            //}
            //catch (Exception ex)
            //{
            //    Debug.Print($"Failed to Json conversion of extensions. {ex}");
            //    return e.Extensions.ToString();
            //}

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
                new LogLayoutPart { Name = "scopeParentId", CanFormat = true },
                new LogLayoutPart { Name = "scopeDepth", CanFormat = true },
                new LogLayoutPart { Name = "threadId", CanFormat = true },
                new LogLayoutPart { Name = "processId", CanFormat = true },
                new LogLayoutPart { Name = "processName", CanFormat = true },
                new LogLayoutPart { Name = "machineName", CanFormat = true },
                new LogLayoutPart { Name = "extensions", CanFormat = false });

            format = converter.Convert(Layout.Trim());
            useExtensions = converter.IsPartPlaced("extensions");
            makedFormat = true;
        }

        readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = false,
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
