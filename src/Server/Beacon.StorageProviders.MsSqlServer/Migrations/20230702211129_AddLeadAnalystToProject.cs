using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

/// <inheritdoc />
public partial class AddLeadAnalystToProject : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "LeadAnalystId",
            table: "Projects",
            type: "uniqueidentifier",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Projects_LeadAnalystId",
            table: "Projects",
            column: "LeadAnalystId");

        migrationBuilder.AddForeignKey(
            name: "FK_Projects_Users_LeadAnalystId",
            table: "Projects",
            column: "LeadAnalystId",
            principalTable: "Users",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Projects_Users_LeadAnalystId",
            table: "Projects");

        migrationBuilder.DropIndex(
            name: "IX_Projects_LeadAnalystId",
            table: "Projects");

        migrationBuilder.DropColumn(
            name: "LeadAnalystId",
            table: "Projects");
    }
}
