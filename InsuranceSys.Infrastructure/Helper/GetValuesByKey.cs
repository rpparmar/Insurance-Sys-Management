using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Helper
{
    public static class GetValues
    {
        // Helper function to retrieve values from ImmutableDictionary safely
        public static T GetValueOrDefault<T>(ImmutableDictionary<string, object> dict, string key, T defaultValue = default)
        {
            if (dict != null && dict.TryGetValue(key, out var value))
            {
                if (value is T typedValue)
                    return typedValue;
            }
            return defaultValue; // Return default if key not found or type mismatch
        }
    }
}
