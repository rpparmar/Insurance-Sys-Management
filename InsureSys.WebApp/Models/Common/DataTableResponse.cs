namespace Insurancesys.web.Models.Common
{
    public class DataTableResponse
    {
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public List<Dictionary<string, object>> data { get; set; } = new();
    }
}
