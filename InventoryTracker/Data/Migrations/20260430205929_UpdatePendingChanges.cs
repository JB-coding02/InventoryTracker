using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTracker.Data.Migrations;

/// <inheritdoc />
public partial class UpdatePendingChanges : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Products_ManufacturerAccounts_ManufacturerId",
            table: "Products");

        migrationBuilder.DropForeignKey(
            name: "FK_Products_WholesalerAccounts_WholesalerId",
            table: "Products");

        migrationBuilder.DropForeignKey(
            name: "FK_WholesalerAccounts_AspNetUsers_AppUserId",
            table: "WholesalerAccounts");

        migrationBuilder.DropIndex(
            name: "IX_WholesalerAccounts_AppUserId",
            table: "WholesalerAccounts");

        migrationBuilder.DropIndex(
            name: "IX_ManufacturerAccounts_AppUserId",
            table: "ManufacturerAccounts");

        migrationBuilder.RenameColumn(
            name: "WholesalerId",
            table: "Products",
            newName: "ManufacturerAccountManufacturerId");

        migrationBuilder.RenameColumn(
            name: "ManufacturerId",
            table: "Products",
            newName: "UserAccountId");

        migrationBuilder.RenameIndex(
            name: "IX_Products_WholesalerId",
            table: "Products",
            newName: "IX_Products_ManufacturerAccountManufacturerId");

        migrationBuilder.RenameIndex(
            name: "IX_Products_ManufacturerId",
            table: "Products",
            newName: "IX_Products_UserAccountId");

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

        migrationBuilder.CreateIndex(
            name: "IX_WholesalerAccounts_AppUserId",
            table: "WholesalerAccounts",
            column: "AppUserId");

        migrationBuilder.CreateIndex(
            name: "IX_ManufacturerAccounts_AppUserId",
            table: "ManufacturerAccounts",
            column: "AppUserId");

        migrationBuilder.CreateIndex(
            name: "IX_UserAccounts_AccountEmail",
            table: "UserAccounts",
            column: "AccountEmail",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_UserAccounts_AccountName",
            table: "UserAccounts",
            column: "AccountName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_UserAccounts_AppUserId",
            table: "UserAccounts",
            column: "AppUserId",
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "FK_Products_ManufacturerAccounts_ManufacturerAccountManufacturerId",
            table: "Products",
            column: "ManufacturerAccountManufacturerId",
            principalTable: "ManufacturerAccounts",
            principalColumn: "ManufacturerId");

        migrationBuilder.AddForeignKey(
            name: "FK_Products_UserAccounts_UserAccountId",
            table: "Products",
            column: "UserAccountId",
            principalTable: "UserAccounts",
            principalColumn: "UserAccountId",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_WholesalerAccounts_AspNetUsers_AppUserId",
            table: "WholesalerAccounts",
            column: "AppUserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Products_ManufacturerAccounts_ManufacturerAccountManufacturerId",
            table: "Products");

        migrationBuilder.DropForeignKey(
            name: "FK_Products_UserAccounts_UserAccountId",
            table: "Products");

        migrationBuilder.DropForeignKey(
            name: "FK_WholesalerAccounts_AspNetUsers_AppUserId",
            table: "WholesalerAccounts");

        migrationBuilder.DropTable(
            name: "UserAccounts");

        migrationBuilder.DropIndex(
            name: "IX_WholesalerAccounts_AppUserId",
            table: "WholesalerAccounts");

        migrationBuilder.DropIndex(
            name: "IX_ManufacturerAccounts_AppUserId",
            table: "ManufacturerAccounts");

        migrationBuilder.RenameColumn(
            name: "UserAccountId",
            table: "Products",
            newName: "ManufacturerId");

        migrationBuilder.RenameColumn(
            name: "ManufacturerAccountManufacturerId",
            table: "Products",
            newName: "WholesalerId");

        migrationBuilder.RenameIndex(
            name: "IX_Products_UserAccountId",
            table: "Products",
            newName: "IX_Products_ManufacturerId");

        migrationBuilder.RenameIndex(
            name: "IX_Products_ManufacturerAccountManufacturerId",
            table: "Products",
            newName: "IX_Products_WholesalerId");

        migrationBuilder.CreateIndex(
            name: "IX_WholesalerAccounts_AppUserId",
            table: "WholesalerAccounts",
            column: "AppUserId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ManufacturerAccounts_AppUserId",
            table: "ManufacturerAccounts",
            column: "AppUserId",
            unique: true);

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
            principalColumn: "WholesalerId");

        migrationBuilder.AddForeignKey(
            name: "FK_WholesalerAccounts_AspNetUsers_AppUserId",
            table: "WholesalerAccounts",
            column: "AppUserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id");
    }
}