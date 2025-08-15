using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OpenVmsTextEditor.Domain.Interfaces;
using OpenVmsTextEditor.Domain.Services;
using OpenVmsTextEditor.Web.Controllers.Api;
using Xunit;
using Shouldly;

namespace OpenVmsTextEditor.Tests;

public class ExplorerControllerTests
{
    [Fact]
    public async Task Folder_Returns_Ok_With_Model()
    {
        var logger = Mock.Of<ILogger<ExplorerController>>();
        var io = new Mock<IOperatingSystemIo>(MockBehavior.Strict);
        var pageInfoLogger = Mock.Of<ILogger<PageInfoService>>();
        var pageInfoSvc = new PageInfoService(pageInfoLogger, io.Object);

        io.Setup(x => x.GetDisksAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<string> { "C:" });
        io.Setup(x => x.GetDirectoryFilesAsync(null, null, false, "C:", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OpenVmsTextEditor.Domain.Models.File>());

        var sut = new ExplorerController(logger, io.Object, pageInfoSvc);
        var result = await sut.Folder(null, null, false, "C:", CancellationToken.None) as OkObjectResult;
        result.ShouldNotBeNull();
        result.Value.ShouldNotBeNull();
    }

    [Fact]
    public async Task File_Returns_Ok_With_Content()
    {
        var logger = Mock.Of<ILogger<ExplorerController>>();
        var io = new Mock<IOperatingSystemIo>(MockBehavior.Strict);
        var pageInfoSvc = Mock.Of<IPageInfoService>();

        io.Setup(x => x.GetFileAsync("C:/readme.txt", It.IsAny<CancellationToken>())).ReturnsAsync("hello");

        var sut = new ExplorerController(logger, io.Object, pageInfoSvc);
        var result = await sut.File("C:/readme.txt", CancellationToken.None) as OkObjectResult;
        result.ShouldNotBeNull();
        result.Value.ShouldBe("hello");
    }
}


