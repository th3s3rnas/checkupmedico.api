namespace CheckupMedico.Test.Controllers;

using CheckupMedico.Api.Controllers;
using CheckupMedico.Application.Dto.Base;
using CheckupMedico.Application.Dto.Catalog;
using CheckupMedico.Application.Service.Interface.Catalog;
using CheckupMedico.Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

public class CatalogControllerTests
{
    [Fact]
    public void GetCampus_ReturnsOkWithData()
    {
        var service = new Mock<ICatalogService>();
        service.Setup(x => x.GetCampus()).Returns(new List<CampusListDto>
        {
            new() { Code = "MTY", Name = "MTY" }
        });

        var sut = new CatalogController(service.Object);

        var result = sut.GetCampus();

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<ApiResponseDto<List<CampusListDto>>>(ok.Value);
        Assert.True(payload.Success);
        Assert.Single(payload.Data!);
    }

    [Fact]
    public void GetHospitalsByCampus_WhenBirthdateClaimIsInvalid_ReturnsBadRequest()
    {
        var service = new Mock<ICatalogService>();
        var sut = new CatalogController(service.Object);
        ControllerTestHelper.SetUser(sut,
            new Claim(ClaimTypes.DateOfBirth, "invalid-date"),
            new Claim(ClaimTypes.Gender, "1"));

        var result = sut.GetHospitalsByCampus(new HospitalsReqDto { CampusName = "MTY" });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var payload = Assert.IsType<ApiResponseDto<string>>(bad.Value);
        Assert.False(payload.Success);
    }

    [Fact]
    public void GetHospitalsByCampus_WhenGenderClaimIsInvalid_ReturnsBadRequest()
    {
        var service = new Mock<ICatalogService>();
        var sut = new CatalogController(service.Object);
        ControllerTestHelper.SetUser(sut,
            new Claim(ClaimTypes.DateOfBirth, "1990-01-01"),
            new Claim(ClaimTypes.Gender, "99"));

        var result = sut.GetHospitalsByCampus(new HospitalsReqDto { CampusName = "MTY" });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var payload = Assert.IsType<ApiResponseDto<string>>(bad.Value);
        Assert.False(payload.Success);
    }

    [Fact]
    public void GetHospitalsByCampus_WhenClaimsAreValid_ReturnsOkAndCallsService()
    {
        var service = new Mock<ICatalogService>();

        service.Setup(x => x.GetHospitalsByCampus(
                It.Is<HospitalsReqDto>(r => r.CampusName == "MTY" && r.Gender == SexEnum.Hombre),
                It.IsAny<DateTime>()))
            .Returns(new List<HospitalListDto>
            {
                new() { Name = "Hospital Uno", Kit = "KIT-01", Location = "MTY", Campus = "MTY", Email = "h@test", Responsible = "R", LocationDetails = "L", Area = "A", ContactDetails = "C", City = "Monterrey", State = "NL" }
            });

        var sut = new CatalogController(service.Object);
        ControllerTestHelper.SetUser(sut,
            new Claim(ClaimTypes.DateOfBirth, "1990-01-01"),
            new Claim(ClaimTypes.Gender, "1"));

        var result = sut.GetHospitalsByCampus(new HospitalsReqDto { CampusName = "MTY" });

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<ApiResponseDto<List<HospitalListDto>>>(ok.Value);
        Assert.True(payload.Success);
        Assert.Single(payload.Data!);
        service.VerifyAll();
    }
}
