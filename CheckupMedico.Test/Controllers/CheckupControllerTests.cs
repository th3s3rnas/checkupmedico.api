namespace CheckupMedico.Test.Controllers;

using CheckupMedico.Api.Controllers;
using CheckupMedico.Application.Dto.Base;
using CheckupMedico.Application.Dto.Catalog;
using CheckupMedico.Application.Service.Interface.Checkup;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

public class CheckupControllerTests
{
    private static HospitalListDto BuildRequest() => new()
    {
        Location = "MTY",
        Name = "Hospital Uno",
        Campus = "MTY",
        Email = "h@test.com",
        Responsible = "Resp",
        LocationDetails = "Loc",
        Area = "Area",
        ContactDetails = "Contact",
        City = "Monterrey",
        State = "NL",
        Kit = "KIT-01"
    };

    [Fact]
    public void Create_WhenIdentityClaimsAreMissing_ReturnsBadRequest()
    {
        var service = new Mock<ICheckupService>();
        var sut = new CheckupController(service.Object);
        ControllerTestHelper.SetUser(sut);

        var result = sut.Create(BuildRequest());

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var payload = Assert.IsType<ApiResponseDto<string>>(bad.Value);
        Assert.False(payload.Success);
    }

    [Fact]
    public void Create_WhenBirthdateIsInvalid_ReturnsBadRequest()
    {
        var service = new Mock<ICheckupService>();
        var sut = new CheckupController(service.Object);
        ControllerTestHelper.SetUser(sut,
            new Claim(ClaimTypes.NameIdentifier, "A1"),
            new Claim(ClaimTypes.Name, "Jane Doe"),
            new Claim(ClaimTypes.DateOfBirth, "invalid"));

        var result = sut.Create(BuildRequest());

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var payload = Assert.IsType<ApiResponseDto<string>>(bad.Value);
        Assert.False(payload.Success);
    }

    [Fact]
    public void Create_WhenGeneratedFileIsEmpty_ReturnsBadRequest()
    {
        var service = new Mock<ICheckupService>();
        service.Setup(x => x.Create(It.IsAny<HospitalListDto>(), "A1", "Jane Doe", It.IsAny<DateTime>()))
            .Returns(new MemoryStream(Array.Empty<byte>()));

        var sut = new CheckupController(service.Object);
        ControllerTestHelper.SetUser(sut,
            new Claim(ClaimTypes.NameIdentifier, "A1"),
            new Claim(ClaimTypes.Name, "Jane Doe"),
            new Claim(ClaimTypes.DateOfBirth, "1990-01-01"));

        var result = sut.Create(BuildRequest());

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var payload = Assert.IsType<ApiResponseDto<string>>(bad.Value);
        Assert.False(payload.Success);
    }

    [Fact]
    public void Create_WhenGeneratedFileHasContent_ReturnsPdfFile()
    {
        var service = new Mock<ICheckupService>();
        service.Setup(x => x.Create(It.IsAny<HospitalListDto>(), "A1", "Jane Doe", It.IsAny<DateTime>()))
            .Returns(new MemoryStream(new byte[] { 1, 2, 3 }));

        var sut = new CheckupController(service.Object);
        ControllerTestHelper.SetUser(sut,
            new Claim(ClaimTypes.NameIdentifier, "A1"),
            new Claim(ClaimTypes.Name, "Jane Doe"),
            new Claim(ClaimTypes.DateOfBirth, "1990-01-01"));

        var result = sut.Create(BuildRequest());

        var file = Assert.IsType<FileStreamResult>(result);
        Assert.Equal("application/pdf", file.ContentType);
        Assert.Equal("CheckupA1.pdf", file.FileDownloadName);
    }
}
