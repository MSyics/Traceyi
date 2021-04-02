using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Traceyi
{
    /// <summary>
    /// トレースデータに関するユーティリティークラスです。
    /// </summary>
    internal static class TraceUtility
    {
        /// <summary>
        /// ライブラリで定義されているすべての型を取得します。
        /// </summary>
        internal static Type[] LibraryTypes { get; } = Assembly.GetExecutingAssembly().GetTypes();

        /// <summary>
        /// トレースしたメンバー情報を取得します。
        /// </summary>
        /// <returns>メンバー情報</returns>
        public static MemberInfo GetTracedMemberInfo()
        {
            var stack = new StackTrace(false);
            if (stack.FrameCount == 0) return null;

            return stack.
                GetFrames().
                Reverse().
                Select(x => x.GetMethod()).
                TakeWhile(x => !LibraryTypes.Any(y => y.Equals(x.ReflectedType))).
                Last();
        }

        /// <summary>
        /// トレースするメンバーから操作識別子を取得します。
        /// </summary>
        /// <returns>操作識別子</returns>
        internal static string GetOperationId()
        {
            var stack = new StackTrace(false);
            if (stack.FrameCount == 0) return null;

            var method = stack.
                GetFrames().
                Reverse().
                Select(x => x.GetMethod()).
                TakeWhile(x => !LibraryTypes.Any(y => y.Equals(x.ReflectedType))).
                Last();
            return $"{method.ReflectedType.FullName}.{method.Name}";
        }
    }
}
