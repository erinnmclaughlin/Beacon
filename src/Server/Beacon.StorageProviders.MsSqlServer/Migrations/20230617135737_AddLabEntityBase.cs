using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

/// <inheritdoc />
public partial class AddLabEntityBase : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_LaboratoryInvitations_Laboratories_LaboratoryId",
            table: "LaboratoryInvitations");

        migrationBuilder.DropForeignKey(
            name: "FK_LaboratoryMemberships_Laboratories_LaboratoryId",
            table: "LaboratoryMemberships");

        migrationBuilder.AddColumn<Guid>(
            name: "LaboratoryId",
            table: "LaboratoryInvitationEmails",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.Sql("""
            update LaboratoryInvitationEmails
            set LaboratoryId = i.LaboratoryId
            from LaboratoryInvitations i
            where i.Id = LaboratoryInvitationId
            """);

        migrationBuilder.CreateIndex(
            name: "IX_LaboratoryInvitationEmails_LaboratoryId",
            table: "LaboratoryInvitationEmails",
            column: "LaboratoryId");

        migrationBuilder.AddForeignKey(
            name: "FK_LaboratoryInvitationEmails_Laboratories_LaboratoryId",
            table: "LaboratoryInvitationEmails",
            column: "LaboratoryId",
            principalTable: "Laboratories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_LaboratoryInvitations_Laboratories_LaboratoryId",
            table: "LaboratoryInvitations",
            column: "LaboratoryId",
            principalTable: "Laboratories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_LaboratoryMemberships_Laboratories_LaboratoryId",
            table: "LaboratoryMemberships",
            column: "LaboratoryId",
            principalTable: "Laboratories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_LaboratoryInvitationEmails_Laboratories_LaboratoryId",
            table: "LaboratoryInvitationEmails");

        migrationBuilder.DropForeignKey(
            name: "FK_LaboratoryInvitations_Laboratories_LaboratoryId",
            table: "LaboratoryInvitations");

        migrationBuilder.DropForeignKey(
            name: "FK_LaboratoryMemberships_Laboratories_LaboratoryId",
            table: "LaboratoryMemberships");

        migrationBuilder.DropIndex(
            name: "IX_LaboratoryInvitationEmails_LaboratoryId",
            table: "LaboratoryInvitationEmails");

        migrationBuilder.DropColumn(
            name: "LaboratoryId",
            table: "LaboratoryInvitationEmails");

        migrationBuilder.AddForeignKey(
            name: "FK_LaboratoryInvitations_Laboratories_LaboratoryId",
            table: "LaboratoryInvitations",
            column: "LaboratoryId",
            principalTable: "Laboratories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_LaboratoryMemberships_Laboratories_LaboratoryId",
            table: "LaboratoryMemberships",
            column: "LaboratoryId",
            principalTable: "Laboratories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
