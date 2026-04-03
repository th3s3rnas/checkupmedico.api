namespace CheckupMedico.Application.Service.Interface
{
    using CheckupMedico.Application.Dto.Auth;
    public interface IAuthService
    {
        Task<AuthResponseDto> AuthenticateAsync(AuthRequestDto req);
    }
}
