namespace CheckupMedico.Domain.Repository.Interface.LocalFile
{
    using CheckupMedico.Domain.Entity;
    using CheckupMedico.Domain.Repository.Interface.LocalFile.Base;

    public interface IRepoLocalFileBillingConfig : IRepoLocalFile
    {
        List<BillingConfigEntity> GetAll();
    }
}
