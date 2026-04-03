namespace CheckupMedico.Application.Service.Interface
{
    using CheckupMedico.Application.Dto.Auth;
    public interface IAuthService
    {
        AuthResponseDto Authenticate(AuthRequestDto req);
    }
}
