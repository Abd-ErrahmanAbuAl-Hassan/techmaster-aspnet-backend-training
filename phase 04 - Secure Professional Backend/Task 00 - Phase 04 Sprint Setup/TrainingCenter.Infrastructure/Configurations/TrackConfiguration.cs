using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    internal class TrackConfiguration : IEntityTypeConfiguration<TrainingTrack>
    {
        public void Configure(EntityTypeBuilder<TrainingTrack> builder)
        {
            builder.HasIndex(s => s.Code).IsUnique();

            builder.Property(s => s.Title).HasMaxLength(50).IsRequired();
            builder.Property(s => s.Code).HasMaxLength(8).IsRequired();
            builder.Property(s => s.Description).HasMaxLength(250).IsRequired();
            builder.Property(t => t.Price).HasPrecision(18, 2);

            builder.ToTable(t => t.HasCheckConstraint("CK_TrainingTracks_Price_Positive", "[Price] > 0"));
            builder.ToTable(t => t.HasCheckConstraint("CK_TrainingTracks_Capacity_Positive", "[Capacity] > 0"));
            builder.ToTable(t => t.HasCheckConstraint("CK_TrainingTracks_DateRange", "[EndDate] > [StartDate]"));

            builder.HasMany(t => t.Enrollments)
                  .WithOne(e => e.TrainingTrack)
                  .HasForeignKey(f => f.TrainingTrackId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired();
        }
    }
}
