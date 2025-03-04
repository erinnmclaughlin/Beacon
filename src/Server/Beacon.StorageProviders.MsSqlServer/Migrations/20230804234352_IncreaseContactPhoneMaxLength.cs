using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

/// <inheritdoc />
public partial class IncreaseContactPhoneMaxLength : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "PhoneNumber",
            table: "ProjectContacts",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(20)",
            oldMaxLength: 20,
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "PhoneNumber",
            table: "ProjectContacts",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldNullable: true);
    }
}
