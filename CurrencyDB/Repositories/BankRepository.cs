
using CurrencyDB.Models;
using Dapper;

namespace CurrencyDB.Repositories;

public class BankRepository
{
    private readonly DapperContext _context;
    
    public BankRepository(DapperContext context) => _context = context;
    
    // GET-запрос для получения всех банков с БД
    public async Task<IEnumerable<Bank>> GetAllBanksAsync()
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT bank_id AS BankId, bank_name AS BankName, bank_code AS BankCode, bank_type AS BankType, is_active AS IsActive FROM banks";
        return await connection.QueryAsync<Bank>(sql);
    }
    // POST-запрос для добавления нового банка 
    public async Task<Bank> CreateBankAsync(Bank bank)
    {
        using var connection = _context.CreateConnection();
        var query = @"INSERT INTO banks (bank_name, bank_code, bank_type, is_active) 
                  VALUES (@BankName, @BankCode, @BankType, @IsActive) 
                  RETURNING bank_id AS BankId, bank_name AS BankName, 
                            bank_code AS BankCode, bank_type AS BankType, 
                            is_active AS IsActive";
        return await connection.QuerySingleAsync<Bank>(query, bank);
    }
    // UPDATE-запрос для изменения информации о банке
    public async Task UpdateBankAsync(Bank bank)
    {
        using var connection = _context.CreateConnection();
        var query = @"UPDATE banks 
                  SET bank_name = @BankName, 
                      bank_code = @BankCode, 
                      bank_type = @BankType, 
                      is_active = @IsActive 
                  WHERE bank_id = @BankId";
        await  connection.ExecuteAsync(query, bank);
    }
    // DELETE-запрос для удаление банка
    public async Task DeleteBankAsync(int bankId)
    {
        using var connection = _context.CreateConnection();
        var query = @"DELETE FROM banks WHERE bank_id = @BankId";
        await connection.ExecuteAsync(query, new {BankId = bankId});
    }
}