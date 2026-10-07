using Microsoft.EntityFrameworkCore;
using TrainingCenter.Domain.Entities;
using TrainingCenter.Domain.Interfaces;

namespace TrainingCenter.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<TrainingTrack> TrainingTracks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            StampAuditFields();
            StampDeleteFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void StampDeleteFields()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.Entity.DeletedAt = now;
                        break;
                    case EntityState.Deleted:
                        entry.Entity.DeletedAt = now;
                        break;
                }
            }
        }

        private void StampAuditFields()
        {
            var now  = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<IAudiable>())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        break;
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        break;
                }
            }
        }
    }
}
