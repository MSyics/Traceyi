using MSyics.Traceyi.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MSyics.Traceyi.Tests.Layout
{
    public class LogTests
    {
        readonly ITestOutputHelper testOutput;

        public LogTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        /****************************************************************
         * Convert
         ****************************************************************/
        #region Convert
        [Fact]
        public void When__Expect_()
        {
            //LogLayout layout = new("{@=>json,indent}{newline}{message}{newline}{extensions=>json,indent}");
            LogLayout layout = new("{extensions}");

            var s = layout.GetLog(new TraceEventArgs(new TraceScope(), DateTimeOffset.MaxValue, TraceAction.Info, new SystemException("aaa", new SystemException()), x =>
            {
                x.hoge = "hogehoge";
                x.exception = new SystemException("aaa", new SystemException("aaa"));
            }));

            testOutput.WriteLine(s);

        }
        #endregion
    }
}
