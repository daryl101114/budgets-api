using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace budget_api.Migrations
{
    /// <inheritdoc />
    public partial class addwallettypetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_WalletType_WalletTypeId",
                table: "Wallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletType",
                table: "WalletType");

            migrationBuilder.RenameTable(
                name: "WalletType",
                newName: "WalletTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletTypes",
                table: "WalletTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_WalletTypes_WalletTypeId",
                table: "Wallets",
                column: "WalletTypeId",
                principalTable: "WalletTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_WalletTypes_WalletTypeId",
                table: "Wallets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletTypes",
                table: "WalletTypes");

            migrationBuilder.RenameTable(
                name: "WalletTypes",
                newName: "WalletType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletType",
                table: "WalletType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_WalletType_WalletTypeId",
                table: "Wallets",
                column: "WalletTypeId",
                principalTable: "WalletType",
                principalColumn: "Id");
        }
    }
}
