namespace CheckupMedico.Api.Controllers.Base
{
    using CheckupMedico.Application.Dto.Base;
    using Microsoft.AspNetCore.Mvc;
    public class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string? message = null) => Ok(ApiResponseDto<T>.Ok(data, message));

        protected IActionResult Failure(List<string> errors, string? message = null) => BadRequest(ApiResponseDto<string>.Fail(errors, message));
    }
}
