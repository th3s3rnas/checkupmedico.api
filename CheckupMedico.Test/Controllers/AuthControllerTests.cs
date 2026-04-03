namespace CheckupMedico.Test.Controllers;

using CheckupMedico.Api.Controllers;
using CheckupMedico.Application.Dto.Auth;
using CheckupMedico.Application.Dto.Base;
using CheckupMedico.Application.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

public class AuthControllerTests
{
    [Fact]
    public async Task GenerateToken_WhenServiceReturnsToken_ReturnsOk()
    {
        var authService = new Mock<IAuthService>();
        authService
            .Setup(x => x.AuthenticateAsync(It.IsAny<AuthRequestDto>()))
            .ReturnsAsync(new AuthResponseDto
            {
                Token = "jwt-token",
                Expiration = DateTime.UtcNow.AddMinutes(30)
            });

        var sut = new AuthController(authService.Object);

        var result = await sut.GenerateToken(new AuthRequestDto
        {
            IdCalaborador = "A1",
            Email = "a@test.com",
            Sociedad = "0010"
        });

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<ApiResponseDto<AuthResponseDto>>(ok.Value);
        Assert.True(payload.Success);
        Assert.Equal("jwt-token", payload.Data!.Token);
    }

    [Fact]
    public async Task GenerateToken_WhenServiceReturnsNull_ReturnsUnauthorized()
    {
        var authService = new Mock<IAuthService>();
        authService
            .Setup(x => x.AuthenticateAsync(It.IsAny<AuthRequestDto>()))
            .ReturnsAsync((AuthResponseDto)null!);

        var sut = new AuthController(authService.Object);

        var result = await sut.GenerateToken(new AuthRequestDto
        {
            IdCalaborador = "A1",
            Email = "a@test.com",
            Sociedad = "0010"
        });

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void GetCampus_WhenNameClaimExists_ReturnsUserName()
    {
        var authService = new Mock<IAuthService>();
        var sut = new AuthController(authService.Object);
        ControllerTestHelper.SetUser(sut, new Claim(ClaimTypes.Name, "Jane Doe"));

        var result = sut.GetCampus();

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<ApiResponseDto<UserResponseDto>>(ok.Value);
        Assert.Equal("Jane Doe", payload.Data!.FullName);
    }
}
