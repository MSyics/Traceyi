using System;
using System.IO;
using System.Text;
using System.Linq;
using MSyics.Traceyi.Layout;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースデータを TextWriter オブジェクトを使用して記録します。
    /// </summary>
    public abstract class TextWriterLog : Log
    {
        /// <summary>
        /// TextWriter クラスのインスタンスを初期化します。
        /// </summary>
        public TextWriterLog(TextWriter writer, ILogLayout layout)
        {
            this.TextWriter = writer;
            this.Layout = layout;
        }

        /// <summary>
        /// TextWriter クラスのインスタンスを初期化します。
        /// </summary>
        public TextWriterLog(TextWriter writer)
            : this(writer, new TextLogLayout())
        {
        }

        /// <summary>
        /// TextWriter クラスのインスタンスを初期化します。
        /// </summary>
        protected TextWriterLog()
            : this(TextWriter.Null)
        {
        }

        /// <summary>
        /// トレースデータを書き込みます。
        /// </summary>
        public override void Write(object message, DateTime dateTime, TraceAction action, TraceEventCacheData cacheData)
        {
            this.TextWriter.WriteLine(this.Layout.Format(message, dateTime, action, cacheData));
        }

        /// <summary>
        /// ライターのすべてのバッファーをクリアし、基になるデバイスに書き込みます。
        /// </summary>
        public virtual void Flush()
        {
            this.TextWriter.Flush();
        }

        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        public string NewLine
        {
            get { return this.TextWriter.NewLine; }
            set
            {
                this.TextWriter.NewLine = value;

                if (this.Layout is TextLogLayout)
                {
                    ((TextLogLayout)this.Layout).NewLine = value;
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
        protected ILogLayout Layout { get; private set; }

        /// <summary>
        /// 文字エンコーディングを取得します。
        /// </summary>
        public Encoding Encoding => this.TextWriter.Encoding;
    }
}
