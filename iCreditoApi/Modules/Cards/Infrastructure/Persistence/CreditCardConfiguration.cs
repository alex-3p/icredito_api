using iCreditoApi.Modules.Cards.Domain.Entities;
using iCreditoApi.Modules.Cards.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iCreditoApi.Modules.Cards.Infrastructure.Persistence;

/// <summary>
/// Configuración de Entity Framework para la entidad CreditCard
/// </summary>
public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
{
    public void Configure(EntityTypeBuilder<CreditCard> builder)
    {
        builder.ToTable("CreditCards");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.CardNumber)
            .IsRequired()
            .HasMaxLength(19);

        builder.Property(c => c.CardholderName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.ExpirationMonth)
            .IsRequired();

        builder.Property(c => c.ExpirationYear)
            .IsRequired();

        builder.Property(c => c.CVV)
            .IsRequired()
            .HasMaxLength(4);

        builder.Property(c => c.CreditLimit)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.CurrentBalance)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.Brand)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.Alias)
            .HasMaxLength(50);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // Índices
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.CardNumber)
            .IsUnique();
        builder.HasIndex(c => new { c.UserId, c.Status });
    }
}
