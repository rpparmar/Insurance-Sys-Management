using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure
{
    public interface IAppDBContext
    {
        Task<int> ExecuteNonQueryAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "");
        Task<DataSet> GetDataSetAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "");
        Task<T> GetObjectAsync<T>(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "") where T : class,new();
        Task<string?> ExecuteScalarAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "");
        //Task<DataTable> GetDataTableAsync(Dictionary<string, object> paramCollection, CommandType cmdType, string cmdText, string dynamicConnstring = "");
        //Task<string> ExecuteScalarAsync();
        //Task<T> GetEntityAsync<T>();
        //Task<List<T>> GetListAsync<T>() where T : new();
    }
}
