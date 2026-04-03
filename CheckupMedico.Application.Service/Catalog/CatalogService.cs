namespace CheckupMedico.Application.Service.Catalog
{
    using CheckupMedico.Application.Dto.Catalog;
    using CheckupMedico.Application.Service.Interface.Catalog;
    using CheckupMedico.Domain.Repository.Interface.LocalFile;
    using CheckupMedico.Transversal.Exception;
    using CheckupMedico.Transversal.Util;

    public class CatalogService : ICatalogService
    {
        private readonly IRepoLocalFileHospital _repoLocalFileHospital;
        private readonly IRepoLocalFileKit _repoLocalFileKit;

        public CatalogService(
            IRepoLocalFileHospital repoLocalFileHospital,
            IRepoLocalFileKit repoLocalFileKit)
        {
            _repoLocalFileHospital = repoLocalFileHospital;
            _repoLocalFileKit = repoLocalFileKit;
        }

        public List<CampusListDto> GetCampus()
        {
            var datos = _repoLocalFileHospital.GetAll().Select(x => new CampusListDto()
            {
                Code = x.Location,
                Name = x.Location,
            }).DistinctBy(x => x.Code).ToList();

            return datos;
        }

        public List<HospitalListDto> GetHospitalsByCampus(HospitalsReqDto req, DateTime birthDate)
        {
            var age = CalculateAgeDecimal(birthDate);
            var kits = _repoLocalFileKit.GetAll();
            var datos = _repoLocalFileHospital.GetByLocation(req.CampusName).Select(x =>
            {
                var dto = new HospitalListDto();
                dto.Location = x.Location;
                dto.Name = x.Name;
                dto.Campus = x.Campus;
                dto.Email = x.Email;
                dto.Responsible = x.Responsible;
                dto.LocationDetails = x.LocationDetails;
                dto.Area = x.Area;
                dto.ContactDetails = x.ContactDetails;
                dto.City = x.City;
                dto.State = x.State;

                var kit = kits.FirstOrDefault(k => k.HospitalName.Trim().ToUpper() == x.Name.Trim().ToUpper() && k.Gender == req.Gender && age > k.MinimumAge && age < k.MaximumAge);
                if (kit is null)
                    throw new NotFoundException($"No se encontró un paquete de checkup para el Hospital '{x.Name}', sexo '{req.Gender.GetDescription()}' y edad '{age}' años.");

                dto.Kit = kit.Name;
                return dto;
            }).ToList();

            return datos;
        }

        private decimal CalculateAgeDecimal(DateTime birthDate)
        {
            var today = DateTime.UtcNow.Date;
            var age = (decimal)(today - birthDate.Date).TotalDays / 365.2425m;

            return Math.Round(age, 2, MidpointRounding.AwayFromZero);
        }
    }
}
