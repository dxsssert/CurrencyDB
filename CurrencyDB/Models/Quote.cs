namespace CurrencyDB.Models;

public class Quote
{
    public int QuoteId { get; set; }
    public DateTime QuoteDate { get; set; }
    public decimal RateBuy { get; set; }
    public decimal RateSell { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CurrencyId { get; set; }
    public int BankId { get; set; }
    public string? BankName { get; set; }
    public string? CurrencyName { get; set; }
}