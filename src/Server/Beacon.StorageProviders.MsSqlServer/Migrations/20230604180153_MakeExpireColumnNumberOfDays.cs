using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

/// <inheritdoc />
public partial class MakeExpireColumnNumberOfDays : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ExpirationTimeSpan",
            table: "LaboratoryInvitations");

        migrationBuilder.AddColumn<double>(
            name: "ExpireAfterDays",
            table: "LaboratoryInvitations",
            type: "float",
            nullable: false,
            defaultValue: 0.0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ExpireAfterDays",
            table: "LaboratoryInvitations");

        migrationBuilder.AddColumn<TimeSpan>(
            name: "ExpirationTimeSpan",
            table: "LaboratoryInvitations",
            type: "time",
            nullable: false,
            defaultValue: new TimeSpan(0, 0, 0, 0, 0));
    }
}
