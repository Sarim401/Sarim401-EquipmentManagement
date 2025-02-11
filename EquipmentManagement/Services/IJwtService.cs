namespace EquipmentManagement.Services
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string userName, string email, List<string> roles);
    }
}
