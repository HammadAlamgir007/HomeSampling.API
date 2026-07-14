using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

using Shared.Infrastructure.Models;
using System.Data;

namespace Shared.Infrastructure.DBContext
{
    public abstract class BaseDBContext
    {
        private readonly String _connectionString;
        protected BaseDBContext(IOptions<DatabaseConnection> options)
        {
            _connectionString = options.Value.DefaultConnection;
        }
        protected IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
        protected async Task<List<T>> QueryListAsync<T>(
            string storedProcedure,
            object? parameters = null)
        {
            using var connection = CreateConnection();
            var result = await connection.QueryAsync<T>(
            storedProcedure,
            parameters,
                commandType: CommandType.StoredProcedure);

            return result.AsList();

        }
        protected async Task<T?> QuerySingleAsync<T>(
       string storedProcedure,
       object? parameters = null)
        {
            using var connection = CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<T>(
             storedProcedure,
             parameters,
            commandType: CommandType.StoredProcedure); ;
        }

        protected async Task<int> ExecuteAsync(
          string storedProcedure,
          object? parameters = null)
        {
            using var connection = CreateConnection();
            
            return await connection.ExecuteAsync(
          storedProcedure,
          parameters,
          commandType: CommandType.StoredProcedure); ;
        }


        protected async Task<T?> ExecuteScalarAsync<T>(
        string storedProcedure,
        object? parameters = null)
        {
            using var connection = CreateConnection();
            var result = await connection.ExecuteScalarAsync<T>(
            storedProcedure,
            parameters,
            commandType: CommandType.StoredProcedure);
            return result;
        }

    }

}