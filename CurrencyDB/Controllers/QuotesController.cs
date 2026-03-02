using CurrencyDB.Models;
using CurrencyDB.Repositories;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace CurrencyDB.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class QuotesController : ControllerBase
{
    private readonly QuoteRepository _repository;
    
    public QuotesController(QuoteRepository repository) => _repository = repository;
    
    // Получение всех котировок
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        
        var quotes = await _repository.GetAllQuotesAsync();
        return Ok(quotes);
    }
    // Добавление новой котировки
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Quote quote)
    {
        try
        {
            var result = await _repository.CreateQuoteAsync(quote);
            return Ok(result);
        }
        catch (PostgresException ex) when (ex.SqlState == "23503") // Ошибка внешнего ключа
        {
            
            return BadRequest("Ошибка: Указанный банк или валюта не существуют в базе. " + ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Системная ошибка: " + ex.Message);
        }
    }
    // Изменение котировки (исправление ошибочного курса)
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Quote quote)
    {
        
        if (id != quote.QuoteId)
        {
            return BadRequest(new { Message = "ID котировки не найден или не совпадает" });
        }

        try
        {
            await _repository.UpdateQuoteAsync(quote);
            return Ok("Курс успешно обновлен");
        }
        catch (PostgresException ex) when (ex.SqlState == "23503")
        {
            return BadRequest("Не удалось обновить: нарушена связь с банком или валютой. " + ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ошибка при обновлении данных: " + ex.Message);
        }
    }
    // Удаление котировки
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repository.DeleteQuoteAsync(id);
            return Ok(new { Message = $"Котировка с Id {id} удалена" });
        }
        catch (Exception ex)
        {
            
            return StatusCode(500, "Ошибка при удалении: " + ex.Message);
        }
    }
}