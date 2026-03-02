using CurrencyDB.Models;
using CurrencyDB.Repositories;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace CurrencyDB.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class CurrenciesController : ControllerBase
{
    private readonly CurrencyRepository _repository;
    
    public CurrenciesController(CurrencyRepository repository) => _repository = repository;
    // Получение всех валют
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var currencies = await _repository.GetAllCurrencyAsync();
        return Ok(currencies);
    }
    // Добавление новой валюты
    [HttpPost]
    public async Task<IActionResult> InsertCurrency([FromBody] Currency currency)
    {
        try
        {
            var result = await _repository.CreateCurrencyAsync(currency);
            return Ok(result);
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            return BadRequest("Валюта с таким кодом уже существует: " + ex.Message);
        }
    }
    // Измнение информации валюты
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCurrency(int id, [FromBody] Currency currency)
    {
        if (id != currency.CurrencyId)
        {
            return BadRequest(new { Message = "ID не найден" });
        }
        try
        {
            await _repository.UpdateCurrencyAsync(currency);
            return Ok("Данные обновлены");
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            return BadRequest("Произошла ошибка в изменении данных: " + ex.Message);
        }
    }
    // Удаление валюты (+ проверка на использовании в бд)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCurrency(int id)
    {
        try
        {
            await _repository.DeleteCurrencyAsync(id);
            return Ok(new { Message = $"Валюта с Id {id} удалена" });
        }
        catch (PostgresException ex) when (ex.SqlState == "23503") // 23503 - ошибка внешнего ключа
        {
            return BadRequest("Ошибка в валидности данных: " + ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}