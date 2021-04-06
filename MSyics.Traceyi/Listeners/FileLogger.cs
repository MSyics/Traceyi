using MSyics.Traceyi.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MSyics.Traceyi.Listeners
{
    /// <summary>
    /// トレースデータをファイルに記録します。
    /// </summary>
    public class FileLogger : TextLogger
    {
        readonly MutexFactory namedMutex = MutexFactory.Create();
        readonly IFormatProvider formatProvider = new LogLayoutFormatProvider();

        private FileStreamStore Streams => _streams.Value;
        readonly Lazy<FileStreamStore> _streams;

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        public FileLogger(ILogLayout layout, string path = null, bool useLock = false, bool useAsync = true, int concurrency = 1, bool useMutex = false, bool keepFilesOpen = true) :
            base(TextWriter.Null, layout, useLock, useAsync, concurrency)
        {
            Path = string.IsNullOrWhiteSpace(path) ? $"{System.IO.Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName)}.log" : path;
            FormattedPath = CreateFormattedPath();
            UseMutex = useMutex;
            KeepFilesOpen = keepFilesOpen;

            _streams = new Lazy<FileStreamStore>(
                () =>
                {
                    if (UseMutex && MaxLength > 0)
                    {
                        KeepFilesOpen = false;
                    }
                    return new FileStreamStore(KeepFilesOpen ? new ReuseFileStreamFactory(UseMutex) : new FileStreamFactory());
                },
                true);
        }

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        public FileLogger(string path = null, bool useLock = false, bool useAsync = true, int concurrency = 1, bool useMutex = false, bool keepFilesOpen = true) :
            this(new LogLayout(), path, useLock, useAsync, concurrency, useMutex, keepFilesOpen)
        {
        }

        /// <summary>
        /// クラスのインスタンスを初期化します。
        /// </summary>
        public FileLogger()
            : this(new LogLayout())
        {
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            Streams.Clear();
        }

        private string CreateFormattedPath()
        {
            var converter = new LogLayoutConverter(
                new LogLayoutPart { Name = "dateTime", CanFormat = true },
                new LogLayoutPart { Name = "threadId", CanFormat = true },
                new LogLayoutPart { Name = "processId", CanFormat = true },
                new LogLayoutPart { Name = "processName", CanFormat = true },
                new LogLayoutPart { Name = "machineName", CanFormat = true },
                new LogLayoutPart { Name = "index", CanFormat = true });

            return converter.Convert(Path.Trim());
        }

        /// <summary>
        /// パスを作成します。
        /// </summary>
        protected string MakePath(TraceEventArgs e, int index)
        {
            var path = string.Format(
                formatProvider,
                FormattedPath,
                DateTimeOffset.Now,
                Thread.CurrentThread.ManagedThreadId,
                e.ProcessId,
                e.ProcessName,
                e.MachineName,
                index);

            if (!System.IO.Path.IsPathRooted(path))
            {
                path = System.IO.Path.GetFullPath(path);
            }

            if (!Streams.Exists(path))
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            }

            return path;
        }

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        protected internal override void WriteCore(TraceEventArgs e, int index)
        {
            var path = MakePath(e, index);
            if (UseMutex)
            {
                using var mutex = namedMutex.Get(path);
                try
                {
                    mutex.WaitOne();
                    Write();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                Write();
            }

            void Write()
            {
                try
                {
                    Rotate(path);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                using var writer = new StreamWriter(Streams.GetOrAdd(path), Encoding)
                {
                    AutoFlush = true,
                    NewLine = NewLine,
                };

                try
                {
                    var log = Layout.GetLog(e);
                    if (string.IsNullOrEmpty(log)) return;

                    writer.WriteLine(log);
                }
                catch (Exception ex)
                {
                    writer.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// ファイルをローテーションします。
        /// </summary>
        private void Rotate(string path)
        {
            if (MaxLength <= 0) return;

            Streams.TryGetLength(path, out var length);
            if (MaxLength > length) return;

            Streams.Remove(path);

            var source = new FileInfo(path);
            if (CanArchive)
            {
                Archive(source);
            }
            else
            {
                source.Delete();
            }
        }

        /// <summary>
        /// アーカイブします。
        /// </summary>
        private void Archive(FileInfo source)
        {
            var dir = source.DirectoryName;
            var name = System.IO.Path.GetFileNameWithoutExtension(source.FullName);
            var extension = source.Extension;

            var files = GetFiles();

            foreach (var (_, file) in files.Skip(MaxArchiveCount))
            {
                try
                {
                    file.Delete();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            foreach (var (number, file) in files.Take(MaxArchiveCount).Reverse())
            {
                try
                {
#if NETCOREAPP
                    file.MoveTo($@"{dir}\{name}-{number}{extension}", true);
#else
                    file.MoveTo($@"{dir}\{name}-{number}{extension}");
#endif
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            // アーカイブファイルを取得する
            IEnumerable<(int number, FileInfo file)> GetFiles()
            {
                yield return (1, source);

                var files = Directory.EnumerateFiles(dir, $"{name}-?*{extension}", SearchOption.TopDirectoryOnly).
                    Select(x => new FileInfo(x)).
                    Select(x => (GetNumber(x), x)).
                    Where(x => x.Item1 > 0).
                    OrderBy(x => x.Item1);

                foreach (var file in files)
                {
                    yield return file;
                }
            }

            // アーカイブファイル名から番号を取得する
            int GetNumber(FileInfo file)
            {
                var result = 0;
                try
                {
                    var length = file.FullName.Length - source.FullName.Length - 1;
#if NETCOREAPP
                    var number = file.Name.AsSpan(name.Length + 1, length);
#else
                    var number = file.Name.Substring(name.Length + 1, length);
#endif
                    if (int.TryParse(number, out var value) && value > 0)
                    {
                        result = ++value;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return result;
            }
        }

        /// <summary>
        /// レイアウトからフォーマットした値を取得します。
        /// </summary>
        protected string FormattedPath { get; private set; }

        /// <summary>
        /// パスを取得します。
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// ファイルの書き込み上限バイト数を取得または設定します。
        /// </summary>
        public long MaxLength { get; set; }

        /// <summary>
        /// プロセス間同期を使用するかどうか示す値を取得または設定します。
        /// </summary>
        public bool UseMutex { get; private set; } = false;

        /// <summary>
        /// ファイルを開いたままにしておくかどうかを示す値を取得または設定します。
        /// </summary>
        public bool KeepFilesOpen { get; private set; } = true;

        /// <summary>
        /// 書き込み上限バイト数を超えたファイルの最大アーカイブ数を取得または設定します。
        /// </summary>
        public int MaxArchiveCount { get; set; }

        /// <summary>
        /// アーカイブするかどうかを示す値を取得します。
        /// </summary>
        public bool CanArchive => MaxLength > 0 && MaxArchiveCount > 0;
    }
}
