namespace CheckupMedico.Domain.Repository.Interface.LocalFile
{
    using CheckupMedico.Domain.Entity;
    using CheckupMedico.Domain.Enum;
    using CheckupMedico.Domain.Repository.Interface.LocalFile.Base;

    public interface IRepoLocalFileKit : IRepoLocalFile
    {
        List<KitEntity> GetAll();
        KitEntity Get(string institute, SexEnum gender, decimal age);
    }
}
