namespace CheckupMedico.Api.Middlewares
{
    using CheckupMedico.Application.Dto.Base;
    using CheckupMedico.Transversal.Exception;
    using System.Net;
    using System.Text.Json;

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            HttpStatusCode statusCode;
            ApiResponseDto<string> response;

            switch (exception)            {
                case Transversal.Exception.ValidationException validationEx:
                    statusCode = HttpStatusCode.BadRequest;
                    response = ApiResponseDto<string>.Fail(
                        validationEx.Errors,
                        validationEx.Message);
                    break;

                case NotFoundException notFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    response = ApiResponseDto<string>.Fail(
                        new List<string> { notFoundEx.Message },
                        "Recurso no encontrado.");
                    break;

                case UnauthorizedException unauthorized:
                    statusCode = HttpStatusCode.Unauthorized;
                    response = ApiResponseDto<string>.Fail(
                        new List<string> { unauthorized.Message },
                        "Sin autorización.");
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    response = ApiResponseDto<string>.Fail(
                        new List<string> { "Error interno del servidor." });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
