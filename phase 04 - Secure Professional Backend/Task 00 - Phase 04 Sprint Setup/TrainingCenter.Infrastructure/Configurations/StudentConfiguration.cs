using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    internal class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasMany(s => s.Enrollments)
                       .WithOne(e => e.Student)
                       .HasForeignKey(f => f.StudentId)
                       .OnDelete(DeleteBehavior.Restrict)
                       .IsRequired();

        }
    }
}
