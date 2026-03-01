
using CurrencyDB.Models;
using Dapper;

namespace CurrencyDB.Repositories;

public class BankRepository
{
    private readonly DapperContext _context;
    
    public BankRepository(DapperContext context) => _context = context;
    
    public async Task<IEnumerable<Bank>> GetAllBanksAsync()
    {
        using var connection = _context.CreateConnection();
        var sql = "SELECT bank_id AS BankId, bank_name AS BankName, bank_code AS BankCode, bank_type AS BankType, is_active AS IsActive FROM banks";
        return await connection.QueryAsync<Bank>(sql);
    }
}