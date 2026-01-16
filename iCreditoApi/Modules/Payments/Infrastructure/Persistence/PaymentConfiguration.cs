using iCreditoApi.Modules.Payments.Domain.Entities;
using iCreditoApi.Modules.Payments.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iCreditoApi.Modules.Payments.Infrastructure.Persistence;

/// <summary>
/// Configuración de Entity Framework para la entidad Payment
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.CreditCardId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(p => p.Reference)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.MerchantName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.MerchantCategory)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.FailureReason)
            .HasMaxLength(500);

        builder.Property(p => p.AuthorizationCode)
            .HasMaxLength(50);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        // Índices
        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.CreditCardId);
        builder.HasIndex(p => p.Reference)
            .IsUnique();
        builder.HasIndex(p => new { p.UserId, p.Status });
        builder.HasIndex(p => new { p.UserId, p.CreatedAt });
    }
}
