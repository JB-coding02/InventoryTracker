using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTracker.Migrations
{
    /// <inheritdoc />
    public partial class AlterOrdersToUseApplicationUserIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_UserAccounts_ManufacturerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_UserAccounts_WholesalerId",
                table: "Orders");

            // Drop existing indexes
            migrationBuilder.DropIndex(
                name: "IX_Orders_ManufacturerId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_WholesalerId",
                table: "Orders");

            // Drop the existing int columns and create new string columns
            migrationBuilder.DropColumn(
                name: "ManufacturerId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WholesalerId",
                table: "Orders");

            // Add new string columns
            migrationBuilder.AddColumn<string>(
                name: "ManufacturerId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WholesalerId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            // Add new indexes for the string columns
            migrationBuilder.CreateIndex(
                name: "IX_Orders_ManufacturerId",
                table: "Orders",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WholesalerId",
                table: "Orders",
                column: "WholesalerId");

            // Add foreign key constraints to AspNetUsers
            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ManufacturerId",
                table: "Orders",
                column: "ManufacturerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_WholesalerId",
                table: "Orders",
                column: "WholesalerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign keys to AspNetUsers
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ManufacturerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_WholesalerId",
                table: "Orders");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_Orders_ManufacturerId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_WholesalerId",
                table: "Orders");

            // Drop string columns
            migrationBuilder.DropColumn(
                name: "ManufacturerId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WholesalerId",
                table: "Orders");

            // Re-add int columns
            migrationBuilder.AddColumn<int>(
                name: "ManufacturerId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WholesalerId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Re-add foreign keys to UserAccounts
            migrationBuilder.CreateIndex(
                name: "IX_Orders_ManufacturerId",
                table: "Orders",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WholesalerId",
                table: "Orders",
                column: "WholesalerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_UserAccounts_ManufacturerId",
                table: "Orders",
                column: "ManufacturerId",
                principalTable: "UserAccounts",
                principalColumn: "UserAccountId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_UserAccounts_WholesalerId",
                table: "Orders",
                column: "WholesalerId",
                principalTable: "UserAccounts",
                principalColumn: "UserAccountId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
