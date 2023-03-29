using LittleFootStockManager.Data.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LittleFootStockManager.Data
{
    public class LittleFootStockManagerDbContext : IdentityDbContext<Users>
    {
        public LittleFootStockManagerDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasMany(u => u.Products)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Events)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(128);
                entity.Property(p => p.Description).IsRequired().HasMaxLength(1024);
                entity.Property(p => p.Category).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Quantity).IsRequired();
                entity.Property(p => p.QuantityAvaible).IsRequired();
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(p => p.ImagePath).IsRequired();


                entity.HasOne(p => p.User)
                    .WithMany(u => u.Products)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.NameClient).IsRequired().HasMaxLength(50);
                entity.Property(o => o.CreatedAt).IsRequired();
                entity.Property(o => o.DeliveryDate).IsRequired();
                entity.Property(o => o.IsPayed).IsRequired();
                entity.Property(o => o.AmountTotal).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(o => o.IsArchive).IsRequired();

                entity.HasMany<Product>(o => o.Products)
                    .WithOne();
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.NameClient).IsRequired().HasMaxLength(128);
                entity.Property(p => p.Lieux).IsRequired().HasMaxLength(128);
                entity.Property(p => p.Address).IsRequired().HasMaxLength(128);
                entity.Property(p => p.City).IsRequired().HasMaxLength(128);
                entity.Property(p => p.DDayDate).IsRequired();

                entity.HasMany<Product>(o => o.Products)
                    .WithOne();
            });

        }
    }

}
