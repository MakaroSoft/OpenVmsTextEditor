using Microsoft.Extensions.Logging;
using Moq;
using OpenVmsTextEditor.Domain.Interfaces;
using OpenVmsTextEditor.Domain.Services;
using Xunit;
using Shouldly;

namespace OpenVmsTextEditor.Tests;

public class PageInfoServiceTests
{
    [Fact]
    public async Task GetPageInfoAsync_Returns_Model_With_Disks_Files_And_Breadcrumb()
    {
        // Arrange
        var logger = Mock.Of<ILogger<PageInfoService>>();
        var io = new Mock<IOperatingSystemIo>(MockBehavior.Strict);
        var svc = new PageInfoService(logger, io.Object);

        var disks = new List<string> { "C:", "D:" };
        var files = new List<OpenVmsTextEditor.Domain.Models.File>
        {
            new OpenVmsTextEditor.Domain.Models.File { Name = "Folder", Dir = true },
            new OpenVmsTextEditor.Domain.Models.File { Name = "Readme.txt" }
        };

        io.Setup(x => x.GetDisksAsync(It.IsAny<CancellationToken>())).ReturnsAsync(disks);
        io.Setup(x => x.GetDirectoryFilesAsync(null, null, false, "C:", It.IsAny<CancellationToken>())).ReturnsAsync(files);

        // Act
        var model = await svc.GetPageInfoAsync(null, null, false, "C:", CancellationToken.None);

        // Assert
        model.ShouldNotBeNull();
        model.Disks.ShouldBe(disks);
        model.Files.ShouldBe(files);
        model.BreadCrumb.ShouldBe(["C:"]);
    }
}


