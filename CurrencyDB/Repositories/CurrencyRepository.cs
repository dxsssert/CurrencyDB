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
        var query = "SELECT currency_id AS CurrencyId, currency_name AS CurrencyName, " +
                    "is_crypto AS IsCrypto, created_at AS CreatedAt, digital_code AS DigitalCode, " +
                    "alpha_code AS AlphaCode FROM currencies";
        return await connection.QueryAsync<Currency>(query);
    }
    // POST-запрос для  создания новой  валюты
    public async Task<Currency> CreateCurrencyAsync(Currency currency)
    {
        using var connection = _context.CreateConnection();
        var query = @"INSERT INTO currencies (currency_name, is_crypto, digital_code, alpha_code) 
                  VALUES (@CurrencyName, @IsCrypto, @DigitalCode, @AlphaCode) 
                  RETURNING *";
        return await connection.QuerySingleAsync<Currency>(query, currency);
    }
    // UPDATE-запрос для изменения ошибочной валюты
    public async Task UpdateCurrencyAsync(Currency currency)
    {
        using var connection = _context.CreateConnection();
        var query = @"UPDATE currencies 
                  SET currency_name = @CurrencyName, 
                      is_crypto = @IsCrypto, 
                      digital_code = @DigitalCode, 
                      alpha_code = @AlphaCode 
                  WHERE currency_id = @CurrencyId";
        await connection.ExecuteAsync(query, currency);
    }
    // DELETE-запрос для удаления валюты
    public async Task DeleteCurrencyAsync(int currencyId)
    {
        using var connection = _context.CreateConnection();
        var query = @"DELETE FROM currencies WHERE currency_id = @CurrencyId";
        await connection.ExecuteAsync(query, new { CurrencyId = currencyId });
    }
}