using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "WholesalerAccounts");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "ManufacturerAccounts");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "WholesalerAccounts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "WholesalerId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "ManufacturerAccounts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

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
                name: "IX_Products_WholesalerId",
                table: "Products",
                column: "WholesalerId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ManufacturerAccounts_AspNetUsers_AppUserId",
                table: "ManufacturerAccounts",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_WholesalerAccounts_WholesalerId",
                table: "Products",
                column: "WholesalerId",
                principalTable: "WholesalerAccounts",
                principalColumn: "WholesalerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WholesalerAccounts_AspNetUsers_AppUserId",
                table: "WholesalerAccounts",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
               onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManufacturerAccounts_AspNetUsers_AppUserId",
                table: "ManufacturerAccounts");

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
                name: "IX_WholesalerAccounts_WholesalerEmail",
                table: "WholesalerAccounts");

            migrationBuilder.DropIndex(
                name: "IX_Products_WholesalerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ManufacturerAccounts_AppUserId",
                table: "ManufacturerAccounts");

            migrationBuilder.DropIndex(
                name: "IX_ManufacturerAccounts_ManufacturerEmail",
                table: "ManufacturerAccounts");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "WholesalerAccounts");

            migrationBuilder.DropColumn(
                name: "WholesalerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "ManufacturerAccounts");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "WholesalerAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "ManufacturerAccounts",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");
        }
    }
}
