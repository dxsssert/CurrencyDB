using CurrencyDB.Models;
using Dapper;

namespace CurrencyDB.Repositories;

public class CurrencyRepository
{
    private readonly DapperContext _context;
    
    public CurrencyRepository(DapperContext context) => _context = context;
    
    // GET-запрос для получения всех валют с БД
    public async Task<IEnumerable<Currency>> GetAllCurrencyAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Currency>("SELECT * FROM get_all_currencies()");
    }
    // POST-запрос для создания новой валюты
    public async Task<Currency> CreateCurrencyAsync(Currency currency)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM create_currency_func(@CurrencyName, @AlphaCode, @IsActive)";
        return await connection.QuerySingleAsync<Currency>(sql, currency);
    }
    // UPDATE-запрос для изменения ошибочной валюты
    public async Task<Currency?> UpdateCurrencyAsync(Currency currency)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM update_currency_func(@CurrencyId, @CurrencyName, @AlphaCode, @IsActive)";
        return await connection.QuerySingleOrDefaultAsync<Currency>(sql, currency);
    }
    // DELETE-запрос для удаления валюты
    public async Task<int?> DeleteCurrencyAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT delete_currency_func(@Id)";
        return await connection.QuerySingleOrDefaultAsync<int>(sql, new { Id = id });
    }
}