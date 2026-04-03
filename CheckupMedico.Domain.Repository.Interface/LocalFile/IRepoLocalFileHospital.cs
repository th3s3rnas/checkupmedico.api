namespace CheckupMedico.Domain.Repository.Interface.LocalFile
{
    using CheckupMedico.Domain.Entity;
    using CheckupMedico.Domain.Repository.Interface.LocalFile.Base;

    public interface IRepoLocalFileHospital : IRepoLocalFile
    {
        List<HospitalEntity> GetAll();
        List<HospitalEntity> GetByLocation(string location);
        List<HospitalEntity> GetByName(string name);
    }
}
