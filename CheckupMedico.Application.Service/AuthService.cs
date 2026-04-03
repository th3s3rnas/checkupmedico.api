namespace CheckupMedico.Application.Service
{
    using CheckupMedico.Application.Dto.Auth;
    using CheckupMedico.Application.Service.Interface;
    using CheckupMedico.Infrastructure.External.Interface.Apigateway.Colaborador;
    using CheckupMedico.Infrastructure.External.Model.Apigateway;
    using CheckupMedico.Transversal.Exception;
    using CheckupMedico.Transversal.Util;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IColaboradorService _colaboradorService;

        public AuthService(IConfiguration config, IColaboradorService colaboradorService)
        {
            _config = config;
            _colaboradorService = colaboradorService;
        }

        public AuthResponseDto Authenticate(AuthRequestDto req)
        {
            var result = new AuthResponseDto();
            if (string.IsNullOrWhiteSpace(req.IdCalaborador))
                throw new ValidationException(new List<string> { "IdCalaborador es requerido" });

            if (string.IsNullOrWhiteSpace(req.Email))
                throw new ValidationException(new List<string> { "Email es requerido" });

            if (string.IsNullOrWhiteSpace(req.Sociedad))
                throw new ValidationException(new List<string> { "Sociedad es requerida" });

            var dataProfile = _colaboradorService.GetEmployeeInformation(req.IdCalaborador, req.Email, req.Sociedad);

            if (dataProfile != null)
                result = GenerateJWT(dataProfile.Data);
            else
                throw new ValidationException(new List<string>
                {
                    $"No se encontró información del colaborador con IdCalaborador '{req.IdCalaborador}' en la sociedad '{req.Sociedad}' con el correo '{req.Email}'."
                });

            return result;
        }

        private AuthResponseDto GenerateJWT(UserProfileResponseDto req)
        {
            if (string.IsNullOrWhiteSpace(req.PayrollID))
                throw new ValidationException(new List<string> { "PayrollID es requerido" });

            if (string.IsNullOrWhiteSpace(req.Email))
                throw new ValidationException(new List<string> { "Email es requerido" });

            if (string.IsNullOrWhiteSpace(req.Birthdate.ToString()))
                throw new ValidationException(new List<string> { "Birthdate es requerida" });

            if (string.IsNullOrWhiteSpace(req.FullName))
                throw new ValidationException(new List<string> { "FullName es requerido" });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, req.PayrollID),
                new Claim(ClaimTypes.Email, req.Email),
                new Claim(ClaimTypes.DateOfBirth, req.Birthdate.ToString()),
                new Claim(ClaimTypes.Name, req.FullName),
                new Claim(ClaimTypes.Sid, req.SocietyKey),
                new Claim(ClaimTypes.Gender, ((int)req.Gender).ToString()),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var expiration = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpirationMinutes"]!));
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
            };
        }
    }
}
