using System;
using FluentAssertions;
using Xunit;
using Xunit.Extensions;

namespace Molimentum.Tests
{
    public class UtilsTests
    {
        [Theory]
        [InlineData("The quick.", 10, "The quick.")]
        [InlineData("The quick brown fox jumped over something I can't remember.", 10, "The quick...")]
        [InlineData("Thequickbrownfoxjumped over something I can't remember.", 10, "Thequickbr...")]
        public void GenerateSummaryFromStringTest(string text, int summaryLength, string expectedSummary)
        {
            var summary = text.GenerateSummaryFromString(summaryLength);
            summary.Should().Be(expectedSummary);
        }

        [Theory]
        [InlineData("<p>The quick.</p>", 10, "The quick.")]
        [InlineData("<p>The <b>quick</b> brown fox jumped over something I can't remember.", 10, "The quick...")]
        [InlineData("<p>Thequick<i>brownfoxjumped over something</i> I can't remember.</p>", 10, "Thequickbr...")]
        public void GenerateSummaryFromHtmlStringTest(string htmlText, int summaryLength, string expectedSummary)
        {
            var summary = htmlText.GenerateSummaryFromHtmlString(summaryLength);
            summary.Should().Be(expectedSummary);
        }

        [Fact]
        public void When_Exception_That_Should_Be_Ignored_Is_Thrown_It_Is_Ignored()
        {
            Action actionThrowingException = () => { throw new TestException(); };

            Action actionIgnoringException = () => actionThrowingException.InvokeAndIgnore<TestException>();

            actionIgnoringException.ShouldNotThrow<TestException>();
        }

        [Fact]
        public void When_Exception_That_Should_Not_Be_Ignored_Is_Thrown_It_Is_Not_Ignored()
        {
            Action actionThrowingException = () => { throw new TestException(); };

            Action actionIgnoringException = () => actionThrowingException.InvokeAndIgnore<NullReferenceException>();

            actionIgnoringException.ShouldThrow<TestException>();
        }
    }
}
