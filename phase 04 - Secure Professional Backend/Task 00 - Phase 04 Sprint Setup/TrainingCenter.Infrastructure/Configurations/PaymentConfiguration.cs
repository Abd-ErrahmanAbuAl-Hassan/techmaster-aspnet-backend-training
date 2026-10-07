using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingCenter.Domain.Entities;

namespace TrainingCenter.Infrastructure.Configurations
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {

        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasIndex(s => s.ReferenceNumber).IsUnique();

            builder.Property(s => s.ReferenceNumber).IsRequired();
            builder.Property(s => s.Notes).HasMaxLength(150).IsRequired(false);
            builder.Property(t => t.Amount).HasPrecision(18, 2);

            builder.ToTable(t => t.HasCheckConstraint("CK_Payments_Amount_Positive", "[Amount] > 0"));
        }
    }
}
