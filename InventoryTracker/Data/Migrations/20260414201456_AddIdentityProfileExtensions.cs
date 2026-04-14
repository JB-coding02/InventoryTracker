using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTracker.Data.Migrations;

/// <inheritdoc />

public partial class AddIdentityProfileExtensions : Migration
{
	/// <inheritdoc />
	protected override void Up (MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AlterColumn<string>(
			name: "Name",
			table: "Products",
			type: "nvarchar(100)",
			maxLength: 100,
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(max)");

		migrationBuilder.AddColumn<string>(
			name: "AppUserId",
			table: "WholesalerAccounts",
			type: "nvarchar(450)",
			nullable: true);

		migrationBuilder.AddColumn<int>(
			name: "WholesalerId",
			table: "Products",
			type: "int",
			nullable: true);

		migrationBuilder.AddColumn<string>(
			name: "AppUserId",
			table: "ManufacturerAccounts",
			type: "nvarchar(450)",
			nullable: true);

		migrationBuilder.Sql("""
			IF EXISTS (
				SELECT ManufacturerEmail
				FROM ManufacturerAccounts
				GROUP BY ManufacturerEmail
				HAVING COUNT(*) > 1
			)
				THROW 51000, 'Cannot apply unique index on ManufacturerEmail because duplicate values exist.', 1;

			IF EXISTS (
				SELECT WholesalerEmail
				FROM WholesalerAccounts
				GROUP BY WholesalerEmail
				HAVING COUNT(*) > 1
			)
				THROW 51001, 'Cannot apply unique index on WholesalerEmail because duplicate values exist.', 1;
			""");

		migrationBuilder.Sql("""
			INSERT INTO AspNetUsers
			(
				Id, UserName, NormalizedUserName, Email, NormalizedEmail,
				EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
				PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount
			)
			SELECT
				CONVERT(nvarchar(450), NEWID()),
				CONCAT('mfg_profile_', CAST(m.ManufacturerId AS nvarchar(20))),
				UPPER(CONCAT('mfg_profile_', CAST(m.ManufacturerId AS nvarchar(20)))),
				m.ManufacturerEmail,
				UPPER(m.ManufacturerEmail),
				1, NULL, CONVERT(nvarchar(36), NEWID()), CONVERT(nvarchar(36), NEWID()),
				NULL, 0, 0, NULL, 0, 0
			FROM ManufacturerAccounts m
			WHERE m.AppUserId IS NULL
			  AND NOT EXISTS (
				  SELECT 1
				  FROM AspNetUsers u
				  WHERE u.NormalizedUserName = UPPER(CONCAT('mfg_profile_', CAST(m.ManufacturerId AS nvarchar(20))))
			  );

			UPDATE m
			SET m.AppUserId = u.Id
			FROM ManufacturerAccounts m
			INNER JOIN AspNetUsers u
				ON u.NormalizedUserName = UPPER(CONCAT('mfg_profile_', CAST(m.ManufacturerId AS nvarchar(20))))
			WHERE m.AppUserId IS NULL;
			""");

		migrationBuilder.Sql("""
			INSERT INTO AspNetUsers
			(
				Id, UserName, NormalizedUserName, Email, NormalizedEmail,
				EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
				PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount
			)
			SELECT
				CONVERT(nvarchar(450), NEWID()),
				CONCAT('wh_profile_', CAST(w.WholesalerId AS nvarchar(20))),
				UPPER(CONCAT('wh_profile_', CAST(w.WholesalerId AS nvarchar(20)))),
				w.WholesalerEmail,
				UPPER(w.WholesalerEmail),
				1, NULL, CONVERT(nvarchar(36), NEWID()), CONVERT(nvarchar(36), NEWID()),
				NULL, 0, 0, NULL, 0, 0
			FROM WholesalerAccounts w
			WHERE w.AppUserId IS NULL
			  AND NOT EXISTS (
				  SELECT 1
				  FROM AspNetUsers u
				  WHERE u.NormalizedUserName = UPPER(CONCAT('wh_profile_', CAST(w.WholesalerId AS nvarchar(20))))
			  );

			UPDATE w
			SET w.AppUserId = u.Id
			FROM WholesalerAccounts w
			INNER JOIN AspNetUsers u
				ON u.NormalizedUserName = UPPER(CONCAT('wh_profile_', CAST(w.WholesalerId AS nvarchar(20))))
			WHERE w.AppUserId IS NULL;
			""");

		migrationBuilder.Sql("""
			DECLARE @DefaultWholesalerId int;

			SELECT TOP(1) @DefaultWholesalerId = WholesalerId
			FROM WholesalerAccounts
			WHERE WholesalerName = 'SystemWholesaler';

			IF @DefaultWholesalerId IS NULL
			BEGIN
				DECLARE @SystemUserId nvarchar(450);
				DECLARE @SystemEmail nvarchar(256);

				SELECT TOP(1) @SystemUserId = Id
				FROM AspNetUsers
				WHERE NormalizedUserName = 'SYSTEM_WHOLESALER_PROFILE';

				IF @SystemUserId IS NULL
				BEGIN
					SET @SystemUserId = CONVERT(nvarchar(450), NEWID());
					SET @SystemEmail = LOWER(CONCAT('system+', REPLACE(CONVERT(nvarchar(36), NEWID()), '-', ''), '@inv.local'));

					INSERT INTO AspNetUsers
					(
						Id, UserName, NormalizedUserName, Email, NormalizedEmail,
						EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
						PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, LockoutEnabled, AccessFailedCount
					)
					VALUES
					(
						@SystemUserId, 'system_wholesaler_profile', 'SYSTEM_WHOLESALER_PROFILE', @SystemEmail, UPPER(@SystemEmail),
						1, NULL, CONVERT(nvarchar(36), NEWID()), CONVERT(nvarchar(36), NEWID()),
						NULL, 0, 0, NULL, 0, 0
					);
				END

				IF NOT EXISTS (SELECT 1 FROM WholesalerAccounts WHERE AppUserId = @SystemUserId)
				BEGIN
					INSERT INTO WholesalerAccounts (WholesalerName, WholesalerEmail, AppUserId)
					VALUES ('SystemWholesaler', LEFT(LOWER(CONCAT('system-wholesaler+', REPLACE(CONVERT(nvarchar(36), NEWID()), '-', ''), '@inv.local')), 65), @SystemUserId);

					SET @DefaultWholesalerId = CAST(SCOPE_IDENTITY() AS int);
				END
				ELSE
				BEGIN
					SELECT TOP(1) @DefaultWholesalerId = WholesalerId
					FROM WholesalerAccounts
					WHERE AppUserId = @SystemUserId;
				END
			END

			UPDATE Products
			SET WholesalerId = @DefaultWholesalerId
			WHERE WholesalerId IS NULL;
			""");

		migrationBuilder.AlterColumn<string>(
			name: "AppUserId",
			table: "WholesalerAccounts",
			type: "nvarchar(450)",
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(450)",
			oldNullable: true);

		migrationBuilder.AlterColumn<int>(
			name: "WholesalerId",
			table: "Products",
			type: "int",
			nullable: false,
			oldClrType: typeof(int),
			oldType: "int",
			oldNullable: true);

		migrationBuilder.AlterColumn<string>(
			name: "AppUserId",
			table: "ManufacturerAccounts",
			type: "nvarchar(450)",
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(450)",
			oldNullable: true);

		migrationBuilder.DropColumn(
			name: "PasswordHash",
			table: "WholesalerAccounts");

		migrationBuilder.DropColumn(
			name: "Password",
			table: "ManufacturerAccounts");

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
			onDelete: ReferentialAction.Cascade);
	}

