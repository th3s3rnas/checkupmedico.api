namespace CheckupMedico.Infrastructure.External.Apigateway.Colaborador
{
    using CheckupMedico.Infrastructure.External.Apigateway.Base;
    using CheckupMedico.Infrastructure.External.Interface.Apigateway.Colaborador;
    using CheckupMedico.Infrastructure.External.Model.Apigateway;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System.Net.Http;

    public class ColaboradorService : ApigatewayService<ColaboradorService>, IColaboradorService
    {

        public ColaboradorService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<ColaboradorService> logger) : base(configuration, httpClientFactory, logger)
        {
        }

        public async Task<ResponseDto<UserProfileResponseDto>> GetEmployeeInformationAsync(string payroll, string email, string societyKey)
        {
            var client = _httpClientFactory.CreateClient();
            WebService wsToken = _apigatewayStructure.WebServices.Where(x => x.Name == "GetEmployeeInformation").FirstOrDefault();
            client.BaseAddress = new Uri(wsToken.Server);

            var token = await GetTokenAsync();
            var tokenJWT = await GetTokenJWTAsync(token.Data.access_token, new UserRequestDto() { email = email, payroll = payroll });

            string url = wsToken.URL.Replace("{payroll}", Uri.EscapeDataString(payroll));

            // Preparar headers de configuración + adicionales
            foreach (var header in wsToken.Headers)
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Attribute, header.Value);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.api+json"));

            if (!string.IsNullOrEmpty(token.Data.access_token))
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Data.access_token);

            if (!string.IsNullOrEmpty(tokenJWT.Data.meta.token))
                client.DefaultRequestHeaders.TryAddWithoutValidation("X-Auth-JWT", tokenJWT.Data.meta.token);

            // Crear request GET
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Enviar request
            var httpResponse = await client.SendAsync(request);
            string responseContent = await httpResponse.Content.ReadAsStringAsync();

            // Validar respuesta
            var validateResponse = _responseMessagesHelper.ValidateResponseWebService(httpResponse, responseContent);
            if (!validateResponse.Succeeded)
                return new ResponseDto<UserProfileResponseDto>(null, validateResponse.Message);

            // Mapear respuesta a DTO
            APIContractsWSResponse employeeInformation = JsonConvert.DeserializeObject<APIContractsWSResponse>(responseContent);
            var dtoResponse = GetDto(employeeInformation, societyKey);
            dtoResponse.Email = email;
            dtoResponse.PayrollID = payroll;
            dtoResponse.PersonID = payroll;
            dtoResponse.SocietyKey = societyKey;
            dtoResponse.TokenJWT = tokenJWT.Data.meta.token;

            return new ResponseDto<UserProfileResponseDto>(dtoResponse, validateResponse.Message);
        }

        private UserProfileResponseDto GetDto(APIContractsWSResponse EmployeeInformation, string societyKey)
        {
            var result = new UserProfileResponseDto();
            Datum dataContract;

            // Si el colaborador tiene más de un contrato, obtiene el nodo del contrato que corresponde a la sociedad con la
            // que esta ingresando al servicio.
            if (EmployeeInformation.data.Count() > 1)
            {
                //string societyKey = isTecmilenio ? "0030" : "0010";
                dataContract = EmployeeInformation.data.Where(e => e.relationships.sociedadcontratante.id.Equals(societyKey, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            }
            else
                dataContract = EmployeeInformation.data.FirstOrDefault();

            result = new UserProfileResponseDto
            {
                ActiveContract = dataContract.attributes.contrato,
                FirstName = dataContract.attributes.nombre,
                LastName = dataContract.attributes.apellidoPaterno,
                LastName2 = dataContract.attributes.apellidoMaterno,
                FullName = string.Format("{0} {1} {2}", dataContract.attributes.nombre, dataContract.attributes.apellidoPaterno, dataContract.attributes.apellidoMaterno),
                Society = new KeyValueString()
                {
                    Key = EmployeeInformation.included.Where(x => x.type.Equals("sociedades", StringComparison.CurrentCultureIgnoreCase)
                                                               && x.id.Equals(dataContract.relationships.sociedadcontratante.id, StringComparison.CurrentCultureIgnoreCase))
                                                      .Select(s => s.id).FirstOrDefault(),
                    Value = EmployeeInformation.included.Where(x => x.type.Equals("sociedades", StringComparison.CurrentCultureIgnoreCase)
                                                                 && x.id.Equals(dataContract.relationships.sociedadcontratante.id, StringComparison.CurrentCultureIgnoreCase))
                                                      .Select(s => s.attributes.descripcion).FirstOrDefault()
                },
                Division = new KeyValueString()
                {
                    Key = EmployeeInformation.included.Where(x => x.type.Equals("divisiones-personal", StringComparison.CurrentCultureIgnoreCase)
                                                               && x.id.Equals(dataContract.relationships.divisioncontratante.id, StringComparison.CurrentCultureIgnoreCase))
                                                      .Select(s => s.id).FirstOrDefault(),
                    Value = EmployeeInformation.included.Where(x => x.type.Equals("divisiones-personal", StringComparison.CurrentCultureIgnoreCase)
                                                                 && x.id.Equals(dataContract.relationships.divisioncontratante.id, StringComparison.CurrentCultureIgnoreCase))
                                                        .Select(s => s.attributes.descripcion).FirstOrDefault()
                },
                Subdivision = new KeyValueString()
                {
                    Key = EmployeeInformation.included.Where(x => x.type.Equals("sub-divisiones-personal", StringComparison.CurrentCultureIgnoreCase)
                                                               && x.id.Equals(dataContract.relationships.subdivisioncontratante.id, StringComparison.CurrentCultureIgnoreCase))
                                                      .Select(s => s.id).FirstOrDefault(),
                    Value = EmployeeInformation.included.Where(x => x.type.Equals("sub-divisiones-personal", StringComparison.CurrentCultureIgnoreCase)
                                                                 && x.id.Equals(dataContract.relationships.subdivisioncontratante.id, StringComparison.CurrentCultureIgnoreCase))
                                                        .Select(s => s.attributes.descripcion).FirstOrDefault()
                },
                PersonalAreaKey = EmployeeInformation.included.Where(x => x.type.Equals("areas-personal", StringComparison.CurrentCultureIgnoreCase)
                                                                       && x.id.Equals(dataContract.relationships.areapersonalasignada.id, StringComparison.CurrentCultureIgnoreCase))
                                                              .Select(s => s.id).FirstOrDefault(),
                PersonalGroupKey = EmployeeInformation.included.Where(x => x.type.Equals("grupos-personal", StringComparison.CurrentCultureIgnoreCase)
                                                                        && x.id.Equals(dataContract.relationships.grupopersonalasignado.id, StringComparison.CurrentCultureIgnoreCase))
                                                               .Select(s => s.id).FirstOrDefault(),
                SeniorityDate = DateTime.Parse(dataContract.attributes.fechaAntiguedad),
                Birthdate = DateTime.Parse(dataContract.attributes.fechaNacimiento)
            };
            return result;
        }
    }
}
