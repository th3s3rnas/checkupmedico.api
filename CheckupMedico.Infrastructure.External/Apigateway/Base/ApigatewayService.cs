namespace CheckupMedico.Infrastructure.External.Apigateway.Base
{
    using CheckupMedico.Infrastructure.External.Apigateway.Models;
    using CheckupMedico.Infrastructure.External.Model.Apigateway;
    using CheckupMedico.Transversal.Exception;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public abstract class ApigatewayService<T>
    {
        protected ApigatewayStructure _apigatewayStructure = new ApigatewayStructure();
        protected ResponseMessagesHelper<T> _responseMessagesHelper;
        protected IHttpClientFactory _httpClientFactory;
        protected readonly ILogger<T> _logger;

        public ApigatewayService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<T> logger)
        {
            _httpClientFactory = httpClientFactory;
            _apigatewayStructure = configuration.GetSection("ApigatewayStructure").Get<ApigatewayStructure>() ?? new ApigatewayStructure();
            _responseMessagesHelper = new ResponseMessagesHelper<T>(logger);
            _logger = logger;
        }

        public async Task<ResponseDto<TokenResponse>> GetTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            WebService wsToken = _apigatewayStructure.WebServices.Where(x => x.Name == "Token").FirstOrDefault();
            client.BaseAddress = new Uri(wsToken.Server);
            Dictionary<string, string> contentList = new Dictionary<string, string>();
            foreach (Attributes atr in wsToken.Body)
                contentList.Add(atr.Attribute, atr.Value);

            client.DefaultRequestHeaders.Add(wsToken.Headers[0].Attribute, wsToken.Headers[0].Value);
            client.DefaultRequestHeaders.TryAddWithoutValidation(wsToken.Headers[1].Attribute, wsToken.Headers[1].Value);
            var httpResponse = await client.PostAsync(wsToken.URL, new FormUrlEncodedContent(contentList));
            string response = await httpResponse.Content.ReadAsStringAsync();

            // Validar la respuesta del web service
            ResponseDto<string> validateResponse = _responseMessagesHelper.ValidateResponseWebService(httpResponse, response);
            if (!validateResponse.Succeeded)
                throw new UnauthorizedException(validateResponse.Message);

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(response);
            return new ResponseDto<TokenResponse>(token, validateResponse.Message);
        }

        public async Task<ResponseDto<TokenJWTResponse>> GetTokenJWTAsync(string token, UserRequestDto user)
        {
            var client = _httpClientFactory.CreateClient();
            WebService wsTokenJWT = _apigatewayStructure.WebServices.Where(x => x.Name == "TokenJWT").FirstOrDefault();
            client.BaseAddress = new Uri(wsTokenJWT.Server);
            client.DefaultRequestHeaders.Add(wsTokenJWT.Headers[0].Attribute, wsTokenJWT.Headers[0].Value);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add(wsTokenJWT.Body[0].Attribute, user.payroll);
            client.DefaultRequestHeaders.Add(wsTokenJWT.Body[1].Attribute, user.email);
            client.DefaultRequestHeaders.Add(wsTokenJWT.Body[2].Attribute, "Colaborador");
            var httpResponse = await client.PostAsync(wsTokenJWT.URL, null);
            var response = await httpResponse.Content.ReadAsStringAsync();
            // Validar la respuesta del web service
            ResponseDto<string> validateResponse = _responseMessagesHelper.ValidateResponseWebService(httpResponse, response);
            if (!validateResponse.Succeeded)
                throw new UnauthorizedException(validateResponse.Message);

            TokenJWTResponse tokenRequest = JsonConvert.DeserializeObject<TokenJWTResponse>(response);
            return new ResponseDto<TokenJWTResponse>(tokenRequest, validateResponse.Message);
        }
    }
}
