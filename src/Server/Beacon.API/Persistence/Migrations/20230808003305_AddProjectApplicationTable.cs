using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectApplicationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryInstrumentUsage_Laboratories_LaboratoryId",
                table: "LaboratoryInstrumentUsage");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectContacts_Laboratories_LaboratoryId",
                table: "ProjectContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleGroups_Laboratories_LaboratoryId",
                table: "SampleGroups");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ProjectApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectApplications_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectApplicationTags",
                columns: table => new
                {
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectApplicationTags", x => new { x.ApplicationId, x.ProjectId });
                    table.ForeignKey(
                        name: "FK_ProjectApplicationTags_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectApplicationTags_ProjectApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "ProjectApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectApplicationTags_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApplications_LaboratoryId",
                table: "ProjectApplications",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApplications_Name",
                table: "ProjectApplications",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApplicationTags_LaboratoryId",
                table: "ProjectApplicationTags",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApplicationTags_ProjectId",
                table: "ProjectApplicationTags",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryInstrumentUsage_Laboratories_LaboratoryId",
                table: "LaboratoryInstrumentUsage",
                column: "LaboratoryId",
                principalTable: "Laboratories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectContacts_Laboratories_LaboratoryId",
                table: "ProjectContacts",
                column: "LaboratoryId",
                principalTable: "Laboratories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SampleGroups_Laboratories_LaboratoryId",
                table: "SampleGroups",
                column: "LaboratoryId",
                principalTable: "Laboratories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryInstrumentUsage_Laboratories_LaboratoryId",
                table: "LaboratoryInstrumentUsage");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectContacts_Laboratories_LaboratoryId",
                table: "ProjectContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleGroups_Laboratories_LaboratoryId",
                table: "SampleGroups");

            migrationBuilder.DropTable(
                name: "ProjectApplicationTags");

            migrationBuilder.DropTable(
                name: "ProjectApplications");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Projects");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryInstrumentUsage_Laboratories_LaboratoryId",
                table: "LaboratoryInstrumentUsage",
                column: "LaboratoryId",
                principalTable: "Laboratories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectContacts_Laboratories_LaboratoryId",
                table: "ProjectContacts",
                column: "LaboratoryId",
                principalTable: "Laboratories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SampleGroups_Laboratories_LaboratoryId",
                table: "SampleGroups",
                column: "LaboratoryId",
                principalTable: "Laboratories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
