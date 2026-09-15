using Microsoft.EntityFrameworkCore;

namespace Project
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(p =>
            {
                p.HasKey(x => x.Id);

                p.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                p.Property(x => x.Price)
                    .HasPrecision(18, 2);

                p.ToTable(t =>
                    t.HasCheckConstraint(
                        "CK_Product_Price",
                        "[Price] >= 0"));
            });
        }
    }
}