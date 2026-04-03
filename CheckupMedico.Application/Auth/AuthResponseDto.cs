namespace CheckupMedico.Application.Dto.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = default!;
        public DateTime Expiration { get; set; }
    }
}
