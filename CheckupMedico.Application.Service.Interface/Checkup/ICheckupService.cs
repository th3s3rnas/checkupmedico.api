namespace CheckupMedico.Application.Service.Interface.Checkup
{
    using CheckupMedico.Application.Dto.Catalog;
    public interface ICheckupService
    {
        Stream Create(HospitalListDto req, string payrollId, string fullName, DateTime birthDate);
    }
}
