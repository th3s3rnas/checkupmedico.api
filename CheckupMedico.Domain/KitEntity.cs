namespace CheckupMedico.Domain.Entity
{
    using CheckupMedico.Domain.Enum;
    public class KitEntity
    {
        public string HospitalName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public SexEnum Gender { get; set; }
        public int MinimumAge { get; set; }
        public int MaximumAge { get; set; }
    }
}
