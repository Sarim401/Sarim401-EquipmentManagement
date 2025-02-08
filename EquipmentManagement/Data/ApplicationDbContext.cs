using Microsoft.EntityFrameworkCore;
using EquipmentManagement.Models;

namespace EquipmentManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Device> Devices { get; set; }
    }
}
