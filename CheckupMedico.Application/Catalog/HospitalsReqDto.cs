namespace CheckupMedico.Application.Dto.Catalog
{
    using CheckupMedico.Domain.Enum;
    public class HospitalsReqDto
    {
        public SexEnum Gender { get; set; }
        public string CampusName { get; set; } = null!;
    }
}
