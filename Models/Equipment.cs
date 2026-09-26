namespace TechVault.UI.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Condition { get; set; } = "Good";
    }
}