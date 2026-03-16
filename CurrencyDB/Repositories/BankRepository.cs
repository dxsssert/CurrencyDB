
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
        return await connection.QueryAsync<Bank>("SELECT * FROM banks");
    }
    // POST-запрос для добавления нового банка 
    public async Task<Bank> CreateBankAsync(Bank bank)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM create_bank_proc(@BankName, @BankCode, @BankType, @IsActive)";
        return await connection.QuerySingleAsync<Bank>(sql, bank);
    }
    // UPDATE-запрос для изменения информации о банке
    public async Task<Bank?> UpdateBankAsync(Bank bank)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT * FROM update_bank_func(@BankId, @BankName, @BankCode, @BankType, @IsActive)";
        return await connection.QuerySingleOrDefaultAsync<Bank>(sql, bank);
    }
    // DELETE-запрос для удаления банка
    public async Task<int?> DeleteBankAsync(int bankId)
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT delete_bank_func(@BankId)";
        return await connection.QuerySingleOrDefaultAsync<int>(sql, new { BankId = bankId });
    }
}