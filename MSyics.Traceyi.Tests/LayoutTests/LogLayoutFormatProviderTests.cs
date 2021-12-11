using Moq;
using MSyics.Traceyi.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MSyics.Traceyi.Tests.Layout
{
    public class LogLayoutFormatProviderTests
    {
        readonly ITestOutputHelper testOutput;

        public LogLayoutFormatProviderTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        /****************************************************************
         * GetFormat
         ****************************************************************/
        #region GetFormat
        [Theory]
        [InlineData(null)]
        [InlineData(typeof(object))]
        public void GetFormat_NotTypeOfICustomFormatter_ReturnsNull(Type type)
        {
            // Arrange
            LogLayoutFormatProvider provider = new();

            // Act
            var format = provider.GetFormat(type);

            // Assert
            Assert.Null(format);
        }

        [Fact]
        public void GetFormat_TypeOfICustomFormatter_ReturnsThisType()
        {
            // Arrange
            LogLayoutFormatProvider provider = new();

            // Act
            var format = provider.GetFormat(typeof(ICustomFormatter));

            // Assert
            Assert.IsType<LogLayoutFormatProvider>(format);
        }
        #endregion

        /****************************************************************
         * Format
         ****************************************************************/
        #region Format
        #region Format_Standards
        [Theory]
        [MemberData(nameof(Format_Standards_Data))]
        public void Format_Standards(string format, object arg, object expected)
        {
            // Arrange
            LogLayoutFormatProvider provider = new();

            // Act
            string actual = string.Format(provider, format, arg);

            // Assert
            Assert.Equal(expected, actual);
        }

        static IEnumerable<object[]> Format_Standards_Data()
        {
            yield return new object[] { "{0}", 0, "0" };
            yield return new object[] { "{0:00}", 0, "00" };
            yield return new object[] { "{0:D2}", 0, "00" };
            yield return new object[] { "{0:d}", DateTimeOffset.MaxValue, DateTimeOffset.MaxValue.ToString("d") };
        }
        #endregion

        #region Format_Customns
        [Theory]
        [MemberData(nameof(Format_Customns_Data))]
        public void Format_Customns(string format, object arg, object expected)
        {
            // Arrange
            LogLayoutFormatProvider provider = new();

            // Act
            string actual = string.Format(provider, format, arg);

            // Assert
            Assert.Equal(expected, actual);
        }

        static IEnumerable<object[]> Format_Customns_Data()
        {
            yield return new object[] { "{0:|_,2:R}", 0, "_0" };
            yield return new object[] { "{0:|_,2:L}", 0, "0_" };
            yield return new object[] { "{0:D2|_,2:L}", 0, "00" };
            yield return new object[] { "{0:D3|_,2:L}", 0, "00" };
            yield return new object[] { "{0:d|_,4:L}", DateTimeOffset.MaxValue, $"{DateTimeOffset.MaxValue.Year}" };

            yield return new object[] { "{0:hogehoge}", 0, "hogehoge" };
            yield return new object[] { "{0:|_,-1:R}", 0, "0" };
            yield return new object[] { $"{{0:|_,{int.MaxValue}0:R}}", 0, $"|_{int.MaxValue}0:R" };
        }
        #endregion

        #region Format_Serializations
        [Theory]
        [MemberData(nameof(GetFormatSerializations))]
        public void Format_Serializations(string format, object arg, object expected)
        {
            // Arrange
            LogLayoutFormatProvider provider = new();

            // Act
            string actual = string.Format(provider, format, arg);

            // Assert
            Assert.Equal(expected, actual);
        }

        static IEnumerable<object[]> GetFormatSerializations()
        {
            // json
            yield return new object[] { "{0:=>}", 0, "0" };
            yield return new object[] { "{0:=>json}", 0, "0" };
            yield return new object[] { "{0:=>json}", "0", "\"0\"" };
            yield return new object[] { "{0:=>json}", DateTimeOffset.MaxValue, string.Format("\"{0:yyyy-MM-ddTHH:mm:ss.fffffffzzz}\"", DateTimeOffset.MaxValue) };
            yield return new object[] { "{0:D2=>json}", 0, "0" };
            yield return new object[] { "{0:|_,2:R=>json}", 0, "0" };
            yield return new object[] { "{0:=>json|_,2:R}", 0, "" };
            yield return new object[] { "{0:=>json}", TimeSpan.Zero, string.Format("\"{0:d\\.hh\\:mm\\:ss\\.fffffff}\"", TimeSpan.Zero) };
        }
        #endregion

        #region Format_ReturnsEmpty
        [Theory]
        [MemberData(nameof(Format_ReturnsEmpty_Data))]
        public void Format_ReturnsEmpty(string format, object arg, object expected)
        {
            // Arrange
            LogLayoutFormatProvider provider = new();

            // Act
            string actual = string.Format(provider, format, arg);

            // Assert
            Assert.Equal(expected, actual);
        }

        static IEnumerable<object[]> Format_ReturnsEmpty_Data()
        {
            // json
            yield return new object[] { "{0:=>xml}", 0, "" };
            yield return new object[] { "{0:=>json|_,2:L}", 0, "" };
        }
        #endregion

        #region Format_Add_Dictionary
        [Theory]
        [MemberData(nameof(Format_Add_Dictionary_Data))]
        public void Format_Add_Dictionary(Dictionary<string, object> extensions, string format, string expected)
        {
            // Arrange
            LogLayoutFormatProvider provider = new();

            // Act
            string actual = string.Format(provider, format, extensions);
            testOutput.WriteLine(actual);

            // Assert
            Assert.Equal(expected, actual);
        }

        static IEnumerable<object[]> Format_Add_Dictionary_Data()
        {
            Dictionary<string, object?> extensions;

            extensions = new()
            {
                ["key1"] = 0,
                ["key2"] = TimeSpan.MaxValue,
                ["key3"] = DateTimeOffset.MaxValue,
                ["key4"] = null,
            };
            yield return new object[] { extensions, "{0}", $"[key1,0][key2,10675199.02:48:05.4775807][key3,9999-12-31T23:59:59.9999999+00:00][key4,]" };
            yield return new object[] { extensions, "{0:=>}", $"{{\"key1\":0,\"key2\":\"10675199.02:48:05.4775807\",\"key3\":\"9999-12-31T23:59:59.9999999+00:00\",\"key4\":null}}" };

            extensions = new();
            yield return new object[] { extensions, "{0}", $"" };
        }
        #endregion

        #region Format_Add_Event_Data
        [Theory]
        [MemberData(nameof(Format_Add_Event_Data))]
        public void Format_Add_Event(TraceEventArgs e, string format, string expected)
        {
            // Arrange
            LogLayoutFormatProvider provider = new();

            // Act
            string actual = string.Format(provider, format, e);
            testOutput.WriteLine(actual);

            // Assert
            Assert.Equal(expected, actual);
        }

        static IEnumerable<object[]> Format_Add_Event_Data()
        {
            var process = Process.GetCurrentProcess();

            TraceEventArgs traceEvent = new(new(), DateTimeOffset.MaxValue, TraceAction.Trace, 0, x => x.test = 0);
            yield return new object[] { traceEvent, "{0}", $"[action,Trace][traced,9999-12-31T23:59:59.9999999+00:00][threadId,{Environment.CurrentManagedThreadId}][processId,{process.Id}][processName,{process.ProcessName}][machineName,{Environment.MachineName}][message,0][test,0]" };
            yield return new object[] { traceEvent, "{0:=>json}", $"{{\"action\":\"Trace\",\"traced\":\"9999-12-31T23:59:59.9999999+00:00\",\"threadId\":{Environment.CurrentManagedThreadId},\"processId\":{process.Id},\"processName\":\"{process.ProcessName}\",\"machineName\":\"{Environment.MachineName}\",\"message\":0,\"test\":0}}" };

            traceEvent = new(new(), DateTimeOffset.MaxValue, TraceAction.Trace, 0);
            yield return new object[] { traceEvent, "{0:=>json}", $"{{\"action\":\"Trace\",\"traced\":\"9999-12-31T23:59:59.9999999+00:00\",\"threadId\":{Environment.CurrentManagedThreadId},\"processId\":{process.Id},\"processName\":\"{process.ProcessName}\",\"machineName\":\"{Environment.MachineName}\",\"message\":0}}" };
        }
        #endregion
        #endregion
    }
}
