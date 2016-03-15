using System.IO;

namespace MSyics.Traceyi
{
    /// <summary>
    /// StreamWriter を閉じてもストリームを閉じないようにして、使いまわすようにするためのクラスです。
    /// </summary>
    internal sealed class ReuseFileStream : FileStream
    {
        /// <summary>
        /// ReuseFileStream クラスのインスタンスを初期化します。
        /// </summary>
        public ReuseFileStream(string path)
            : base(path, FileMode.Append, FileAccess.Write, FileShare.Read)
        {
        }

        /// <summary>
        /// このメソッドの代わりに Clean メソッドを使用してください。
        /// </summary>
        public override void Close()
        {
            // StreamWriter を閉じてもストリームを閉じないようにするために、
            // ここでは何もしません。
        }

        /// <summary>
        /// 現在のストリームを閉じて関連付けられているリソースを解放します。
        /// </summary>
        public void Clean()
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
