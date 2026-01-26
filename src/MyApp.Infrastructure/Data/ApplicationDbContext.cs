using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;

namespace MyApp.Infrastructure.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options
        ) : base(options)
        {
        }

        // =========================
        // DbSets
        // =========================
        public DbSet<OtpCode> OtpCodes { get; set; }

        // =========================
        // Fluent API
        // =========================
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<OtpCode>(entity =>
            {
                entity.ToTable("OtpCodes");

                entity.HasKey(x => x.IdOtp);

                entity.Property(x => x.Code)
                      .IsRequired()
                      .HasMaxLength(6);

                entity.Property(x => x.CreatedAt)
                      .IsRequired();

                entity.Property(x => x.ExpirationDate)
                      .IsRequired();

                entity.Property(x => x.Status)
                      .IsRequired()
                      .HasConversion<int>();

                entity.Property(x => x.Purpose)
                      .IsRequired()
                      .HasConversion<int>();

                entity.HasOne(x => x.User)
                      .WithMany()
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);



                entity.HasIndex(x => new { x.UserId, x.Code, x.Purpose });
            });
        }
    }
}
