using System.Data;
using Npgsql;
using Dapper;
namespace CurrencyDB.Repositories;

public class DapperContext
{
    private readonly IConfiguration _configuration;
    static DapperContext()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public IDbConnection CreateConnection() => new NpgsqlConnection(_configuration.GetConnectionString("CurrencyDB"));
}