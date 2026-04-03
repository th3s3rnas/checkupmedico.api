using CheckupMedico.Domain.Enum;

namespace CheckupMedico.Infrastructure.External.Model.Apigateway
{
    public class UserProfileResponseDto
    {
        public string PersonID { get; set; }
        public string PayrollID { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public string Profiles { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LastName2 { get; set; }
        public string FullName { get; set; }
        public DateTime Birthdate { get; set; }
        public string SocietyKey { get; set; }
        public SexEnum Gender { get; set; }
        public KeyValueString Society { get; set; }
        public KeyValueString Division { get; set; }
        public KeyValueString Subdivision { get; set; }
        public string PersonalAreaKey { get; set; }
        public string PersonalGroupKey { get; set; }
        public string EffectiveEndDate { get; set; }
        public string EffectiveStartDate { get; set; }
        public DateTime SeniorityDate { get; set; }
        public string ActiveContract { get; set; }
        public string TokenJWT { get; set; }

        public UserProfileResponseDto ToEmployeeData()
        {
            return new UserProfileResponseDto()
            {
                PayrollID = PayrollID,
                FirstName = FirstName,
                LastName = LastName,
                LastName2 = LastName2,
                Email = Email,
                Birthdate = Birthdate,
                SeniorityDate = SeniorityDate,
                Society = Society,
                Division = Division,
                Subdivision = Subdivision,
                PersonalAreaKey = PersonalAreaKey,
                PersonalGroupKey = PersonalGroupKey
            };
        }
    }
}
