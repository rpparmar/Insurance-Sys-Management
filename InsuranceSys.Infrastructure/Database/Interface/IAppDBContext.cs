using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Database.Interface
{
    public interface IAppDBContext
    {        
        Task<int> ExecuteNonQueryAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText,bool masterDBConn=true);
        Task<DataSet> GetDataSetAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, bool masterDBConn = true);
        Task<T> GetObjectAsync<T>(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, bool masterDBConn = true) where T : class, new();
        Task<string?> ExecuteScalarAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, bool masterDBConn = true);
        Task<DataTable> GetDataTableAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, bool masterDBConn = true);
        Task<SqlDataReader> GetSqlDataReaderAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText, bool masterDBConn = true);
        Task<int> GetNextIdAsync(string objName);
        //Task<string> ExecuteScalarAsync();
        //Task<T> GetEntityAsync<T>();
        //Task<List<T>> GetListAsync<T>() where T : new();
    }
}
