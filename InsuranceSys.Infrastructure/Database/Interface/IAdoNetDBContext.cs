using System.Collections.Immutable;
using System.Data;

namespace InsuranceSys.Infrastructure.Database.Interface
{
    public interface IAdoNetDBContext
    {
        Task<int> ExecuteNonQueryAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText);
        Task<DataSet> GetDataSetAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText);
        Task<T?> GetObjectAsync<T>(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText) where T : class, new();
        Task<List<T>> GetListAsync<T>(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText) where T : class, new();
        Task<string?> ExecuteScalarAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText);
        Task<T?> ExecuteScalarAsync<T>(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText);
        Task<DataTable> GetDataTableAsync(ImmutableDictionary<string, object> paramCollection, CommandType cmdType, string cmdText);
        Task<int> GetNextIdAsync(string objName);
    }
}
