using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    internal class TrackSessionConfiguration : IEntityTypeConfiguration<TrackSession>
    {
        public void Configure(EntityTypeBuilder<TrackSession> builder)
        {

            builder.Property(s => s.Title).HasMaxLength(50).IsRequired();
            builder.Property(s => s.Description).HasMaxLength(250).IsRequired();
            builder.Property(t => t.MeetingLink).IsRequired();
            builder.Property(t => t.SessionDate).IsRequired();
            builder.Property(t => t.CreatedByInstructorId).HasColumnName("CreatedBy").IsRequired();


            builder.HasOne(t => t.Track)
                  .WithMany(e => e.Sessions)
                  .HasForeignKey(f => f.TrackId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired();

            builder.HasOne(s=>s.Instructor)
                   .WithMany(i=>i.Sessions)
                   .HasForeignKey(s=>s.CreatedByInstructorId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();
        }
    }
}
