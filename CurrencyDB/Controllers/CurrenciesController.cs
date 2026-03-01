using CurrencyDB.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyDB.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class CurrenciesController : ControllerBase
{
    private readonly CurrencyRepository _repository;
    
    public CurrenciesController(CurrencyRepository repository) => _repository = repository;
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
     
        var currencies = await _repository.GetAllCurrencyAsync();
        return Ok(currencies);
    }
}