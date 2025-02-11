namespace EquipmentManagement.Services
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string userName, string email, List<string> roles);
        string GenerateRefreshToken();
        (bool isValid, string message) ValidateToken(string token);
    }
}
