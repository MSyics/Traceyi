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
    public class LogLayoutConverterTests
    {
        readonly ITestOutputHelper testOutput;

        public LogLayoutConverterTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        /****************************************************************
         * Convert
         ****************************************************************/
        #region Convert
        [Theory]
        [MemberData(nameof(Convert_ReturnsDefault_Data))]
        public void Convert_ReturnsDefault(string format, string expected)
        {
            // Arrange
            var converter = new LogLayoutConverter(
                new LogLayoutPart { Name = "tab", CanFormat = false },
                new LogLayoutPart { Name = "newLine", CanFormat = false },
                new LogLayoutPart { Name = "action", CanFormat = true },
                new LogLayoutPart { Name = "traced", CanFormat = true },
                new LogLayoutPart { Name = "elapsed", CanFormat = true },
                new LogLayoutPart { Name = "activityId", CanFormat = true },
                new LogLayoutPart { Name = "scopeLabel", CanFormat = true },
                new LogLayoutPart { Name = "scopeId", CanFormat = true },
                new LogLayoutPart { Name = "scopeParentId", CanFormat = true },
                new LogLayoutPart { Name = "scopeDepth", CanFormat = true },
                new LogLayoutPart { Name = "threadId", CanFormat = true },
                new LogLayoutPart { Name = "processId", CanFormat = true },
                new LogLayoutPart { Name = "processName", CanFormat = true },
                new LogLayoutPart { Name = "machineName", CanFormat = true },
                new LogLayoutPart { Name = "message", CanFormat = true },
                new LogLayoutPart { Name = "extensions", CanFormat = true },
                new LogLayoutPart { Name = "@", CanFormat = true });

            // Act
            var actual = converter.Convert(format);
            testOutput.WriteLine(actual);

            // Assert
            Assert.Equal(expected, actual);
        }

        static IEnumerable<object[]> Convert_ReturnsDefault_Data()
        {
            yield return new object[] { LogLayout.DefaultFormat, @"{2}{0}{3:yyyy-MM-ddTHH:mm:ss.fffffffzzz}{0}{4:d\.hh\:mm\:ss\.fffffff}{0}{7:|-,16:R}{0}{8:|-,16:R}{0}{9}{0}{6}{0}{5}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15:=>json}" };
            yield return new object[] { "", @"" };
            yield return new object[] { "{0}", @"{{0}}" };
            yield return new object[] { "{0", @"{{0" };
            yield return new object[] { "0}", @"0}}" };
            yield return new object[] { "{tab}", "{0}" };
            yield return new object[] { "{@}", "{16}" };
            yield return new object[] { "{tab|_,4:R}", "{0}" };
        }

        [Theory]
        [MemberData(nameof(Convert_CanFormat_Data))]
        public void Convert_CanFormat(string format, string expected)
        {
            // Arrange
            var converter = new LogLayoutConverter(
                new LogLayoutPart { Name = "can_not_foramt", CanFormat = false },
                new LogLayoutPart { Name = "can_foramt", CanFormat = true });

            // Act
            var actual = converter.Convert(format);
            testOutput.WriteLine(actual);

            // Assert
            Assert.Equal(expected, actual);
        }

        static IEnumerable<object[]> Convert_CanFormat_Data()
        {
            yield return new object[] { "{can_not_foramt|_,4:R}", "{0}" };
            yield return new object[] { "{can_foramt|_,0:R}", "{1:|_,0:R}" };
            yield return new object[] { "{can_foramt=>json}", "{1:=>json}" };
        }
        #endregion

        /****************************************************************
         * IsPartPlaced
         ****************************************************************/
        #region IsPartPlaced
        [Theory]
        [MemberData(nameof(IsPartPlaced_ReturnsTrue_Data))]
        public void IsPartPlaced_ReturnsTrue(string name, string placed, bool expected)
        {
            // Arrange
            var converter = new LogLayoutConverter(new LogLayoutPart { Name = name, CanFormat = true });
            converter.Convert($"{{{name}}}");

            // Act
            var actual = converter.IsPartPlaced(placed);

            // Assert
            Assert.Equal(expected, actual);
        }

        static IEnumerable<object[]> IsPartPlaced_ReturnsTrue_Data()
        {
            yield return new object[] { "hoge", "hoge", true };
            yield return new object[] { "hoge", "piyo", false };
        }
        #endregion
    }
}
