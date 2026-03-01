using CurrencyDB.Models;
using Dapper;

namespace CurrencyDB.Repositories;

public class CurrencyRepository
{
    private readonly DapperContext _context;
    
    public CurrencyRepository(DapperContext context) => _context = context;

    public async Task<IEnumerable<Currency>> GetAllCurrencyAsync()
    {
        using var connection = _context.CreateConnection();
        var query = "SELECT currency_id AS CurrencyId, currency_name AS CurrencyName, " +
                    "is_crypto AS IsCrypto, created_at AS CreatedAt, digital_code AS DigitalCode, " +
                    "alpha_code AS AlphaCode FROM currencies";
        return await connection.QueryAsync<Currency>(query);
    }
}