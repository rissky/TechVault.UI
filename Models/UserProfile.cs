namespace TechVault.UI.Models;

public class UserProfile
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-16);
    public string Role { get; set; } = string.Empty;
}