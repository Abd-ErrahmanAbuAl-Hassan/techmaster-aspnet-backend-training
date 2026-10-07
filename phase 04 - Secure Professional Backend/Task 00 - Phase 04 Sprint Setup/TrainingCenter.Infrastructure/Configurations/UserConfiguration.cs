using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(s => s.Email).IsUnique();
            builder.HasIndex(s => s.PhoneNumber).IsUnique();

            builder.Property(s => s.PhoneNumber).HasMaxLength(11).IsRequired();
            builder.Property(s => s.FName).HasColumnName("First Name").HasMaxLength(20).IsRequired();
            builder.Property(s => s.LName).HasColumnName("Last Name").HasMaxLength(20).IsRequired();
            builder.Property(s => s.PasswordHashed).HasMaxLength(256).IsRequired();
            builder.Property(s => s.Email).HasMaxLength(50).IsRequired();
        }
    }
}
