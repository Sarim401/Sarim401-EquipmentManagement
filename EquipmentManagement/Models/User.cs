using Microsoft.AspNetCore.Identity;

namespace EquipmentManagement.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
