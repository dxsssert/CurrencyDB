using CurrencyDB.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyDB.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class QuotesController : ControllerBase
{
    private readonly QuoteRepository _repository;
    
    public QuotesController(QuoteRepository repository) => _repository = repository;
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        
        var quotes = await _repository.GetAllQuotesAsync();
        return Ok(quotes);
    }
}