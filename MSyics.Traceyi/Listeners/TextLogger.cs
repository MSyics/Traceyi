using MSyics.Traceyi.Layout;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Traceyi.Listeners
{
    /// <summary>
    /// トレースデータを TextWriter オブジェクトを使用して記録します。
    /// </summary>
    public abstract class TextLogger : Logger
    {
        /// <summary>
        /// TextWriter クラスのインスタンスを初期化します。
        /// </summary>
        public TextLogger(TextWriter writer, ILogLayout layout)
        {
            TextWriter = writer;
            Layout = layout;
        }

        /// <summary>
        /// TextWriter クラスのインスタンスを初期化します。
        /// </summary>
        public TextLogger(TextWriter writer) : this(writer, new LogLayout())
        {
        }

        /// <summary>
        /// TextWriter クラスのインスタンスを初期化します。
        /// </summary>
        protected TextLogger() : this(TextWriter.Null)
        {
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            TextWriter.Dispose();
        }

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        protected internal override void WriteCore(TraceEventArgs e)
        {
            try
            {
                var line = Layout.Format(e);
                if (string.IsNullOrEmpty(line)) { return; }
                TextWriter.WriteLine(line);
            }
            catch (FormatException)
            {
                TextWriter.WriteLine($"Can't write in because the layout is in the wrong format.");
            }
            catch (Exception ex)
            {
                TextWriter.WriteLine($"Can't write in.{NewLine}{ex}");
            }
        }

        /// <summary>
        /// ライターのすべてのバッファーをクリアして基になるデバイスに書き込みます。
        /// </summary>
        public virtual void Flush() => TextWriter.Flush();

        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        public string NewLine
        {
            get => TextWriter.NewLine;
            set
            {
                TextWriter.NewLine = value;

                if (Layout is LogLayout layout)
                {
                    layout.NewLine = value;
                }
            }
        }

        /// <summary>
        /// TextWriter オブジェクトを取得または設定します。
        /// </summary>
        protected TextWriter TextWriter { get; set; }

        /// <summary>
        /// ログデータのレイアウト機能を取得または設定します。
        /// </summary>
        public ILogLayout Layout { get; set; }

        /// <summary>
        /// 文字エンコーディングを取得します。
        /// </summary>
        public virtual Encoding Encoding { get; set; } = Encoding.Default;
    }
}
