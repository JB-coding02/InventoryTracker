using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTracker.Data.Migrations
{
	/// <inheritdoc />
	public partial class ProfilePageUpdate : Migration
	{
		/// <inheritdoc />
		protected override void Up (MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Products_WholesalerAccounts_WholesalerId",
				table: "Products");

			migrationBuilder.AlterColumn<int>(
				name: "WholesalerId",
				table: "Products",
				type: "int",
				nullable: true,
				oldClrType: typeof(int),
				oldType: "int");

			migrationBuilder.AddForeignKey(
				name: "FK_Products_WholesalerAccounts_WholesalerId",
				table: "Products",
				column: "WholesalerId",
				principalTable: "WholesalerAccounts",
				principalColumn: "WholesalerId");
		}

		/// <inheritdoc />
		protected override void Down (MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Products_WholesalerAccounts_WholesalerId",
				table: "Products");

			migrationBuilder.AlterColumn<int>(
				name: "WholesalerId",
				table: "Products",
				type: "int",
				nullable: false,
				defaultValue: 0,
				oldClrType: typeof(int),
				oldType: "int",
				oldNullable: true);

			migrationBuilder.AddForeignKey(
				name: "FK_Products_WholesalerAccounts_WholesalerId",
				table: "Products",
				column: "WholesalerId",
				principalTable: "WholesalerAccounts",
				principalColumn: "WholesalerId",
				onDelete: ReferentialAction.Cascade);
		}
	}
}