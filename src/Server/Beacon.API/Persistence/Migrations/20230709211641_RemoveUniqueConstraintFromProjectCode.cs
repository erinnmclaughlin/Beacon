using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueConstraintFromProjectCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectCode_CustomerCode_ProjectCode_Suffix",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectCode_CustomerCode_ProjectCode_Suffix",
                table: "Projects",
                columns: new[] { "ProjectCode_CustomerCode", "ProjectCode_Suffix" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectCode_CustomerCode_ProjectCode_Suffix",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectCode_CustomerCode_ProjectCode_Suffix",
                table: "Projects",
                columns: new[] { "ProjectCode_CustomerCode", "ProjectCode_Suffix" },
                unique: true);
        }
    }
}
