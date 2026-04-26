namespace Insurancesys.web.Api.Models
{
    public class ProductV1Response
    {
        public string Version { get; set; } = "1.0";
        public string Message { get; set; } = string.Empty;
    }

    public class ProductV2Response
    {
        public string Version { get; set; } = "2.0";
        public string Message { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new();
    }
}

