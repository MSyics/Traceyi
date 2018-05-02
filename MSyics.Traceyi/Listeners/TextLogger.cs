/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Layout;
using System;
using System.IO;
using System.Text;

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
        public TextLogger(TextWriter writer)
            : this(writer, new LogLayout())
        {
        }

        /// <summary>
        /// TextWriter クラスのインスタンスを初期化します。
        /// </summary>
        protected TextLogger()
            : this(TextWriter.Null)
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
        public override void WriteCore(TraceEventArg e)
        {
            try
            {
                TextWriter.WriteLine(Layout.Format(e));
            }
            catch (FormatException)
            {
                TextWriter.WriteLine($"レイアウトの書式が間違っているため書き込めません。");
            }
            catch (Exception ex)
            {
                TextWriter.WriteLine($"システム異常のため書き込めません。{NewLine}{ex}");
            }
        }

        /// <summary>
        /// ライターのすべてのバッファーをクリアし、基になるデバイスに書き込みます。
        /// </summary>
        public virtual void Flush() => TextWriter.Flush();

        /// <summary>
        /// 改行文字を取得または設定します。
        /// </summary>
        public string NewLine
        {
            get { return TextWriter.NewLine; }
            set
            {
                TextWriter.NewLine = value;

                if (Layout is LogLayout)
                {
                    ((LogLayout)Layout).NewLine = value;
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
