namespace CheckupMedico.Infrastructure.External.Apigateway.Base
{
    public class ApigatewayStructure
    {
        public List<WebService> WebServices { get; set; }
        public string Server { get; set; }
    }

    public class WebService
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Attributes> Headers { get; set; }
        public List<Attributes> Body { get; set; }
        public string URL { get; set; }
        public string Server { get; set; }
    }

    public class Attributes
    {
        public string Attribute { get; set; }
        public string Value { get; set; }
    }
}
