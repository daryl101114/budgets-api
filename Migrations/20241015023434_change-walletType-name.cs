using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace budget_api.Migrations
{
    /// <inheritdoc />
    public partial class changewalletTypename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_WalletTypes_WalletTypesId",
                table: "Wallets");

            migrationBuilder.DropTable(
                name: "WalletTypes");

            migrationBuilder.RenameColumn(
                name: "WalletTypesId",
                table: "Wallets",
                newName: "WalletTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Wallets_WalletTypesId",
                table: "Wallets",
                newName: "IX_Wallets_WalletTypeId");

            migrationBuilder.CreateTable(
                name: "WalletType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletType", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_WalletType_WalletTypeId",
                table: "Wallets",
                column: "WalletTypeId",
                principalTable: "WalletType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_WalletType_WalletTypeId",
                table: "Wallets");

            migrationBuilder.DropTable(
                name: "WalletType");

            migrationBuilder.RenameColumn(
                name: "WalletTypeId",
                table: "Wallets",
                newName: "WalletTypesId");

            migrationBuilder.RenameIndex(
                name: "IX_Wallets_WalletTypeId",
                table: "Wallets",
                newName: "IX_Wallets_WalletTypesId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_WalletTypes_WalletTypesId",
                table: "Wallets",
                column: "WalletTypesId",
                principalTable: "WalletTypes",
                principalColumn: "Id");
        }
    }
}
