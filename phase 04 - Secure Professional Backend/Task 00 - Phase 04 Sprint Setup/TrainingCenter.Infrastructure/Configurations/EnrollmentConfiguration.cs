using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    internal class EnrollmentConfiguration:IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasIndex(e => new
            {
                e.StudentId,
                e.TrainingTrackId
            }).IsUnique()
                 .HasFilter("[Status] <> 3");

            builder.ToTable(t => t.HasCheckConstraint("CK_Enrollments_ProgressPercentage", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100"));
            builder.ToTable(t => t.HasCheckConstraint("CK_Enrollments_FinalGrade", "[FinalGrade] >= 0 AND [FinalGrade] <= 100"));


            builder.HasMany(t => t.Payments)
                  .WithOne(e => e.Enrollment)
                  .HasForeignKey(f => f.EnrollmentId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired();
        }
    }
}
