namespace CheckupMedico.Infrastructure.External.Model.Apigateway
{
    public class TokenJWTResponse
    {
        public Meta meta { get; set; }
        public Jsonapi jsonapi { get; set; }
        public Links links { get; set; }
    }
    public class Meta
    {
        public string token { get; set; }
    }

    //public class Jsonapi
    //{
    //    public string version { get; set; }
    //}

    //public class Links
    //{
    //    public string self { get; set; }
    //}
}
