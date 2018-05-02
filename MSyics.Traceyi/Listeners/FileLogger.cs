/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Layout;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MSyics.Traceyi.Listeners
{
    /// <summary>
    /// トレースデータをファイルに記録します。
    /// </summary>
    public class FileLogger : Logger
    {
        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        public FileLogger(string pathLayout, ILogLayout layout)
        {
            Path = string.IsNullOrEmpty(pathLayout) ? System.IO.Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName) + ".log" : pathLayout;
            Layout = layout;
            SetFormattedPathLayout();
        }

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        public FileLogger(string pathLayout)
            : this(pathLayout, new LogLayout())
        {
        }

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        public FileLogger()
            : this(string.Empty)
        {
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            StreamManager.Clear();
        }

        private void SetFormattedPathLayout()
        {
            var converter = new LogLayoutConverter(
                new LogLayoutPart { Name = "dateTime", CanFormat = true },
                new LogLayoutPart { Name = "threadId", CanFormat = true },
                new LogLayoutPart { Name = "processId", CanFormat = true },
                new LogLayoutPart { Name = "processName", CanFormat = true },
                new LogLayoutPart { Name = "machineName", CanFormat = true });

            FormattedPath = converter.Convert(Path.Trim());
        }

        /// <summary>
        /// パスを作成します。
        /// </summary>
        private string MakePath(TraceEventArg e)
        {
            var path = string.Format(
                FormatProvider,
                FormattedPath,
                e.Traced,
                e.ThreadId,
                e.ProcessId,
                e.ProcessName,
                e.MachineName);

            if (!System.IO.Path.IsPathRooted(path))
            {
                path = System.IO.Path.GetFullPath(path);
            }

            if (!StreamManager.Exists(path))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            }

            return path;
        }

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        public override void WriteCore(TraceEventArg e)
        {
            var path = MakePath(e);
            Rotate(path);

            var log = new BasicFileLogger(StreamManager.GetOrAdd(path), Encoding, Layout)
            {
                Name = Name,
                NewLine = NewLine,
                UseLock = false,
            };
            using (log)
            {
                log.WriteCore(e);
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
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                    var extension = System.IO.Path.GetExtension(path);
                    var directoryName = System.IO.Path.GetDirectoryName(path);
                    var fileCount = Directory.GetFiles(directoryName, fileName + "-?*" + extension, SearchOption.TopDirectoryOnly).Count() + 1;

                    File.Move(path, directoryName + "\\" + fileName + "-" + fileCount + extension);
                }
                else
                {
                    File.Delete(path);
                }
            }
        }

        private IFormatProvider FormatProvider { get; set; } = new LogLayoutFormatProvider();

        /// <summary>
        /// パスを取得します。
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// レイアウトからフォーマットした値を取得または設定します。
        /// </summary>
        private string FormattedPath { get; set; }

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
        public Encoding Encoding { get; set; } = Encoding.Default;

        /// <summary>
        /// ファイルの書き込み上限バイト数を取得または設定します。
        /// </summary>
        public long MaxLength { get; set; }

        /// <summary>
        /// 書き込み上限バイト数を超えたファイルを残しておくのかどうかを示す値を取得または設定します。
        /// </summary>
        public bool LeaveFiles { get; set; }

        private ReuseFileStreamManager StreamManager { get; } = new ReuseFileStreamManager();
    }
}
