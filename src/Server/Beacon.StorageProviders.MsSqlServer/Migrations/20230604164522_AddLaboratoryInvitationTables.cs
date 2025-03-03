using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

/// <inheritdoc />
public partial class AddLaboratoryInvitationTables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LaboratoryInvitations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                ExpirationTimeSpan = table.Column<TimeSpan>(type: "time", nullable: false),
                NewMemberEmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                MembershipType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LaboratoryInvitations", x => x.Id);
                table.ForeignKey(
                    name: "FK_LaboratoryInvitations_Laboratories_LaboratoryId",
                    column: x => x.LaboratoryId,
                    principalTable: "Laboratories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_LaboratoryInvitations_Users_CreatedById",
                    column: x => x.CreatedById,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "LaboratoryInvitationEmails",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DeliveryStatus_OperationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DeliveryStatus_SentOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                DeliveryStatus_ExpiresOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LaboratoryInvitationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LaboratoryInvitationEmails", x => x.Id);
                table.ForeignKey(
                    name: "FK_LaboratoryInvitationEmails_LaboratoryInvitations_LaboratoryInvitationId",
                    column: x => x.LaboratoryInvitationId,
                    principalTable: "LaboratoryInvitations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_LaboratoryInvitationEmails_LaboratoryInvitationId",
            table: "LaboratoryInvitationEmails",
            column: "LaboratoryInvitationId");

        migrationBuilder.CreateIndex(
            name: "IX_LaboratoryInvitations_CreatedById",
            table: "LaboratoryInvitations",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_LaboratoryInvitations_LaboratoryId",
            table: "LaboratoryInvitations",
            column: "LaboratoryId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LaboratoryInvitationEmails");

        migrationBuilder.DropTable(
            name: "LaboratoryInvitations");
    }
}
