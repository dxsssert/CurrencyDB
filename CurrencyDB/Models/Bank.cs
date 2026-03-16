namespace CurrencyDB.Models;

public class Bank
{
    public int BankId { get; set; }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public string? BankType { get; set; }
    public bool IsActive { get; set; }
    
}