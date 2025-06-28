using Insurancesys.web.Models.Common;
using Microsoft.Extensions.Primitives;
using System.Collections.Immutable;
using System.Data;

namespace Insurancesys.web.Helper
{
    public static class DataTableHelper
    {
        public static async Task<DataTableResponse> BuildGridResponseAsync(
        HttpRequest request
        , Func<ImmutableDictionary<string, object>, Task<DataSet>> getDataFunc
        , Func<HttpRequest, IDictionary<string, object>>? extraParamsFunc = null)
        {
            // Parse Form values
            string sortDirection = SearchSortValue(request.Form, "sSortDir_0", "desc").ToLowerInvariant();
            string sortField = SearchSortValue(request.Form, "SortingField", "1");
            string sortExp = $"{sortField} {(sortDirection == "asc" ? "asc" : "desc")}";
            string searchTerm = SearchSortValue(request.Form, "searchText");
            int page = int.TryParse(SearchSortValue(request.Form, "iDisplayStart", "0"), out var pg) ? pg : 0;
            int pageSize = int.TryParse(SearchSortValue(request.Form, "iDisplayLength", "10"), out var pz) ? pz : 10;

            var parameters = ImmutableDictionary.CreateBuilder<string, object>();

            parameters.Add("@PageNumber", page);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@SearchTerm", searchTerm);
            parameters.Add("@SortExp", sortExp);

            // Add additional dynamic filters if provided
            if (extraParamsFunc != null)
            {
                var extraParams = extraParamsFunc(request);
                foreach (var kvp in extraParams)
                {
                    parameters[kvp.Key] = kvp.Value;
                }
            }
            using var ds = await getDataFunc(parameters.ToImmutable());

            if (ds != null && ds.Tables.Count > 1)
            {
                int.TryParse(Convert.ToString(ds.Tables[0].Rows[0]["TotalRecords"]), out var totalRecords);

                using var dt = ds.Tables[1];

                var list = dt.AsEnumerable()
                    .Select(row => dt.Columns.Cast<DataColumn>()
                        .ToDictionary(
                            col => col.ColumnName,
                            col => FormatCellValue(row[col])
                        )
                    ).ToList();

                return new DataTableResponse
                {
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    data = list
                };
            }

            return new DataTableResponse(); // empty fallback
        }

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
