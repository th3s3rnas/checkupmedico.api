namespace CheckupMedico.Infrastructure.External.Interface.Apigateway.Colaborador
{
    using CheckupMedico.Infrastructure.External.Model.Apigateway;
    public interface IColaboradorService
    {
        Task<ResponseDto<UserProfileResponseDto>> GetEmployeeInformationAsync(string payroll, string email, string societyKey);
    }
}
