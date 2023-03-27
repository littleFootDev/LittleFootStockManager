using Microsoft.EntityFrameworkCore;

namespace LittleFootStockManager.Data
{
    public class LittleFootStockManagerDbContext : DbContext
    {
        public LittleFootStockManagerDbContext(DbContextOptions options) : base(options) { }
        
    }
}
