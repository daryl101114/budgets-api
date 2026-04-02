using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace budget_api.Migrations
{
    /// <inheritdoc />
    public partial class refactortransactioncategoryentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create SpendingBuckets table first so we can assign a default bucket to existing categories
            migrationBuilder.CreateTable(
                name: "SpendingBuckets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BucketName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpendingBuckets", x => x.Id);
                });

            // Seed a default "General" bucket for existing system categories
            migrationBuilder.Sql("INSERT INTO SpendingBuckets (Id, BucketName) VALUES ('00000000-0000-0000-0000-000000000001', 'General')");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "TransactionCategory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "TransactionCategory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "TransactionCategory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                defaultValue: null);

            // Copy existing names before dropping the old column
            migrationBuilder.Sql("UPDATE TransactionCategory SET CategoryName = TransactionCategoryName");

            migrationBuilder.DropColumn(
                name: "TransactionCategoryName",
                table: "TransactionCategory");

            // Make CategoryName non-nullable now that all rows have a value
            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "TransactionCategory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BucketId",
                table: "TransactionCategory",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000001"));

            // Assign all existing categories to the default "General" bucket
            migrationBuilder.Sql("UPDATE TransactionCategory SET BucketId = '00000000-0000-0000-0000-000000000001'");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TransactionCategory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TransactionCategory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TransactionCategory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCategory_BucketId",
                table: "TransactionCategory",
                column: "BucketId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCategory_UserId",
                table: "TransactionCategory",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionCategory_SpendingBuckets_BucketId",
                table: "TransactionCategory",
                column: "BucketId",
                principalTable: "SpendingBuckets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionCategory_Users_UserId",
                table: "TransactionCategory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionCategory_SpendingBuckets_BucketId",
                table: "TransactionCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionCategory_Users_UserId",
                table: "TransactionCategory");

            migrationBuilder.DropTable(
                name: "SpendingBuckets");

            migrationBuilder.DropIndex(
                name: "IX_TransactionCategory_BucketId",
                table: "TransactionCategory");

            migrationBuilder.DropIndex(
                name: "IX_TransactionCategory_UserId",
                table: "TransactionCategory");

            migrationBuilder.DropColumn(
                name: "BucketId",
                table: "TransactionCategory");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "TransactionCategory");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TransactionCategory");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TransactionCategory");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TransactionCategory");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "TransactionCategory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "TransactionCategory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionCategoryName",
                table: "TransactionCategory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
