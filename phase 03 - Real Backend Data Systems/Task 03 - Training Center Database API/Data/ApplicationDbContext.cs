using Microsoft.EntityFrameworkCore;
using Task_03___Training_Center_Database_API.Entities;
using Task_03___Training_Center_Database_API.Utilities.Enums;
using Task_03___Training_Center_Database_API.Utilities.Interfaces;

namespace Task_03___Training_Center_Database_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<TrainingTrack> TrainingTracks { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasIndex(s => s.Email).IsUnique();

                entity.Property(s => s.PhoneNumber).HasMaxLength(11);
                entity.Property(s => s.FName).HasColumnName("First Name").HasMaxLength(20).IsRequired();
                entity.Property(s => s.LName).HasColumnName("Last Name").HasMaxLength(20).IsRequired();
                entity.Property(s => s.Email).HasMaxLength(50).IsRequired();

                entity.HasMany(s => s.Enrollments)
                      .WithOne(e => e.Student)
                      .HasForeignKey(f => f.StudentId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

            });

            modelBuilder.Entity<Instructor>(entity =>
            {
                entity.HasIndex(s => s.Email).IsUnique();

                entity.Property(s => s.PhoneNumber).HasMaxLength(11);
                entity.Property(s => s.FName).HasColumnName("First Name").HasMaxLength(20).IsRequired();
                entity.Property(s => s.LName).HasColumnName("Last Name").HasMaxLength(20).IsRequired();
                entity.Property(s => s.Email).HasMaxLength(50).IsRequired();
                entity.Property(s => s.Bio).HasMaxLength(250).IsRequired(false);
                entity.Property(s => s.Specialization).HasMaxLength(50).IsRequired(false);

                entity.HasMany(s => s.TrainingTracks)
                      .WithOne(e => e.Instructor)
                      .HasForeignKey(f => f.InstructorId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

            });

            modelBuilder.Entity<TrainingTrack>(entity =>
            {
                entity.HasIndex(s => s.Code).IsUnique();

                entity.Property(s => s.Title).HasMaxLength(50).IsRequired();
                entity.Property(s => s.Code).HasMaxLength(8).IsRequired();
                entity.Property(s => s.Description).HasMaxLength(250).IsRequired();

                entity.HasMany(t => t.Enrollments)
                      .WithOne(e => e.TrainingTrack)
                      .HasForeignKey(f => f.TrainingTrackId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();

            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasMany(t => t.Payments)
                      .WithOne(e => e.Enrollment)
                      .HasForeignKey(f => f.EnrollmentId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired();
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasIndex(s => s.ReferenceNumber).IsUnique();

                entity.Property(s => s.ReferenceNumber).IsRequired();
                entity.Property(s => s.Notes).HasMaxLength(150).IsRequired(false);

            });

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            StampAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
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

            now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.Entity.DeletedAt = now;
                        break;
                   
                }
            }
        }
    }
}
