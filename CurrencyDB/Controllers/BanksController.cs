using CurrencyDB.Models;
using Microsoft.AspNetCore.Mvc;
using CurrencyDB.Repositories;
using Npgsql;

namespace CurrencyDB.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BanksController : ControllerBase
{
    private readonly BankRepository _repository;
    public BanksController(BankRepository repository) => _repository = repository;
    
    // Получение всех банков
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var banks = await _repository.GetAllBanksAsync();
        return Ok(banks);
    }
    // Добавление нового банка
    [HttpPost]
    public async Task<IActionResult> InsertBank([FromBody] Bank bank)
    {
        try
        {
            var result = await _repository.CreateBankAsync(bank);
            return Ok(result);
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            return BadRequest("Банк с таким названием или кодом уже существует: " + ex.Message);
        }
    }
    // Измнение информации банк
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBank(int id, [FromBody] Bank bank)
    {
        if (id != bank.BankId)
        {
            return BadRequest(new { Message = "ID не найден" });
        }

        try
        {
            await _repository.UpdateBankAsync(bank);
            return Ok(new { Message = "Данные обновлены", Data = bank });
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            return BadRequest("Банк с таким названием или кодом уже существует: " + ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    // Удаление банка (+ проверка на использовании в бд)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _repository.DeleteBankAsync(id);
            return Ok("Банк удалён");
        }
        catch (PostgresException ex) when (ex.SqlState == "23503")
        {
            
            return BadRequest("Банк удалить нельзя (предоставляет котировки)" + ex.Message);
        }
    }
}