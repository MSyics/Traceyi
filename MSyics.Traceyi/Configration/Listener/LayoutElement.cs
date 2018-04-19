﻿/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Traceyi.Layout;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// Layout 要素を表します。
    /// </summary>
    public class LayoutElement
    {
        /// <summary>
        /// レイアウト形式を取得または設定します。
        /// </summary>
        public string Format { get; set; } = LogLayout.DefaultLayout;

        /// <summary>
        /// 実行オブジェクトを取得します。
        /// </summary>
        public LogLayout GetRuntimeObject() => new LogLayout(Format);
    }
}