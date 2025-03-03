using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

/// <inheritdoc />
public partial class AddInstrumentAssociationToProjectEvent : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LaboratoryInstrumentUsage",
            columns: table => new
            {
                InstrumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProjectEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LaboratoryInstrumentUsage", x => new { x.InstrumentId, x.ProjectEventId });
                table.ForeignKey(
                    name: "FK_LaboratoryInstrumentUsage_Laboratories_LaboratoryId",
                    column: x => x.LaboratoryId,
                    principalTable: "Laboratories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_LaboratoryInstrumentUsage_LaboratoryInstruments_InstrumentId",
                    column: x => x.InstrumentId,
                    principalTable: "LaboratoryInstruments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_LaboratoryInstrumentUsage_ProjectEvents_ProjectEventId",
                    column: x => x.ProjectEventId,
                    principalTable: "ProjectEvents",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_LaboratoryInstrumentUsage_LaboratoryId",
            table: "LaboratoryInstrumentUsage",
            column: "LaboratoryId");

        migrationBuilder.CreateIndex(
            name: "IX_LaboratoryInstrumentUsage_ProjectEventId",
            table: "LaboratoryInstrumentUsage",
            column: "ProjectEventId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LaboratoryInstrumentUsage");
    }
}
