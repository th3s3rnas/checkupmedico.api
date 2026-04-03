namespace CheckupMedico.Api.Controllers
{
    using CheckupMedico.Api.Controllers.Base;
    using CheckupMedico.Application.Dto.Catalog;
    using CheckupMedico.Application.Service.Interface.Checkup;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
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
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var fullname = User.FindFirst(ClaimTypes.Name)?.Value;
            var birthdate = User.FindFirst(ClaimTypes.DateOfBirth)?.Value;
            var society = User.FindFirst(ClaimTypes.Sid)?.Value;
            var birthDate = DateTime.Parse(birthdate);

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