	/// <inheritdoc />

	protected override void Down (MigrationBuilder migrationBuilder)
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

		migrationBuilder.AddColumn<string>(
			name: "PasswordHash",
			table: "WholesalerAccounts",
			type: "nvarchar(max)",
			nullable: true);

		migrationBuilder.AddColumn<string>(
			name: "Password",
			table: "ManufacturerAccounts",
			type: "nvarchar(25)",
			maxLength: 25,
			nullable: true);

		migrationBuilder.Sql("""
			UPDATE w
			SET w.PasswordHash = ISNULL(u.PasswordHash, '')
			FROM WholesalerAccounts w
			LEFT JOIN AspNetUsers u ON u.Id = w.AppUserId
			WHERE w.PasswordHash IS NULL;

			UPDATE ManufacturerAccounts
			SET Password = ''
			WHERE Password IS NULL;
			""");

		migrationBuilder.AlterColumn<string>(
			name: "PasswordHash",
			table: "WholesalerAccounts",
			type: "nvarchar(max)",
			nullable: false,
			defaultValue: "",
			oldClrType: typeof(string),
			oldType: "nvarchar(max)",
			oldNullable: true);

		migrationBuilder.AlterColumn<string>(
			name: "Password",
			table: "ManufacturerAccounts",
			type: "nvarchar(25)",
			maxLength: 25,
			nullable: false,
			defaultValue: "",
			oldClrType: typeof(string),
			oldType: "nvarchar(25)",
			oldMaxLength: 25,
			oldNullable: true);

		migrationBuilder.DropColumn(
			name: "AppUserId",
			table: "WholesalerAccounts");

		migrationBuilder.DropColumn(
			name: "WholesalerId",
			table: "Products");

		migrationBuilder.DropColumn(
			name: "AppUserId",
			table: "ManufacturerAccounts");

		migrationBuilder.AlterColumn<string>(
			name: "Name",
			table: "Products",
			type: "nvarchar(max)",
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(100)",
			oldMaxLength: 100);
	}
}