using iCreditoApi.Modules.Transactions.Domain.Entities;
using iCreditoApi.Modules.Transactions.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iCreditoApi.Modules.Transactions.Infrastructure.Persistence;

/// <summary>
/// Configuración de Entity Framework para la entidad Transaction
/// </summary>
public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.CreditCardId)
            .IsRequired();

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.BalanceBefore)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.BalanceAfter)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.MerchantName)
            .HasMaxLength(100);

        builder.Property(t => t.Category)
            .HasMaxLength(50);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        // Índices
        builder.HasIndex(t => t.UserId);
        builder.HasIndex(t => t.CreditCardId);
        builder.HasIndex(t => t.PaymentId);
        builder.HasIndex(t => new { t.UserId, t.CreatedAt });
        builder.HasIndex(t => new { t.CreditCardId, t.CreatedAt });
        builder.HasIndex(t => new { t.UserId, t.Type });
    }
}
