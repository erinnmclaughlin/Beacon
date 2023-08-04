using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectDateToProjectCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from ProjectContacts; delete from SampleGroups; delete from ProjectEvents; delete from Projects;");

            migrationBuilder.AddColumn<string>(
                name: "ProjectCode_Date",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectCode_Date",
                table: "Projects");
        }
    }
}
