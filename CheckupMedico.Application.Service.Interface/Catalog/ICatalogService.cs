namespace CheckupMedico.Application.Service.Interface.Catalog
{
    using CheckupMedico.Application.Dto.Catalog;
    public interface ICatalogService
    {
        public List<CampusListDto> GetCampus();
        public List<HospitalListDto> GetHospitalsByCampus(HospitalsReqDto req, DateTime birthDate);
    }
}
