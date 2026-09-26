using System;

namespace TechVault.UI.Models
{
    public class LoanRecord
    {
        public int LoanId { get; set; }
        public int StudentId { get; set; }
        public int EquipmentId { get; set; }
        public DateTime CheckoutDate { get; set; } = DateTime.Now;
        public DateTime ExpectedReturn { get; set; } = DateTime.Now.AddDays(7);
        public string Status { get; set; } = "Active";

        // Navigation properties for convenience when displaying data
        public string StudentName { get; set; } = string.Empty;
        public string EquipmentDetails { get; set; } = string.Empty;
    }


}