using System;
using FluentAssertions;
using Molimentum.Areas.Admin.Controllers;
using Molimentum.Areas.Admin.Models.Synchronization;
using Moq;
using Xunit;

namespace Molimentum.Tests.Areas.Admin.Controllers
{
    public class PicasaImportControllerTests
    {
        [Fact]
        public void When_Calling_Import_An_Import_Is_Started()
        {
            var picasaImportMock = new Mock<IPicasaImport>(MockBehavior.Strict);
            picasaImportMock.Setup(p => p.ImportPicasa()).AtMostOnce();

            var controller = new PicasaImportController(picasaImportMock.Object);
            controller.Import().Wait(1000);

            picasaImportMock.VerifyAll();
        }

        [Fact]
        public void When_Calling_Import_More_Than_Once_Only_One_Import_Is_Started()
        {
            bool finishCall = false;

            var picasaImportMock = new Mock<IPicasaImport>(MockBehavior.Strict);
            picasaImportMock.Setup(p => p.ImportPicasa())
                .Callback(() => { while (!finishCall); })
                .AtMostOnce();

            var controller = new PicasaImportController(picasaImportMock.Object);
            var firstCall = controller.Import();

            Action secondCall = () => controller.Import().Wait(1000);
            secondCall.ShouldThrow<AggregateException>().WithInnerException <InvalidOperationException>();

            finishCall = true;
            firstCall.Wait(1000);

            picasaImportMock.VerifyAll();
        }
    }
}
