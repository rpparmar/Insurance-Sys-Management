using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Insurancesys.web.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insurancesys.web.Helper
{
    /// <summary>
    /// SelectList helpers for vehicle Fuel and Plan Type; values align with <see cref="PolicyVehicleFuelCode"/> and <see cref="PolicyVehiclePlanTypeCode"/>.
    /// </summary>
    public static class VehiclePolicyDropdowns
    {
        public static List<SelectListItem> GetFuelSelectList(string? selected)
            => ToSelectListFromEnum<PolicyVehicleFuelCode>(selected);

        public static List<SelectListItem> GetPlanTypeSelectList(string? selected)
            => ToSelectListFromEnum<PolicyVehiclePlanTypeCode>(selected);

        public static bool IsValidFuelCode(string? value)
            => IsValidEnumCode<PolicyVehicleFuelCode>(value);

        public static bool IsValidPlanTypeCode(string? value)
            => IsValidEnumCode<PolicyVehiclePlanTypeCode>(value);

        private static bool IsValidEnumCode<TEnum>(string? value) where TEnum : struct, Enum
        {
            if (string.IsNullOrEmpty(value))
                return true;
            return Enum.TryParse(value, ignoreCase: true, out TEnum _);
        }

        private static List<SelectListItem> ToSelectListFromEnum<TEnum>(string? selected) where TEnum : struct, Enum
        {
            var selectedNorm = selected?.Trim();
            return Enum.GetValues<TEnum>()
                .Select(e =>
                {
                    var name = e.ToString();
                    return new SelectListItem
                    {
                        Value = name,
                        Text = GetDisplayName(e) ?? name,
                        Selected = !string.IsNullOrEmpty(selectedNorm) &&
                                   string.Equals(name, selectedNorm, StringComparison.OrdinalIgnoreCase)
                    };
                })
                .ToList();
        }

        private static string? GetDisplayName(Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            return member?.GetCustomAttribute<DisplayAttribute>()?.Name;
        }
    }
}
