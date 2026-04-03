namespace CheckupMedico.Infrastructure.External.Model.Apigateway
{
    using Newtonsoft.Json;

    public class APIContractsWSResponse
    {
        public Jsonapi jsonapi { get; set; }
        public Links links { get; set; }
        public Datum[] data { get; set; }
        public Included[] included { get; set; }
    }

    public class Jsonapi
    {
        public string version { get; set; }
    }

    public class Links
    {
        public string self { get; set; }
    }

    public class Datum
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public Relationships relationships { get; set; }
    }

    public class Attributes
    {
        public string contrato { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string fechaInicioVigencia { get; set; }
        public string fechaFinVigencia { get; set; }
        public string fechaAntiguedad { get; set; }
        public string fechaNacimiento { get; set; }
    }

    public class Relationships
    {
        [JsonProperty("sociedad-contratante")]
        public SociedadContratante sociedadcontratante { get; set; }

        [JsonProperty("division-contratante")]
        public DivisionContratante divisioncontratante { get; set; }

        [JsonProperty("sub-division-contratante")]
        public SubDivisionContratante subdivisioncontratante { get; set; }

        [JsonProperty("grupo-personal-asignado")]
        public GrupoPersonalAsignado grupopersonalasignado { get; set; }

        [JsonProperty("area-personal-asignada")]
        public AreaPersonalAsignada areapersonalasignada { get; set; }
    }

    public class SociedadContratante
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class DivisionContratante
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class SubDivisionContratante
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class GrupoPersonalAsignado
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class AreaPersonalAsignada
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Included
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes1 attributes { get; set; }
    }

    public class Attributes1
    {
        public string descripcion { get; set; }
    }
}
