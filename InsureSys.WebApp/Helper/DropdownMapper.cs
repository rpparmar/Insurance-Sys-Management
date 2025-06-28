using InsuranceSys.Application.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insurancesys.web.Helper
{
    public static class DropdownMapper
    {
        public static List<SelectListItem> ToSelectListItems(List<DropdownItemDto> source)
        {
            return source.Select(x => new SelectListItem
            {
                Value = x.Value,
                Text = x.Text,
                Selected = x.Selected
            }).ToList();
        }
        public static List<SelectListItem> ToSelectListItems(List<DropdownItemDto> source, string? selectedvalues)
        {            
           var selectedIds = new HashSet<string>();
            if (!string.IsNullOrWhiteSpace(selectedvalues))
            {
                selectedIds = selectedvalues
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => id.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            }                
            return source.Select(c => new SelectListItem
            {
                Value = c.Value,
                Text = c.Text,
                Selected = c.Value != null && selectedIds.Contains(c.Value)
            }).ToList() ?? new List<SelectListItem>();
        }        
    }
}
