using CurrencyDB.Models;
using CurrencyDB.Repositories;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyDB.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly DapperContext _context;

    public ReportsController(DapperContext context) => _context = context;

    // GET-запрос для вызова функции из БД кросс-курса.
    [HttpGet("cross-rate")]
    public async Task<IActionResult> GetCrossRate(string baseCurr, string targetCurr, DateTime calcDate)
    {
        using var db = _context.CreateConnection();
        var result = await db.QuerySingleOrDefaultAsync<decimal?>(
            "SELECT get_cross_rate(@baseCurr, @targetCurr, @calcDate::date)", 
            new { baseCurr, targetCurr, calcDate });

        return result != null ? Ok(result) : NotFound("Данные не найдены");
    }
    // GET-запрос для получения истории котировок
    [HttpGet("dynamics")]
    public async Task<IActionResult> GetDynamics(string currencyCode, DateTime startDate, DateTime endDate)
    {
        using var db = _context.CreateConnection();
        var sql = "SELECT * FROM get_currency_dynamics_func(@currencyCode, @startDate::date, @endDate::date)";
        var result = await db.QueryAsync(sql, new { currencyCode, startDate, endDate });
    
        return Ok(result);
    }
    //GET-запрос для просмотра актуальных курсов валюты во всех банках на текущую дату
    [HttpGet("current-rate/{currencyCode}")]
    public async Task<IActionResult> GetCurrentRate(string currencyCode)
    {
        using var db = _context.CreateConnection();
        var sql = "SELECT * FROM get_current_rate(@currencyCode)";
        var result = await db.QueryAsync(sql, new { currencyCode });
    
        return Ok(result);
    }
    //GET-запрос для формирования отчета по спредам банков для поиска наиболее выгодных условий обмена
    [HttpGet("spread-report")]
    public async Task<IActionResult> GetSpreadReport(string currencyCode, DateTime reportDate)
    {
        using var db = _context.CreateConnection();
        var sql = "SELECT * FROM get_spread_report(@currencyCode, @reportDate::date)";
        var result = await db.QueryAsync(sql, new { currencyCode, reportDate });
    
        return Ok(result);
    }
}