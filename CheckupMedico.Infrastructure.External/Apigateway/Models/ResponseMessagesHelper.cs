namespace CheckupMedico.Infrastructure.External.Apigateway.Models
{
    using CheckupMedico.Infrastructure.External.Model.Apigateway;
    using Microsoft.Extensions.Logging;
    public class ResponseMessagesHelper<T>
    {
        private readonly ILogger<T> _logger;

        public ResponseMessagesHelper(ILogger<T> logger)
        {
            _logger = logger;
        }

        public ResponseDto<string> ValidateResponseWebService(HttpResponseMessage httpResponse, string response)
        {
            ResponseDto<string> responseObj = new ResponseDto<string>();

            try
            {
                switch (httpResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        responseObj = new ResponseDto<string>() { Succeeded = true };
                        break;
                    case System.Net.HttpStatusCode.Created:
                        responseObj = new ResponseDto<string>() { Succeeded = true };
                        break;
                    case System.Net.HttpStatusCode.NoContent:
                        responseObj = new ResponseDto<string>("ResponseMessagesHelper.CommunicationError");
                        break;
                    case System.Net.HttpStatusCode.Unauthorized:
                        responseObj = new ResponseDto<string>("ResponseMessagesHelper.CommunicationError");
                        break;
                    case System.Net.HttpStatusCode.NotFound:
                        responseObj = new ResponseDto<string>("ResponseMessagesHelper.CommunicationError");
                        break;
                    case System.Net.HttpStatusCode.BadRequest:
                        responseObj = new ResponseDto<string>("ResponseMessagesHelper.CommunicationError");
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        responseObj = new ResponseDto<string>("ResponseMessagesHelper.CommunicationError");
                        break;
                    default:
                        responseObj = new ResponseDto<string>("ResponseMessagesHelper.CommunicationError");
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ApigatewayService.ResponseMessagesHelper.ValidateResponseWebService");
            }

            return responseObj;
        }


    }
}
