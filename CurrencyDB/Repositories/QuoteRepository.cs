using CurrencyDB.Models;
using Dapper;

namespace CurrencyDB.Repositories;

public class QuoteRepository
{
    private readonly DapperContext _context;
    public QuoteRepository(DapperContext context) => _context = context;
    
    // GET-запрос для получения всех котировок с БД
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
    // POST-запрос для добавления котировки в БД
    public async Task<Quote> CreateQuoteAsync(Quote quote)
    {
        using var connection = _context.CreateConnection();
        var query = @"INSERT INTO quotes (quote_date, rate_buy, rate_sell, currency_id, bank_id) 
              VALUES (@QuoteDate, @RateBuy, @RateSell, @CurrencyId, @BankId) 
              RETURNING quote_id AS QuoteId, 
                        quote_date::timestamp AS QuoteDate, 
                        rate_buy AS RateBuy, 
                        rate_sell AS RateSell, 
                        created_at AS CreatedAt, 
                        currency_id AS CurrencyId, 
                        bank_id AS BankId";
        return await connection.QuerySingleAsync<Quote>(query, quote);
    }
    // UPDATE-запрос для изменения котировки в БД
    public async Task UpdateQuoteAsync(Quote quote)
    {
        using var connection = _context.CreateConnection();
        var query = @"UPDATE quotes 
                      SET rate_buy = @RateBuy, 
                          rate_sell = @RateSell,
                          quote_date = @QuoteDate
                      WHERE quote_id = @QuoteId";
        await connection.ExecuteAsync(query, quote);
    }
    // DELETE-запрос для удаления котировки из БД
    public async Task DeleteQuoteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var query = "DELETE FROM quotes WHERE quote_id = @Id";
        await connection.ExecuteAsync(query, new { Id = id });
    }
    
}