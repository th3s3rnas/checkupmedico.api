namespace CheckupMedico.Application.Dto.Auth
{
    public class AuthRequestDto
    {
        public string IdCalaborador { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Sociedad { get; set; } = null!;
    }
}
