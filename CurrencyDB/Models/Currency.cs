namespace CurrencyDB.Models;

public class Currency
{
    public int CurrencyId { get; set; }
    public string? CurrencyName { get; set; }
    public bool IsCrypto {get; set;}
    public DateTime CreatedAt { get; set; }
    public int DigitalCode { get; set; }
    public string? AlphaCode { get; set; }
}
