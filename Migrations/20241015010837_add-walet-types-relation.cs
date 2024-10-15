using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace budget_api.Migrations
{
    /// <inheritdoc />
    public partial class addwalettypesrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Wallets");

            migrationBuilder.AddColumn<int>(
                name: "WalletTypesId",
                table: "Wallets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WalletTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_WalletTypesId",
                table: "Wallets",
                column: "WalletTypesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_WalletTypes_WalletTypesId",
                table: "Wallets",
                column: "WalletTypesId",
                principalTable: "WalletTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_WalletTypes_WalletTypesId",
                table: "Wallets");

            migrationBuilder.DropTable(
                name: "WalletTypes");

            migrationBuilder.DropIndex(
                name: "IX_Wallets_WalletTypesId",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "WalletTypesId",
                table: "Wallets");

            migrationBuilder.AddColumn<string>(
                name: "AccountType",
                table: "Wallets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
