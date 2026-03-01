using Microsoft.AspNetCore.Mvc;
using CurrencyDB.Repositories;

namespace CurrencyDB.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BanksController : ControllerBase
{
    private readonly BankRepository _repository;
    
    public BanksController(BankRepository repository) => _repository = repository;
    
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var banks = await _repository.GetAllBanksAsync();
        return Ok(banks);
    }
}