namespace CheckupMedico.Api.Controllers
{
    using CheckupMedico.Api.Controllers.Base;
    using CheckupMedico.Application.Dto.Catalog;
    using CheckupMedico.Application.Service.Interface.Catalog;
    using CheckupMedico.Domain.Enum;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Globalization;
    using System.Security.Claims;

    [ApiController]
    [Route("api/catalog")]
    [Authorize]
    public class CatalogController : BaseController
    {
        private readonly ICatalogService _serviceCatalog;
        public CatalogController(ICatalogService serviceCatalog)
        {
            _serviceCatalog = serviceCatalog;
        }


        [HttpGet("hospitals")]
        [ProducesResponseType(typeof(List<CampusListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetCampus()
        {
            var data = _serviceCatalog.GetCampus();
            return Success(data);
        }

        [HttpPost("hospitals/campus")]
        [ProducesResponseType(typeof(List<HospitalListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetHospitalsByCampus([FromBody] HospitalsReqDto request)
        {
            var birthdate = User.FindFirst(ClaimTypes.DateOfBirth)?.Value;
            var gender = User.FindFirst(ClaimTypes.Gender)?.Value;

            if (!DateTime.TryParse(birthdate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthDate))
                return Failure(["Date of birth claim is missing or invalid."], "Invalid user context.");

            if (!short.TryParse(gender, out var genderType) || !Enum.IsDefined(typeof(SexEnum), (int)genderType))
                return Failure(["Gender claim is missing or invalid."], "Invalid user context.");

            request.Gender = (SexEnum)genderType;

            var data = _serviceCatalog.GetHospitalsByCampus(request, birthDate);
            return Success(data);
        }
    }
}
