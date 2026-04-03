namespace CheckupMedico.Api.Controllers
{
    using CheckupMedico.Api.Controllers.Base;
    using CheckupMedico.Application.Dto.Auth;
    using CheckupMedico.Application.Dto.Base;
    using CheckupMedico.Application.Dto.Catalog;
    using CheckupMedico.Application.Service.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("token")]
        [ProducesResponseType(typeof(ApiResponseDto<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<string>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GenerateToken([FromBody] AuthRequestDto request)
        {
            var token = await _authService.AuthenticateAsync(request);

            if (token == null)
                return Unauthorized();

            return Success(token);
        }

        [HttpGet("user")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetCampus()
        {
            var fullname = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            return Success(new UserResponseDto() { FullName = fullname });
        }
    }
}
