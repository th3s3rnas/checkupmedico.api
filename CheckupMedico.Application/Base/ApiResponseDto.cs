namespace CheckupMedico.Application.Dto.Base
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponseDto<T> Ok(T data, string? message = null)
        {
            return new ApiResponseDto<T>
            {
                Success = true,
                Message = message ?? "Operación éxitosa",
                Data = data
            };
        }

        public static ApiResponseDto<T> Fail(List<string> errors, string? message = null)
        {
            return new ApiResponseDto<T>
            {
                Success = false,
                Message = message ?? "Operación fallida",
                Errors = errors
            };
        }
    }
}
