using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace budget_api.Migrations
{
    /// <inheritdoc />
    public partial class addwalletimportprofile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WalletImportProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateColumn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AmountColumn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeColumn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionColumn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawDescriptionColumn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankTxnTypeColumn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CheckNumberColumn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BalanceAfterColumn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncomeTypeValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpenseTypeValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletImportProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletImportProfiles_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletImportProfiles_WalletId",
                table: "WalletImportProfiles",
                column: "WalletId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletImportProfiles");
        }
    }
}
