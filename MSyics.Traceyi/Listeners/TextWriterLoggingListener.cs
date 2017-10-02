/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Layout;
using System.IO;
using System.Text;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースデータを TextWriter オブジェクトを使用して記録します。
    /// </summary>
    public abstract class TextWriterLoggingListener : LoggingListener
    {
        /// <summary>
        /// TextWriter クラスのインスタンスを初期化します。
        /// </summary>
        public TextWriterLoggingListener(TextWriter writer, ILogFormatter layout)
        {
            this.TextWriter = writer;
            this.Layout = layout;
        }

        /// <summary>
        /// TextWriter クラスのインスタンスを初期化します。
        /// </summary>
        public TextWriterLoggingListener(TextWriter writer)
            : this(writer, new LogFormatter())
        {
        }

        /// <summary>
        /// TextWriter クラスのインスタンスを初期化します。
        /// </summary>
        protected TextWriterLoggingListener()
            : this(TextWriter.Null)
        {
        }

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        public override void Write(TraceEventArg e)
        {
            this.TextWriter.WriteLine(this.Layout.Format(e));
        }

        /// <summary>
        /// ライターのすべてのバッファーをクリアし、基になるデバイスに書き込みます。
        /// </summary>
        public virtual void Flush() => this.TextWriter.Flush();

        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        public string NewLine
        {
            get { return this.TextWriter.NewLine; }
            set
            {
                this.TextWriter.NewLine = value;

                if (this.Layout is LogFormatter)
                {
                    ((LogFormatter)this.Layout).NewLine = value;
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
        protected ILogFormatter Layout { get; private set; }

        /// <summary>
        /// 文字エンコーディングを取得します。
        /// </summary>
        public Encoding Encoding => this.TextWriter.Encoding;
    }
}
