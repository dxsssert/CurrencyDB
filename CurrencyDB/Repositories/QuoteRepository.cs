using CurrencyDB.Models;
using Dapper;

namespace CurrencyDB.Repositories;

public class QuoteRepository
{
    private readonly DapperContext _context;
    public QuoteRepository(DapperContext context) => _context = context;

    public async Task<IEnumerable<Quote>> GetAllQuotesAsync()
    {
        using var connection = _context.CreateConnection();
        var query = @"SELECT 
                    quote_id AS QuoteId, 
                    quote_date::timestamp AS QuoteDate, 
                    rate_buy AS RateBuy, 
                    rate_sell AS RateSell, 
                    created_at AS CreatedAt, 
                    currency_id AS CurrencyId, 
                    bank_id AS BankId 
                  FROM quotes";
        return await connection.QueryAsync<Quote>(query);
    }
}