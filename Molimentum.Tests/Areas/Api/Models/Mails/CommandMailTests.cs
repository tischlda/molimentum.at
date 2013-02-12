using FluentAssertions;
using Molimentum.Areas.Api.Models.Mails;
using Xunit;

namespace Molimentum.Tests.Areas.Api.Models.Mails
{
    public class CommandMailTests
    {
        [Fact]
        public void ACommandMailCanBeParsedFromAMail()
        {
            var mail = new Mail
            {
                From = "me@here.com",
                Subject = "TestSubject",
                Plain =
@"KEY1: TestKey1
Key2: TestKey2

TestBody"
            };

            var commandMail = CommandMail.Parse(mail);

            commandMail.From.Should().Be("me@here.com");
            commandMail.Subject.Should().Be("TestSubject");
            commandMail.Fields.Should().Contain("KEY1", "TestKey1");
            commandMail.Fields.Should().Contain("KEY2", "TestKey2");
            commandMail.Fields.Should().Contain("BODY", "TestBody");
            commandMail.Fields.Keys.Count.Should().Be(3);
            commandMail.Body.Should().Be("TestBody");
        }
    }
}
