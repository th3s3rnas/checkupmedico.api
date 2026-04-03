namespace CheckupMedico.Infrastructure.Repository.LocalFile
{
    using CheckupMedico.Domain.Entity;
    using CheckupMedico.Domain.Enum;
    using CheckupMedico.Domain.Repository.Interface.LocalFile;
    using CheckupMedico.Infrastructure.Repository.LocalFile.Base;
    using Microsoft.Extensions.Caching.Memory;

    public class RepoLocalFileKit : RepoLocalBase<KitEntity>, IRepoLocalFileKit
    {
        public RepoLocalFileKit(IMemoryCache cache) : base(cache, "Files/Kits.xlsx")
        {
        }

        public List<KitEntity> GetAll() => Load(row => new KitEntity
        {
            Name = row.Cell(1).GetString().Trim(),
            HospitalName = row.Cell(2).GetString().Trim(),
            Gender = row.Cell(3).GetString().ToUpper() == "HOMBRE" ? SexEnum.Hombre : SexEnum.Mujer,
            MinimumAge = row.Cell(4).GetValue<int>(),
            MaximumAge = row.Cell(5).GetValue<int>()
        });

        public KitEntity Get(string institute, SexEnum gender, decimal age) 
            => GetAll().FirstOrDefault(x => x.HospitalName == institute && x.Gender == gender && age > x.MinimumAge && age < x.MaximumAge)
               ?? throw new KeyNotFoundException($"No kit was found for institute '{institute}', gender '{gender}' and age '{age}'.");
    }
}
