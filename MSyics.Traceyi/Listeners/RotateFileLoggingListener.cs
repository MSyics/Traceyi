using MSyics.Traceyi.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースデータをローテーションファイルに記録します。
    /// </summary>
    public class RotateFileLoggingListener : LoggingListener
    {
        private object m_thisLock = new object();

        /// <summary>
        /// RotateFileLog クラスのインスタンスを初期化します。
        /// </summary>
        public RotateFileLoggingListener(string pathLayout, ILogFormatter layout)
        {
            this.PathLayout = string.IsNullOrEmpty(pathLayout) ? Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName) + ".log" : pathLayout;
            this.Layout = layout;

            this.SetFormattedPathLayout();
        }

        /// <summary>
        /// RotateFileLog クラスのインスタンスを初期化します。
        /// </summary>
        public RotateFileLoggingListener(string pathLayout)
            : this(pathLayout, new LogFormatter())
        {
        }

        /// <summary>
        /// RotateFileLog クラスのインスタンスを初期化します。
        /// </summary>
        public RotateFileLoggingListener()
            : this(string.Empty)
        {
        }

        private void SetFormattedPathLayout()
        {
            var converter = new LogLayoutConverter(
                new LogLayoutPart { Name = "dateTime", CanFormat = true },
                new LogLayoutPart { Name = "threadId", CanFormat = true },
                new LogLayoutPart { Name = "processId", CanFormat = true },
                new LogLayoutPart { Name = "processName", CanFormat = true },
                new LogLayoutPart { Name = "machineName", CanFormat = true });

            this.FormattedPathLayout = converter.Convert(this.PathLayout.Trim());
        }

        /// <summary>
        /// パスを作成します。
        /// </summary>
        private string MakePath(TraceEventArg e)
        {
            var path = string.Format(
                this.FormatProvider,
                this.FormattedPathLayout,
                e.Traced,
                e.ThreadId,
                e.ProcessId,
                e.ProcessName,
                e.MachineName);

            if (!Path.IsPathRooted(path))
            {
                path = Path.GetFullPath(path);
            }

            if (!StreamManager.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            return path;
        }

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        public override void Write(TraceEventArg e)
        {
            lock (m_thisLock)
            {
                var path = this.MakePath(e);

                this.Rotate(path);

                var log = new FileLoggingListener(this.StreamManager.AddOrUpdate(path), this.Encoding, this.Layout)
                {
                    Name = this.Name,
                    NewLine = this.NewLine,
                    UseGlobalLock = false,
                };
                using (log)
                {
                    log.Write(e);
                }
            }
        }

        /// <summary>
        /// ファイルをローテーションします。
        /// </summary>
        private void Rotate(string path)
        {
            if (this.MaxLength <= 0) { return; }

            ReuseFileStream stream;
            if (!this.StreamManager.TryGet(path, out stream)) { return; }
            if (this.MaxLength >= stream.Length) { return; }

            if (File.Exists(path))
            {
                this.StreamManager.Remove(path);
                if (this.LeaveFiles)
                {
                    // 指定サイズ以上になるファイルの名前を変える。
                    var fileName = Path.GetFileNameWithoutExtension(path);
                    var extension = Path.GetExtension(path);
                    var directoryName = Path.GetDirectoryName(path);
                    var fileCount = Directory.GetFiles(directoryName, fileName + "-?*" + extension, SearchOption.TopDirectoryOnly).Count() + 1;

                    File.Move(path, directoryName + "\\" + fileName + "-" + fileCount + extension);
                }
                else
                {
                    File.Delete(path);
                }
            }
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~RotateFileLoggingListener()
        {
            Dispose(false);
        }

        /// <summary>
        /// アンマネージリソースを破棄します。
        /// </summary>
        protected override void DisposeUnmanagedResources()
        {
            try
            {
                try
                {
                    this.StreamManager?.Clear();
                }
                catch (Exception)
                {
                    // 余計な例外を潰すため
                }
            }
            finally
            {
                base.DisposeUnmanagedResources();
            }
        }

        private IFormatProvider FormatProvider { get; set; } = new LogLayoutFormatProvider();

        /// <summary>
        /// パスのレイアウトを取得します。
        /// </summary>
        public string PathLayout { get; private set; }

        /// <summary>
        /// レイアウトからフォーマットした値を取得または設定します。
        /// </summary>
        private string FormattedPathLayout { get; set; }

        /// <summary>
        /// トレースデータの記録形式を取得または設定します。
        /// </summary>
        public ILogFormatter Layout { get; set; }

        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        public string NewLine { get; set; }

        /// <summary>
        /// 文字エンコーディングを取得または設定します。
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// ファイルの書き込み上限バイト数を取得または設定します。
        /// </summary>
        public long MaxLength { get; set; }

        /// <summary>
        /// 書き込み上限バイト数を超えたファイルを残しておくのかどうかを示す値を取得または設定します。
        /// </summary>
        public bool LeaveFiles { get; set; }

        private ReuseFileStreamManager StreamManager { get; set; } = new ReuseFileStreamManager();
    }
}
