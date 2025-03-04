using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

/// <inheritdoc />
public partial class AddLabInstruments : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LaboratoryInstruments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                InstrumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LaboratoryInstruments", x => x.Id);
                table.ForeignKey(
                    name: "FK_LaboratoryInstruments_Laboratories_LaboratoryId",
                    column: x => x.LaboratoryId,
                    principalTable: "Laboratories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_LaboratoryInstruments_LaboratoryId",
            table: "LaboratoryInstruments",
            column: "LaboratoryId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LaboratoryInstruments");
    }
}
