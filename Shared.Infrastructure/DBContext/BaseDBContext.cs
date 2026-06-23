using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Shared.Infrastructure.Models;
using System.Data;

namespace Shared.Infrastructure.DBContext;

public abstract class BaseDBContext
{
    private readonly string _connectionString;

    protected BaseDBContext(IOptions<DatabaseConnection> options)
    {
        _connectionString = options.Value.DefaultConnection;
    }

    protected async Task<SqlConnection> CreateConnectionAsync()
    {
        var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        return conn;
    }

    protected async Task<List<T>> QueryListAsync<T>(
        string storedProc,
        SqlParameter[]? parameters,
        Func<SqlDataReader, T> mapper)
    {
        var results = new List<T>();
        await using var conn = await CreateConnectionAsync();
        await using var cmd = new SqlCommand(storedProc, conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        if (parameters != null) cmd.Parameters.AddRange(parameters);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            results.Add(mapper(reader));

        return results;
    }

    protected async Task<T?> QuerySingleAsync<T>(
        string storedProc,
        SqlParameter[]? parameters,
        Func<SqlDataReader, T> mapper) where T : class
    {
        await using var conn = await CreateConnectionAsync();
        await using var cmd = new SqlCommand(storedProc, conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        if (parameters != null) cmd.Parameters.AddRange(parameters);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? mapper(reader) : null;
    }

    protected async Task ExecuteAsync(
        string storedProc,
        SqlParameter[]? parameters = null)
    {
        await using var conn = await CreateConnectionAsync();
        await using var cmd = new SqlCommand(storedProc, conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        if (parameters != null) cmd.Parameters.AddRange(parameters);
        await cmd.ExecuteNonQueryAsync();
    }

    protected async Task<T?> ExecuteScalarAsync<T>(
        string storedProc,
        SqlParameter[]? parameters = null)
    {
        await using var conn = await CreateConnectionAsync();
        await using var cmd = new SqlCommand(storedProc, conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        if (parameters != null) cmd.Parameters.AddRange(parameters);

        var result = await cmd.ExecuteScalarAsync();
        if (result is null || result == DBNull.Value) return default;
        return (T)Convert.ChangeType(result, typeof(T));
    }

    // Safe nullable readers
    protected static T? ReadNullable<T>(SqlDataReader r, string col) where T : struct
    {
        int ord = r.GetOrdinal(col);
        return r.IsDBNull(ord) ? null : r.GetFieldValue<T>(ord);
    }

    protected static string? ReadNullableString(SqlDataReader r, string col)
    {
        int ord = r.GetOrdinal(col);
        return r.IsDBNull(ord) ? null : r.GetString(ord);
    }
}
