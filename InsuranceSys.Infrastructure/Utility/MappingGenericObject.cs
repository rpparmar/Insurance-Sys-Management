using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure
{
    public static class MappingGenericObject
    {
        public static List<T> MapToList<T>(SqlDataReader reader) where T : new()
        {
            var results = new List<T>();
            var properties = typeof(T).GetProperties();

            while (reader.Read())
            {
                T item = new T();
                foreach (var property in properties)
                {
                    if (reader.HasColumn(property.Name) && !reader.IsDBNull(reader.GetOrdinal(property.Name)))
                    {
                        property.SetValue(item, reader[property.Name]);
                    }
                }
                results.Add(item);
            }

            return results;
        }
        public static T MapToObject<T>(SqlDataReader reader) where T : new()
        {
            T item = new T();
            var properties = typeof(T).GetProperties();

            while (reader.Read())
            {
                foreach (var property in properties)
                {
                    if (reader.HasColumn(property.Name) && !reader.IsDBNull(reader.GetOrdinal(property.Name)))
                    {
                        property.SetValue(item, reader[property.Name]);
                    }
                }
            }
            return item;
        }
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
