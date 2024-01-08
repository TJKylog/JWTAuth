using JWTAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace JWTAuth
{
    public class JWTAuthDbContext : DbContext
    {
        public JWTAuthDbContext(DbContextOptions<JWTAuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(user =>
            {
                user.ToTable("users");
                user.HasKey(x => x.Id);
                user.Property(x => x.Id).HasColumnName("id");
                user.Property(x => x.Name).HasColumnName("name");
                user.Property(x => x.Email).HasColumnName("email");
                user.Property(x => x.Password).HasColumnName("password");
                user.Property(x => x.IsActive).HasColumnName("is_active");
            });
        }

        public DbSet<User> Users { get; set; }
    }
}