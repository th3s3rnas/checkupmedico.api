namespace CheckupMedico.Domain.External.Model.Apigateway
{
    public class ResponseDto<T>
    {
        public ResponseDto()
        {
        }
        public ResponseDto(T data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }
        public ResponseDto(string message)
        {
            Succeeded = false;
            Message = message;
        }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
