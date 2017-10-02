﻿/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Traceyi.Layout
{
    /// <summary>
    /// 書式設定されたログを取得する機能を提供します。
    /// </summary>
    public interface ILogFormatter
    {
        /// <summary>
        /// 書式設定されたログデータを取得します。
        /// </summary>
        /// <param name="e">トレースイベントデータ</param>
        /// <returns>ログデータ</returns>
        string Format(TraceEventArg e);
    }
}
