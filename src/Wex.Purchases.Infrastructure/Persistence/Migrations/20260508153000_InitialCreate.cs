using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wex.Purchases.Infrastructure.Persistence.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "purchase_transactions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                description = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                transaction_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                amount_usd = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_purchase_transactions", x => x.id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "ix_purchase_transactions_description",
            table: "purchase_transactions",
            column: "description");

        migrationBuilder.CreateIndex(
            name: "ix_purchase_transactions_transaction_date",
            table: "purchase_transactions",
            column: "transaction_date");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "purchase_transactions");
    }
}
