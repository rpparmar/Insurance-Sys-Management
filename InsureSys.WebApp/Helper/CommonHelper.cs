using Microsoft.Extensions.Primitives;

namespace Insurancesys.web.Helper
{
    public static class CommonHelper
    {
        public static object FormatCellValue(object value)
        {
            if (value == null || value == DBNull.Value)
                return "NA";

            if (value is string strVal && string.IsNullOrWhiteSpace(strVal))
                return "NA";

            return value;
        }
        public static string SearchSortValue(IFormCollection form, string key, string defaultValue = "")
        {
            return form.TryGetValue(key, out var values) && !StringValues.IsNullOrEmpty(values)
                ? values[0]?.Trim() ?? defaultValue
                : defaultValue;
        }
    }
}
