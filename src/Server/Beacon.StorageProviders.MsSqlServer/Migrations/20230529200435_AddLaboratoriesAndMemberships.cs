using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

/// <inheritdoc />
public partial class AddLaboratoriesAndMemberships : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Laboratories",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Slug = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Laboratories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LaboratoryMemberships",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MembershipType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LaboratoryMemberships", x => x.Id);
                table.ForeignKey(
                    name: "FK_LaboratoryMemberships_Laboratories_LaboratoryId",
                    column: x => x.LaboratoryId,
                    principalTable: "Laboratories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_LaboratoryMemberships_Users_MemberId",
                    column: x => x.MemberId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Laboratories_Slug",
            table: "Laboratories",
            column: "Slug",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_LaboratoryMemberships_LaboratoryId_MemberId",
            table: "LaboratoryMemberships",
            columns: new[] { "LaboratoryId", "MemberId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_LaboratoryMemberships_MemberId",
            table: "LaboratoryMemberships",
            column: "MemberId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LaboratoryMemberships");

        migrationBuilder.DropTable(
            name: "Laboratories");
    }
}
