using Dapper;
using CurrencyDB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace CurrencyDB.Repositories;

public class ReportRepository
{
    private readonly DapperContext _context;

    public ReportRepository(DapperContext context)
    {
        _context = context;
    }

    // 1. Кросс-курс (возвращает число)
    public async Task<decimal?> GetCrossRateAsync(string baseCurr, string targetCurr, DateTime calcDate)
    {
        using var db = _context.CreateConnection();
        return await db.QuerySingleOrDefaultAsync<decimal?>(
            "SELECT get_cross_rate(@baseCurr, @targetCurr, @calcDate::date)", 
            new { baseCurr, targetCurr, calcDate });
    }

    // 2. Динамика курса (возвращает список объектов)
    public async Task<IEnumerable<dynamic>> GetDynamicsAsync(string currencyCode, DateTime startDate, DateTime endDate)
    {
        using var db = _context.CreateConnection();
        var sql = "SELECT * FROM get_currency_dynamics_func(@currencyCode, @startDate::date, @endDate::date)";
        var result = await db.QueryAsync(sql, new { currencyCode, startDate, endDate });
        return result;
    }

    // 3. Текущий курс
    public async Task<IEnumerable<dynamic>> GetCurrentRateAsync(string currencyCode)
    {
        using var db = _context.CreateConnection();
        var sql = "SELECT * FROM get_current_rate(@currencyCode)";
        var result = await db.QueryAsync(sql, new { currencyCode });
        return result;
    }

    // 4. Отчет по спредам
    public async Task<IEnumerable<dynamic>> GetSpreadReportAsync(string currencyCode, DateTime reportDate)
    {
        using var db = _context.CreateConnection();
        var sql = "SELECT * FROM get_spread_report(@currencyCode, @reportDate::date)";
        var result = await db.QueryAsync(sql, new { currencyCode, reportDate });
        return result;
    }
}