using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wex.Purchases.Domain.Entities;

namespace Wex.Purchases.Infrastructure.MySql.Persistence.Configurations;

public sealed class PurchaseTransactionConfiguration : IEntityTypeConfiguration<PurchaseTransaction>
{
    public void Configure(EntityTypeBuilder<PurchaseTransaction> builder)
    {
        builder.ToTable("purchase_transactions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TransactionDate)
            .HasColumnName("transaction_date")
            .IsRequired();

        builder.Property(x => x.AmountUsd)
            .HasColumnName("amount_usd")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasIndex(x => x.TransactionDate)
            .HasDatabaseName("ix_purchase_transactions_transaction_date");

        builder.HasIndex(x => x.Description)
            .HasDatabaseName("ix_purchase_transactions_description");
    }
}
