namespace CheckupMedico.Infrastructure.Repository.LocalFile
{
    using CheckupMedico.Domain.Entity;
    using CheckupMedico.Domain.Repository.Interface.LocalFile;
    using CheckupMedico.Infrastructure.Repository.LocalFile.Base;
    using Microsoft.Extensions.Caching.Memory;

    public class RepoLocalFileBillingConfig : RepoLocalBase<BillingConfigEntity>, IRepoLocalFileBillingConfig
    {
        
        public RepoLocalFileBillingConfig(IMemoryCache cache) : base(cache, "Files/Billing.xlsx")
        {
        }

        public List<BillingConfigEntity> GetAll() => Load(row => new BillingConfigEntity
        {
            Institute = row.Cell(1).GetString().Trim(),
            Number = row.Cell(2).GetString().Trim(),
            FullName = row.Cell(3).GetString().Trim(),
            Rfc = row.Cell(4).GetString().Trim(),
        });
    }
}
