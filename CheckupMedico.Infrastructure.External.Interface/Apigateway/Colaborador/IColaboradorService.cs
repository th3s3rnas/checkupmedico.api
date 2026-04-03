namespace CheckupMedico.Infrastructure.External.Interface.Apigateway.Colaborador
{
    using CheckupMedico.Infrastructure.External.Model.Apigateway;
    public interface IColaboradorService
    {
        ResponseDto<UserProfileResponseDto> GetEmployeeInformation(string payroll, string email, string societyKey);
    }
}
