namespace CheckupMedico.Application.Service.Checkup
{
    using CheckupMedico.Application.Doc.Interface;
    using CheckupMedico.Application.Dto.Catalog;
    using CheckupMedico.Application.Dto.Checkup;
    using CheckupMedico.Application.Service.Interface.Checkup;
    using CheckupMedico.Domain.Repository.Interface.LocalFile;

    public class CheckupService : ICheckupService
    {
        private readonly IRepoLocalFileBillingConfig _repoLocalFileBillingConfig;
        private readonly ICheckupITESMDoc _checkupITESMDoc;
        public CheckupService(
            IRepoLocalFileBillingConfig repoLocalFileBillingConfig,
            ICheckupITESMDoc checkupITESMDoc)
        {
            _repoLocalFileBillingConfig = repoLocalFileBillingConfig;
            _checkupITESMDoc = checkupITESMDoc;
        }

        public Stream Create(HospitalListDto req, string payrollId, string fullName, DateTime birthDate)
        {
            var billingData = _repoLocalFileBillingConfig.GetAll().FirstOrDefault();

            if (billingData is null)
                throw new InvalidOperationException("Billing configuration was not found.");

            _checkupITESMDoc.Build(new CheckupITESMDto()
            {
                City = req.City,
                State = req.State,
                Hospital = req.Name,
                Campus = req.Campus,
                Responsible = req.Responsible,
                Institute = billingData.Institute,
                PayrollId = payrollId,
                Name = fullName,
                BirthDate = birthDate,
                Kit = req.Kit,
                LocationDetails = req.LocationDetails,
                ContactDetails = req.ContactDetails,
                Email = req.Email,
                FullName = billingData.FullName,
            });

            return new MemoryStream(_checkupITESMDoc.GetDoc());
        }
    }
}
