namespace TechVault.UI.Models;

public class LoanRecord
{
    public int StudentId { get; set; }
    public string EquipmentType { get; set; } = string.Empty;
    public DateTime ExpectedReturn { get; set; } = DateTime.Now.AddDays(1);
}