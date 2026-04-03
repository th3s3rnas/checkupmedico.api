namespace CheckupMedico.Infrastructure.Repository.LocalFile
{
    using CheckupMedico.Domain.Entity;
    using CheckupMedico.Domain.Repository.Interface.LocalFile;
    using CheckupMedico.Infrastructure.Repository.LocalFile.Base;
    using Microsoft.Extensions.Caching.Memory;

    public class RepoLocalFileHospital : RepoLocalBase<HospitalEntity>, IRepoLocalFileHospital
    {
        public RepoLocalFileHospital(IMemoryCache cache) : base(cache, "Files/Hospitals.xlsx")
        {
        }

        public List<HospitalEntity> GetAll() => Load(row => new HospitalEntity
        {
            Location = row.Cell(1).GetString().Trim(),
            Name = row.Cell(2).GetString().Trim(),
            Campus = row.Cell(3).GetString().Trim(),
            Email = row.Cell(4).GetString().Trim(),
            LocationDetails = row.Cell(5).GetString().Trim(),
            Area = row.Cell(6).GetString().Trim(),
            ContactDetails = row.Cell(7).GetString().Trim(),
            Responsible = row.Cell(8).GetString().Trim(),
            City = row.Cell(9).GetString().Trim(),
            State = row.Cell(10).GetString().Trim(),
        });

        public List<HospitalEntity> GetByLocation(string location) => GetAll().Where(x => x.Location.Trim() == location.Trim()).ToList();

        public List<HospitalEntity> GetByName(string name) => GetAll().Where(x => x.Name.Trim() == name.Trim()).ToList();
    }
}
