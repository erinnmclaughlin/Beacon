using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UseCompositePKForMemberships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LaboratoryMemberships",
                table: "LaboratoryMemberships");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryMemberships_LaboratoryId_MemberId",
                table: "LaboratoryMemberships");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LaboratoryMemberships");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LaboratoryMemberships",
                table: "LaboratoryMemberships",
                columns: new[] { "LaboratoryId", "MemberId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LaboratoryMemberships",
                table: "LaboratoryMemberships");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "LaboratoryMemberships",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_LaboratoryMemberships",
                table: "LaboratoryMemberships",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryMemberships_LaboratoryId_MemberId",
                table: "LaboratoryMemberships",
                columns: new[] { "LaboratoryId", "MemberId" },
                unique: true);
        }
    }
}
