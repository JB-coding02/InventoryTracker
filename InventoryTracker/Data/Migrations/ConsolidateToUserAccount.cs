using Microsoft.EntityFrameworkCore.Migrations;

namespace InventoryTracker.Data.Migrations
{
    public partial class ConsolidateToUserAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Create the new UserAccounts table
            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    UserAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountEmail = table.Column<string>(type: "nvarchar(65)", maxLength: 65, nullable: false),
                    AccountRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.UserAccountId);
                    table.ForeignKey(
                        name: "FK_UserAccounts_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Step 2: Create indexes on the new table
            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_AccountName",
                table: "UserAccounts",
                column: "AccountName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_AccountEmail",
                table: "UserAccounts",
                column: "AccountEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_AppUserId",
                table: "UserAccounts",
                column: "AppUserId",
                unique: true);

            // Step 3: Migrate data from ManufacturerAccounts to UserAccounts
            migrationBuilder.Sql(@"
                INSERT INTO UserAccounts (AccountName, AccountEmail, AppUserId, AccountRole)
                SELECT ManufacturerName, ManufacturerEmail, AppUserId, 0
                FROM ManufacturerAccounts;
            ");

            // Step 4: Migrate data from WholesalerAccounts to UserAccounts
            migrationBuilder.Sql(@"
                INSERT INTO UserAccounts (AccountName, AccountEmail, AppUserId, AccountRole)
                SELECT WholesalerName, WholesalerEmail, AppUserId, 1
                FROM WholesalerAccounts;
            ");

            // Step 5: Add the new UserAccountId column to Products table
            migrationBuilder.AddColumn<int>(
                name: "UserAccountId",
                table: "Products",
                type: "int",
                nullable: true);

            // Step 6: Migrate Products data - Map ManufacturerId to the corresponding UserAccountId
            migrationBuilder.Sql(@"
                UPDATE Products
                SET UserAccountId = (
                    SELECT UserAccountId
                    FROM UserAccounts
                    WHERE UserAccounts.AccountRole = 0
                    AND UserAccounts.AccountName = (
                        SELECT ManufacturerName
                        FROM ManufacturerAccounts
                        WHERE ManufacturerAccounts.ManufacturerId = Products.ManufacturerId
                    )
                    LIMIT 1
                )
                WHERE ManufacturerId IS NOT NULL;
            ");

            // Step 7: Migrate Products data - Map WholesalerId to the corresponding UserAccountId
            migrationBuilder.Sql(@"
                UPDATE Products
                SET UserAccountId = (
                    SELECT UserAccountId
                    FROM UserAccounts
                    WHERE UserAccounts.AccountRole = 1
                    AND UserAccounts.AccountName = (
                        SELECT WholesalerName
                        FROM WholesalerAccounts
                        WHERE WholesalerAccounts.WholesalerId = Products.WholesalerId
                    )
                    LIMIT 1
                )
                WHERE WholesalerId IS NOT NULL;
            ");

            // Step 8: Make UserAccountId non-nullable after migration
            migrationBuilder.AlterColumn<int>(
                name: "UserAccountId",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Step 9: Add foreign key constraint for UserAccountId
            migrationBuilder.AddForeignKey(
                name: "FK_Products_UserAccounts_UserAccountId",
                table: "Products",
                column: "UserAccountId",
                principalTable: "UserAccounts",
                principalColumn: "UserAccountId",
                onDelete: ReferentialAction.Cascade);

            // Step 10: Drop the old foreign key constraints and columns
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ManufacturerAccounts_ManufacturerId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_WholesalerAccounts_WholesalerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ManufacturerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_WholesalerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ManufacturerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WholesalerId",
                table: "Products");

            // Step 11: Drop the old account tables
            migrationBuilder.DropTable(
                name: "ManufacturerAccounts");

            migrationBuilder.DropTable(
                name: "WholesalerAccounts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate WholesalerAccounts table
            migrationBuilder.CreateTable(
                name: "WholesalerAccounts",
                columns: table => new
                {
                    WholesalerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WholesalerEmail = table.Column<string>(type: "nvarchar(65)", maxLength: 65, nullable: false),
                    WholesalerName = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WholesalerAccounts", x => x.WholesalerId);
                    table.ForeignKey(
                        name: "FK_WholesalerAccounts_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            // Recreate ManufacturerAccounts table
            migrationBuilder.CreateTable(
                name: "ManufacturerAccounts",
                columns: table => new
                {
                    ManufacturerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ManufacturerEmail = table.Column<string>(type: "nvarchar(65)", maxLength: 65, nullable: false),
                    ManufacturerName = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManufacturerAccounts", x => x.ManufacturerId);
                    table.ForeignKey(
                        name: "FK_ManufacturerAccounts_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Migrate data back from UserAccounts
            migrationBuilder.Sql(@"
                INSERT INTO ManufacturerAccounts (ManufacturerName, ManufacturerEmail, AppUserId)
                SELECT AccountName, AccountEmail, AppUserId
                FROM UserAccounts
                WHERE AccountRole = 0;
            ");

            migrationBuilder.Sql(@"
                INSERT INTO WholesalerAccounts (WholesalerName, WholesalerEmail, AppUserId)
                SELECT AccountName, AccountEmail, AppUserId
                FROM UserAccounts
                WHERE AccountRole = 1;
            ");

            // Recreate old Product columns and relationships
            migrationBuilder.AddColumn<int>(
                name: "ManufacturerId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WholesalerId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Products
                SET ManufacturerId = (
                    SELECT ManufacturerId
                    FROM ManufacturerAccounts
                    WHERE ManufacturerAccounts.ManufacturerName = (
                        SELECT AccountName
                        FROM UserAccounts
                        WHERE UserAccounts.UserAccountId = Products.UserAccountId
                    )
                    LIMIT 1
                )
                WHERE UserAccountId IN (
                    SELECT UserAccountId FROM UserAccounts WHERE AccountRole = 0
                );
            ");

            migrationBuilder.Sql(@"
                UPDATE Products
                SET WholesalerId = (
                    SELECT WholesalerId
                    FROM WholesalerAccounts
                    WHERE WholesalerAccounts.WholesalerName = (
                        SELECT AccountName
                        FROM UserAccounts
                        WHERE UserAccounts.UserAccountId = Products.UserAccountId
                    )
                    LIMIT 1
                )
                WHERE UserAccountId IN (
                    SELECT UserAccountId FROM UserAccounts WHERE AccountRole = 1
                );
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_UserAccounts_UserAccountId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_UserAccountId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserAccountId",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ManufacturerId",
                table: "Products",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_WholesalerId",
                table: "Products",
                column: "WholesalerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ManufacturerAccounts_ManufacturerId",
                table: "Products",
                column: "ManufacturerId",
                principalTable: "ManufacturerAccounts",
                principalColumn: "ManufacturerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_WholesalerAccounts_WholesalerId",
                table: "Products",
                column: "WholesalerId",
                principalTable: "WholesalerAccounts",
                principalColumn: "WholesalerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturerAccounts_AppUserId",
                table: "ManufacturerAccounts",
                column: "AppUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturerAccounts_ManufacturerEmail",
                table: "ManufacturerAccounts",
                column: "ManufacturerEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ManufacturerAccounts_ManufacturerName",
                table: "ManufacturerAccounts",
                column: "ManufacturerName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WholesalerAccounts_AppUserId",
                table: "WholesalerAccounts",
                column: "AppUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WholesalerAccounts_WholesalerEmail",
                table: "WholesalerAccounts",
                column: "WholesalerEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WholesalerAccounts_WholesalerName",
                table: "WholesalerAccounts",
                column: "WholesalerName",
                unique: true);

            // Drop UserAccounts table
            migrationBuilder.DropTable(
                name: "UserAccounts");
        }
    }
}
