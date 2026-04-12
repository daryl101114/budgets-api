using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace budget_api.Migrations
{
    /// <inheritdoc />
    public partial class constrainiconcolorandseeddefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Truncate any existing values that exceed the new column limits
            migrationBuilder.Sql("UPDATE TransactionCategory SET Icon = LEFT(Icon, 50) WHERE LEN(Icon) > 50");
            migrationBuilder.Sql("UPDATE TransactionCategory SET Color = LEFT(Color, 7) WHERE LEN(Color) > 7");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "TransactionCategory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "TransactionCategory",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // ── Spending Buckets ────────────────────────────────────────────
            migrationBuilder.Sql(@"
                UPDATE SpendingBuckets
                SET BucketName = 'Other'
                WHERE Id = '00000000-0000-0000-0000-000000000001'");

            migrationBuilder.Sql(@"
                INSERT INTO SpendingBuckets (Id, BucketName) VALUES
                ('00000000-0000-0000-0000-000000000002', 'Housing'),
                ('00000000-0000-0000-0000-000000000003', 'Transportation'),
                ('00000000-0000-0000-0000-000000000004', 'Lifestyle'),
                ('00000000-0000-0000-0000-000000000005', 'Debt'),
                ('00000000-0000-0000-0000-000000000006', 'Savings & Investments'),
                ('00000000-0000-0000-0000-000000000007', 'Income')");

            // ── Default Transaction Categories (system: UserId = NULL) ──────
            migrationBuilder.Sql(@"
                INSERT INTO TransactionCategory (Id, BucketId, UserId, CategoryName, IsFixed, Icon, Color, CreatedAt, DeletedAt) VALUES
                (NEWID(), '00000000-0000-0000-0000-000000000002', NULL, 'Rent / Mortgage',  1, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000002', NULL, 'Utilities',         1, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000002', NULL, 'Home Insurance',    1, NULL, NULL, GETUTCDATE(), NULL)");

            migrationBuilder.Sql(@"
                INSERT INTO TransactionCategory (Id, BucketId, UserId, CategoryName, IsFixed, Icon, Color, CreatedAt, DeletedAt) VALUES
                (NEWID(), '00000000-0000-0000-0000-000000000003', NULL, 'Car Payment',       1, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000003', NULL, 'Gas',               0, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000003', NULL, 'Auto Insurance',    1, NULL, NULL, GETUTCDATE(), NULL)");

            migrationBuilder.Sql(@"
                INSERT INTO TransactionCategory (Id, BucketId, UserId, CategoryName, IsFixed, Icon, Color, CreatedAt, DeletedAt) VALUES
                (NEWID(), '00000000-0000-0000-0000-000000000004', NULL, 'Groceries',            0, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000004', NULL, 'Dining Out',           0, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000004', NULL, 'Entertainment',        0, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000004', NULL, 'Streaming Services',   1, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000004', NULL, 'Gym & Fitness',        1, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000004', NULL, 'Personal Care',        0, NULL, NULL, GETUTCDATE(), NULL)");

            migrationBuilder.Sql(@"
                INSERT INTO TransactionCategory (Id, BucketId, UserId, CategoryName, IsFixed, Icon, Color, CreatedAt, DeletedAt) VALUES
                (NEWID(), '00000000-0000-0000-0000-000000000005', NULL, 'Credit Card Payment',  0, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000005', NULL, 'Student Loan',         1, NULL, NULL, GETUTCDATE(), NULL)");

            migrationBuilder.Sql(@"
                INSERT INTO TransactionCategory (Id, BucketId, UserId, CategoryName, IsFixed, Icon, Color, CreatedAt, DeletedAt) VALUES
                (NEWID(), '00000000-0000-0000-0000-000000000006', NULL, 'Savings Transfer',     0, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000006', NULL, 'Investment Deposit',   0, NULL, NULL, GETUTCDATE(), NULL)");

            migrationBuilder.Sql(@"
                INSERT INTO TransactionCategory (Id, BucketId, UserId, CategoryName, IsFixed, Icon, Color, CreatedAt, DeletedAt) VALUES
                (NEWID(), '00000000-0000-0000-0000-000000000007', NULL, 'Salary',               1, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000007', NULL, 'Freelance',            0, NULL, NULL, GETUTCDATE(), NULL)");

            migrationBuilder.Sql(@"
                INSERT INTO TransactionCategory (Id, BucketId, UserId, CategoryName, IsFixed, Icon, Color, CreatedAt, DeletedAt) VALUES
                (NEWID(), '00000000-0000-0000-0000-000000000001', NULL, 'Remittance',           0, NULL, NULL, GETUTCDATE(), NULL),
                (NEWID(), '00000000-0000-0000-0000-000000000001', NULL, 'Peer Payment',         0, NULL, NULL, GETUTCDATE(), NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM TransactionCategory
                WHERE UserId IS NULL
                AND CategoryName IN (
                    'Rent / Mortgage', 'Utilities', 'Home Insurance',
                    'Car Payment', 'Gas', 'Auto Insurance',
                    'Groceries', 'Dining Out', 'Entertainment', 'Streaming Services', 'Gym & Fitness', 'Personal Care',
                    'Credit Card Payment', 'Student Loan',
                    'Savings Transfer', 'Investment Deposit',
                    'Salary', 'Freelance',
                    'Remittance', 'Peer Payment'
                )");

            migrationBuilder.Sql(@"
                DELETE FROM SpendingBuckets
                WHERE Id IN (
                    '00000000-0000-0000-0000-000000000002',
                    '00000000-0000-0000-0000-000000000003',
                    '00000000-0000-0000-0000-000000000004',
                    '00000000-0000-0000-0000-000000000005',
                    '00000000-0000-0000-0000-000000000006',
                    '00000000-0000-0000-0000-000000000007'
                )");

            migrationBuilder.Sql(@"
                UPDATE SpendingBuckets
                SET BucketName = 'General'
                WHERE Id = '00000000-0000-0000-0000-000000000001'");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "TransactionCategory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "TransactionCategory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7,
                oldNullable: true);
        }
    }
}
