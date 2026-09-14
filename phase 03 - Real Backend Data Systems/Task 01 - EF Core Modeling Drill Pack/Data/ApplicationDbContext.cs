using Microsoft.EntityFrameworkCore;
using Task_01___EF_Core_Modeling_Drill_Pack.Entities;

namespace Task_01___EF_Core_Modeling_Drill_Pack.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {
           
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentProfile> StudentsProfile { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<TrainingTrack> TrainingTracks { get; set; }
        public DbSet<Enrollment> Enrollments {  get; set; }
        public DbSet<PaymentSummary> PaymentSummaries { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentProfile>()
                .HasKey(s=>s.SSN);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.StudentProfile)
                .WithOne(s => s.Student)
                .HasForeignKey<StudentProfile>(s => s.StudentId)
                .IsRequired();

            modelBuilder.Entity<Instructor>()
                .HasMany(i => i.TrainingTracks)
                .WithOne(t => t.Instructor)
                .HasForeignKey(t => t.InstructorId)
                .IsRequired();

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Enrollments)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.TrainingTrack)
                      .WithMany(t => t.Enrollments)
                      .HasForeignKey(e => e.TrainingTrackId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.StudentId, e.TrainingTrackId });
            });

            modelBuilder.Entity<PaymentSummary>()
                .HasOne(p => p.Enrollment)
                .WithOne(e => e.PaymentSummary)
                .HasForeignKey<PaymentSummary>(p => p.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);
            ;
        }
        public override int SaveChanges()
        {
            StampAuditFields();
            return base.SaveChanges();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            StampAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void StampAuditFields()
        {
            var now = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        break;
                }
            }
        }
    }
}
