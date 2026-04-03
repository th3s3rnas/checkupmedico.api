namespace CheckupMedico.Infrastructure.External.Model.Apigateway
{
    using System.ComponentModel.DataAnnotations;
    public class UserRequestDto
    {
        [Required(ErrorMessage = "Se requiere la nómina del colaborador")]
        public string payroll { get; set; }

        [Required(ErrorMessage = "Se requiere el correo institucional del colaborador")]
        public string email { get; set; }
    }
}
