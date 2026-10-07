using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    internal class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {

        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.Property(s => s.Bio).HasMaxLength(250).IsRequired(false);
            builder.Property(s => s.Specialization).HasMaxLength(50).IsRequired(false);

            builder.HasMany(s => s.TrainingTracks)
                          .WithOne(e => e.Instructor)
                          .HasForeignKey(f => f.InstructorId)
                          .OnDelete(DeleteBehavior.Restrict)
                          .IsRequired();
        }
    }
}
