using MSyics.Traceyi.Layout;
using System;
using Diagnostics = System.Diagnostics;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースデータをイベントログに記録します。
    /// </summary>
    public class EventLog : Log
    {
        /// <summary>
        /// EventLog クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="sourceName">ソース名</param>
        /// <param name="logName">ログ名</param>
        /// <param name="machineName">マシン名</param>
        /// <param name="layout">ログの記録形式</param>
        public EventLog(string sourceName, string logName, string machineName, ILogLayout layout)
        {
            this.MachineName = machineName;
            this.SourceName = sourceName;
            this.LogName = logName;
            this.HasEventSource = Diagnostics.EventLog.SourceExists(this.SourceName, this.MachineName);
            this.Layout = layout;
        }

        /// <summary>
        /// EventLog クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="sourceName">ソース名</param>
        /// <param name="logName">ログ名</param>
        /// <param name="machineName">マシン名</param>
        public EventLog(string sourceName, string logName, string machineName)
            : this(sourceName, logName, machineName, new TextLogLayout())
        {
        }

        /// <summary>
        /// EventLog クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="sourceName">ソース名</param>
        /// <param name="logName">ログ名</param>
        public EventLog(string sourceName, string logName)
            : this(sourceName, logName, EventLog.LocalMachineName)
        {
        }

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="dateTime">日付</param>
        /// <param name="action">トレース動作</param>
        /// <param name="cacheData">キャッシュデータ</param>
        public override void Write(object message, DateTime dateTime, TraceAction action, TraceEventCacheData cacheData)
        {
            if (!this.HasEventSource) { return; }

            using (var eventLog = CreateEventLog())
            {
                eventLog.WriteEntry(
                    this.Layout.Format(message, dateTime, action, cacheData),
                    GetEventLogEntryType(action), 
                    0, 0, null);
            }
        }

        /// <summary>
        /// トレースの動作からイベントの種類を取得します。
        /// </summary>
        /// <param name="action">トレースの動作</param>
        protected virtual Diagnostics.EventLogEntryType GetEventLogEntryType(TraceAction action)
        {
            switch (action)
            {
                case TraceAction.Warning:
                    return Diagnostics.EventLogEntryType.Warning;
                case TraceAction.Error:
                    return Diagnostics.EventLogEntryType.Error;
                default:
                    return Diagnostics.EventLogEntryType.Information;
            }
        }

        /// <summary>
        /// System.Diagnostics.EventLog クラスのインスタンスを取得します。
        /// </summary>
        protected Diagnostics.EventLog CreateEventLog()
        {
            return new Diagnostics.EventLog(this.LogName, this.MachineName, this.SourceName);
        }

        /// <summary>
        /// イベントソースが存在するかどうかを示す値を取得します。
        /// </summary>
        public bool HasEventSource { get; private set; }

        /// <summary>
        /// ローカルマシン名を表す定数です。
        /// </summary>
        public const string LocalMachineName = ".";

        /// <summary>
        /// マシン名を取得します。
        /// </summary>
        public string MachineName { get; private set; }

        /// <summary>
        /// ソース名を取得します。
        /// </summary>
        public string SourceName { get; private set; }

        /// <summary>
        /// ログ名を取得します。
        /// </summary>
        public string LogName { get; private set; }

        /// <summary>
        /// ログデータのレイアウト機能を取得または設定します。
        /// </summary>
        protected ILogLayout Layout { get; set; }

        #region EventLog Installer

        private static object m_lock = new object();

        /// <summary>
        /// イベントソースとログを登録します。
        /// <para>イベント ソースを作成するには、管理者特権が必要です。</para>
        /// </summary>
        /// <param name="sourceName">ソース名</param>
        /// <param name="logName">ログ名</param>
        /// <param name="machineName">マシン名</param>
        public static void Install(string sourceName, string logName, string machineName)
        {
            lock (m_lock)
            {
                if (!Diagnostics.EventLog.SourceExists(sourceName, machineName))
                {
                    var data = new Diagnostics.EventSourceCreationData(sourceName, logName)
                        {
                            MachineName = Environment.MachineName,
                        };

                    Diagnostics.EventLog.CreateEventSource(data);
                }
            }
        }

        /// <summary>
        /// ローカルマシン名でイベントソースとログを登録します。
        /// <para>イベント ソースを作成するには、管理者特権が必要です。</para>
        /// </summary>
        /// <param name="sourceName">ソース名</param>
        /// <param name="logName">ログ名</param>
        public static void Install(string sourceName, string logName)
        {
            Install(sourceName, logName, EventLog.LocalMachineName);
        }

        /// <summary>
        /// 指定したイベントソースとログを削除します。
        /// <para>このメソッドの実行には、適切なレジストリ アクセス許可を持っている必要があります。</para>
        /// </summary>
        /// <param name="sourceName">ソース名</param>
        /// <param name="logName">ログ名</param>
        /// <param name="machineName">マシン名</param>
        public static void Uninstall(string sourceName, string logName, string machineName)
        {
            lock (m_lock)
            {
                if (Diagnostics.EventLog.SourceExists(sourceName, machineName))
                {
                    //Diagnostics.EventLog.DeleteEventSource(sourceName, machineName);
                    if (Diagnostics.EventLog.Exists(logName, machineName))
                    {
                        Diagnostics.EventLog.Delete(logName, machineName);
                    }
                }
            }
        }

        /// <summary>
        /// ローカルマシン名で登録しているイベントソースとログを削除します。
        /// <para>このメソッドの実行には、適切なレジストリ アクセス許可を持っている必要があります。</para>
        /// </summary>
        /// <param name="sourceName">ソース名</param>
        /// <param name="logName">ログ名</param>
        public static void Uninstall(string sourceName, string logName)
        {
            Uninstall(sourceName, logName, EventLog.LocalMachineName);
        }

        #endregion // End EventLog Installer
    }
}
