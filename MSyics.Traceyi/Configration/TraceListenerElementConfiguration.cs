/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace MSyics.Traceyi.Configration
{
    /// <summary>
    /// トレースリスナー要素を構成する機能を提供します。
    /// </summary>
    public interface ITraceListenerElementConfiguration
    {
        void In<T>(string name) where T : TraceListenerElement;
    }

    internal sealed class TraceListenerElementConfiguration : Dictionary<string, Func<IConfiguration, IEnumerable<TraceListenerElement>>>, ITraceListenerElementConfiguration
    {
        public TraceListenerElementConfiguration()
        {
            In<ConsoleElement>("Console");
            In<FileElement>("File");
        }

        /// <summary>
        /// カスタム Listener 要素を登録します。
        /// </summary>
        /// <typeparam name="T">カスタム要素の型</typeparam>
        /// <param name="name">セクション名</param>
        public void In<T>(string name)
            where T : TraceListenerElement
        {
            Add(name.ToUpper(), config => config.Get<List<T>>());
        }
    }
}
