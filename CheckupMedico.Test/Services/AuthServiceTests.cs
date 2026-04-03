namespace CheckupMedico.Test.Services;

using CheckupMedico.Application.Dto.Auth;
using CheckupMedico.Application.Service;
using CheckupMedico.Domain.Enum;
using CheckupMedico.Infrastructure.External.Interface.Apigateway.Colaborador;
using CheckupMedico.Infrastructure.External.Model.Apigateway;
using Microsoft.Extensions.Configuration;
using Moq;

public class AuthServiceTests
{
    private static IConfiguration BuildConfiguration()
    {
        var configValues = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "this_is_a_long_key_for_unit_test_only_1234567890",
            ["Jwt:Issuer"] = "CheckupIssuer",
            ["Jwt:Audience"] = "CheckupAudience",
            ["Jwt:ExpirationMinutes"] = "60"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();
    }

    [Fact]
    public async Task AuthenticateAsync_WhenRequestIsValid_ReturnsToken()
    {
        var collaboratorService = new Mock<IColaboradorService>();
        collaboratorService
            .Setup(x => x.GetEmployeeInformationAsync("A123", "user@test.com", "0010"))
            .ReturnsAsync(new ResponseDto<UserProfileResponseDto>(new UserProfileResponseDto
            {
                PayrollID = "A123",
                Email = "user@test.com",
                FullName = "Jane Doe",
                Birthdate = new DateTime(1990, 1, 1),
                SocietyKey = "0010",
                Gender = SexEnum.Mujer
            }));

        var sut = new AuthService(BuildConfiguration(), collaboratorService.Object);

        var response = await sut.AuthenticateAsync(new AuthRequestDto
        {
            IdCalaborador = "A123",
            Email = "user@test.com",
            Sociedad = "0010"
        });

        Assert.False(string.IsNullOrWhiteSpace(response.Token));
        Assert.True(response.Expiration > DateTime.UtcNow);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenIdIsMissing_ThrowsValidationException()
    {
        var collaboratorService = new Mock<IColaboradorService>();
        var sut = new AuthService(BuildConfiguration(), collaboratorService.Object);

        var exception = await Assert.ThrowsAsync<CheckupMedico.Transversal.Exception.ValidationException>(
            () => sut.AuthenticateAsync(new AuthRequestDto
            {
                IdCalaborador = string.Empty,
                Email = "user@test.com",
                Sociedad = "0010"
            }));

        Assert.Contains("IdCalaborador es requerido", exception.Errors);
    }

    [Fact]
    public async Task AuthenticateAsync_WhenProfileIsNull_ThrowsValidationException()
    {
        var collaboratorService = new Mock<IColaboradorService>();
        collaboratorService
            .Setup(x => x.GetEmployeeInformationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((ResponseDto<UserProfileResponseDto>)null!);

        var sut = new AuthService(BuildConfiguration(), collaboratorService.Object);

        await Assert.ThrowsAsync<CheckupMedico.Transversal.Exception.ValidationException>(
            () => sut.AuthenticateAsync(new AuthRequestDto
            {
                IdCalaborador = "A123",
                Email = "user@test.com",
                Sociedad = "0010"
            }));
    }
}
