namespace CheckupMedico.Api.Controllers
{
    using CheckupMedico.Api.Controllers.Base;
    using CheckupMedico.Application.Dto.Catalog;
    using CheckupMedico.Application.Service.Interface.Checkup;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Globalization;
    using System.Security.Claims;

    [ApiController]
    [Route("api/checkup")]
    [Authorize]
    public class CheckupController : BaseController
    {
        private readonly ICheckupService _service;
        public CheckupController(ICheckupService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        [Produces("application/pdf")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Create([FromBody] HospitalListDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var fullname = User.FindFirst(ClaimTypes.Name)?.Value;
            var birthdate = User.FindFirst(ClaimTypes.DateOfBirth)?.Value;

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(fullname))
                return Failure(["Required identity claims are missing."], "Invalid user context.");

            if (!DateTime.TryParse(birthdate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthDate))
                return Failure(["Date of birth claim is missing or invalid."], "Invalid user context.");

            var fileBytes = _service.Create(request, userId, fullname, birthDate);

            if (fileBytes == null || fileBytes.Length == 0)
                return Failure([], "File could not be generated.");

            return File(
                fileBytes,
                "application/pdf",
                $"Checkup{userId}.pdf"
            );
        }
    }
}
