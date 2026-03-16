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
        return await connection.QueryAsync<Quote>("SELECT * FROM get_all_quotes()");
    }
    // POST-запрос для добавления котировки в БД
    public async Task<Quote> CreateQuoteAsync(Quote quote)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM create_quote_func(@QuoteDate::date, @RateBuy, @RateSell, @CurrencyId, @BankId)";
        return await connection.QuerySingleAsync<Quote>(sql, quote);
    }
    // UPDATE-запрос для изменения котировки в БД
    public async Task<Quote?> UpdateQuoteAsync(Quote quote)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM update_quote_func(@QuoteId, @QuoteDate::date, @RateBuy, @RateSell)";
        return await connection.QuerySingleOrDefaultAsync<Quote>(sql, quote);
    }
    // DELETE-запрос для удаления котировки из БД
    public async Task<int?> DeleteQuoteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT delete_quote_func(@Id)";
        return await connection.QuerySingleOrDefaultAsync<int>(sql, new { Id = id });
    }
    
}