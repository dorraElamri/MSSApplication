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
        public DbSet<LogEntry> Logs { get; set; } = null!;
        public DbSet<Instance> Instances { get; set; } = null!;
        public DbSet<UserInstance> UserInstances { get; set; } = null!;



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


            builder.Entity<LogEntry>(entity =>
            {
                entity.ToTable("Logs");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Timestamp)
                      .HasColumnType("datetimeoffset")
                      .HasDefaultValueSql("SYSDATETIMEOFFSET()")
                      .IsRequired();

                entity.Property(e => e.Level)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.Message)
                      .IsRequired();

                entity.HasOne(l => l.Instance)
                      .WithMany(i => i.Logs)
                      .HasForeignKey(l => l.InstanceId)
                      .OnDelete(DeleteBehavior.Cascade);

                // 🔥 Index
                entity.HasIndex(e => e.InstanceId);
                entity.HasIndex(e => e.Timestamp);
                entity.HasIndex(e => e.Level);
                entity.HasIndex(e => new { e.InstanceId, e.Timestamp });

                entity.OwnsOne(l => l.SourceServer, ss =>
                {
                    ss.Property(p => p.Name).HasMaxLength(100);
                    ss.Property(p => p.Ip).HasMaxLength(50);
                });

                entity.OwnsOne(l => l.Request, r =>
                {
                    r.Property(p => p.Method).HasMaxLength(10);
                    r.Property(p => p.Endpoint).HasMaxLength(200);
                });

                entity.OwnsOne(l => l.Exception, ex =>
                {
                    ex.Property(p => p.Type).HasMaxLength(200);
                });
            });



            //builder.Entity<Instance>(entity =>
            //{
            //    entity.ToTable("Instances");

            //    entity.HasKey(i => i.Id);

            //    entity.Property(i => i.ApplicationName)
            //          .IsRequired()
            //          .HasMaxLength(100);

            //    entity.Property(i => i.Host)
            //          .IsRequired();

            //    entity.Property(i => i.ApiKey)
            //          .IsRequired()
            //          .HasMaxLength(100);

            //    entity.HasIndex(i => i.ApiKey).IsUnique();
            //});



            builder.Entity<Instance>(entity =>
            {
                entity.ToTable("Instances");

                entity.HasKey(i => i.Id);

                // 🔹 ApplicationName
                entity.Property(i => i.ApplicationName)
                      .HasMaxLength(150);

                // 🔹 Host
                entity.Property(i => i.Host)
                      .IsRequired()
                      .HasMaxLength(255);

                // 🔹 Description
                entity.Property(i => i.Description)
                      .HasMaxLength(500);

                // 🔐 ApiKey
                entity.Property(i => i.ApiKey)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasIndex(i => i.ApiKey)
                      .IsUnique();

                // 🔐 Security flags
                entity.Property(i => i.IsActive)
                      .HasDefaultValue(true);

                entity.Property(i => i.ApiKeyCreatedAt)
                      .IsRequired();

                entity.Property(i => i.ApiKeyLastUsedAt);

                // ⚙️ Config worker
                entity.Property(i => i.LogPath)
                      .HasMaxLength(500);

                entity.Property(i => i.Environment)
                      .HasConversion<int>()   // 🔥 enum -> int
                      .IsRequired();

                // 🕒 Audit
                entity.Property(i => i.CreatedAt)
                      .IsRequired();

                entity.Property(i => i.UpdatedAt);

                // 🔥 Index utiles
                entity.HasIndex(i => i.Host);
                entity.HasIndex(i => i.ApplicationName);
                entity.HasIndex(i => i.Environment);
            });



            builder.Entity<UserInstance>(entity =>
            {
                entity.ToTable("UserInstances");

                entity.HasKey(ui => ui.Id);

                entity.HasOne(ui => ui.User)
                      .WithMany(u => u.UserInstances)
                      .HasForeignKey(ui => ui.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ui => ui.Instance)
                      .WithMany(i => i.UserInstances)
                      .HasForeignKey(ui => ui.InstanceId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ui => new { ui.UserId, ui.InstanceId })
                      .IsUnique();

                entity.HasIndex(ui => ui.UserId);
                entity.HasIndex(ui => ui.InstanceId);
            });



        }
    }
}
