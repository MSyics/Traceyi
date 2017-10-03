/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Layout;
using System;
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
        public RotateFileLoggingListener(string pathLayout, ILogLayout layout)
        {
            PathLayout = string.IsNullOrEmpty(pathLayout) ? Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName) + ".log" : pathLayout;
            Layout = layout;

            SetFormattedPathLayout();
        }

        /// <summary>
        /// RotateFileLog クラスのインスタンスを初期化します。
        /// </summary>
        public RotateFileLoggingListener(string pathLayout)
            : this(pathLayout, new LogLayout())
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

            FormattedPathLayout = converter.Convert(PathLayout.Trim());
        }

        /// <summary>
        /// パスを作成します。
        /// </summary>
        private string MakePath(TraceEventArg e)
        {
            var path = string.Format(
                FormatProvider,
                FormattedPathLayout,
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
                var path = MakePath(e);
                Rotate(path);

                var log = new FileLoggingListener(StreamManager.AddOrUpdate(path), Encoding, Layout)
                {
                    Name = Name,
                    NewLine = NewLine,
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
            if (MaxLength <= 0) { return; }

            if (!StreamManager.TryGet(path, out var stream)) { return; }
            if (MaxLength >= stream.Length) { return; }

            if (File.Exists(path))
            {
                StreamManager.Remove(path);
                if (LeaveFiles)
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
                    StreamManager?.Clear();
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
        public ILogLayout Layout { get; set; }

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
