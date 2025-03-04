using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

/// <inheritdoc />
public partial class RenameTables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_LaboratoryInvitationEmails_Laboratories_LaboratoryId",
            table: "LaboratoryInvitationEmails");

        migrationBuilder.DropForeignKey(
            name: "FK_LaboratoryInvitationEmails_LaboratoryInvitations_LaboratoryInvitationId",
            table: "LaboratoryInvitationEmails");

        migrationBuilder.DropForeignKey(
            name: "FK_LaboratoryInvitations_Laboratories_LaboratoryId",
            table: "LaboratoryInvitations");

        migrationBuilder.DropForeignKey(
            name: "FK_LaboratoryInvitations_Users_AcceptedById",
            table: "LaboratoryInvitations");

        migrationBuilder.DropForeignKey(
            name: "FK_LaboratoryInvitations_Users_CreatedById",
            table: "LaboratoryInvitations");

        migrationBuilder.DropForeignKey(
            name: "FK_LaboratoryMemberships_Laboratories_LaboratoryId",
            table: "LaboratoryMemberships");

        migrationBuilder.DropForeignKey(
            name: "FK_LaboratoryMemberships_Users_MemberId",
            table: "LaboratoryMemberships");

        migrationBuilder.DropPrimaryKey(
            name: "PK_LaboratoryMemberships",
            table: "LaboratoryMemberships");

        migrationBuilder.DropPrimaryKey(
            name: "PK_LaboratoryInvitations",
            table: "LaboratoryInvitations");

        migrationBuilder.DropPrimaryKey(
            name: "PK_LaboratoryInvitationEmails",
            table: "LaboratoryInvitationEmails");

        migrationBuilder.RenameTable(
            name: "LaboratoryMemberships",
            newName: "Memberships");

        migrationBuilder.RenameTable(
            name: "LaboratoryInvitations",
            newName: "Invitations");

        migrationBuilder.RenameTable(
            name: "LaboratoryInvitationEmails",
            newName: "InvitationEmails");

        migrationBuilder.RenameIndex(
            name: "IX_LaboratoryMemberships_MemberId",
            table: "Memberships",
            newName: "IX_Memberships_MemberId");

        migrationBuilder.RenameIndex(
            name: "IX_LaboratoryInvitations_LaboratoryId",
            table: "Invitations",
            newName: "IX_Invitations_LaboratoryId");

        migrationBuilder.RenameIndex(
            name: "IX_LaboratoryInvitations_CreatedById",
            table: "Invitations",
            newName: "IX_Invitations_CreatedById");

        migrationBuilder.RenameIndex(
            name: "IX_LaboratoryInvitations_AcceptedById",
            table: "Invitations",
            newName: "IX_Invitations_AcceptedById");

        migrationBuilder.RenameIndex(
            name: "IX_LaboratoryInvitationEmails_LaboratoryInvitationId",
            table: "InvitationEmails",
            newName: "IX_InvitationEmails_LaboratoryInvitationId");

        migrationBuilder.RenameIndex(
            name: "IX_LaboratoryInvitationEmails_LaboratoryId",
            table: "InvitationEmails",
            newName: "IX_InvitationEmails_LaboratoryId");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Memberships",
            table: "Memberships",
            columns: new[] { "LaboratoryId", "MemberId" });

        migrationBuilder.AddPrimaryKey(
            name: "PK_Invitations",
            table: "Invitations",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_InvitationEmails",
            table: "InvitationEmails",
            column: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_InvitationEmails_Invitations_LaboratoryInvitationId",
            table: "InvitationEmails",
            column: "LaboratoryInvitationId",
            principalTable: "Invitations",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_InvitationEmails_Laboratories_LaboratoryId",
            table: "InvitationEmails",
            column: "LaboratoryId",
            principalTable: "Laboratories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Invitations_Laboratories_LaboratoryId",
            table: "Invitations",
            column: "LaboratoryId",
            principalTable: "Laboratories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Invitations_Users_AcceptedById",
            table: "Invitations",
            column: "AcceptedById",
            principalTable: "Users",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Invitations_Users_CreatedById",
            table: "Invitations",
            column: "CreatedById",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Memberships_Laboratories_LaboratoryId",
            table: "Memberships",
            column: "LaboratoryId",
            principalTable: "Laboratories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Memberships_Users_MemberId",
            table: "Memberships",
            column: "MemberId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_InvitationEmails_Invitations_LaboratoryInvitationId",
            table: "InvitationEmails");

        migrationBuilder.DropForeignKey(
            name: "FK_InvitationEmails_Laboratories_LaboratoryId",
            table: "InvitationEmails");

        migrationBuilder.DropForeignKey(
            name: "FK_Invitations_Laboratories_LaboratoryId",
            table: "Invitations");

        migrationBuilder.DropForeignKey(
            name: "FK_Invitations_Users_AcceptedById",
            table: "Invitations");

        migrationBuilder.DropForeignKey(
            name: "FK_Invitations_Users_CreatedById",
            table: "Invitations");

        migrationBuilder.DropForeignKey(
            name: "FK_Memberships_Laboratories_LaboratoryId",
            table: "Memberships");

        migrationBuilder.DropForeignKey(
            name: "FK_Memberships_Users_MemberId",
            table: "Memberships");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Memberships",
            table: "Memberships");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Invitations",
            table: "Invitations");

        migrationBuilder.DropPrimaryKey(
            name: "PK_InvitationEmails",
            table: "InvitationEmails");

        migrationBuilder.RenameTable(
            name: "Memberships",
            newName: "LaboratoryMemberships");

        migrationBuilder.RenameTable(
            name: "Invitations",
            newName: "LaboratoryInvitations");

        migrationBuilder.RenameTable(
            name: "InvitationEmails",
            newName: "LaboratoryInvitationEmails");

        migrationBuilder.RenameIndex(
            name: "IX_Memberships_MemberId",
            table: "LaboratoryMemberships",
            newName: "IX_LaboratoryMemberships_MemberId");

        migrationBuilder.RenameIndex(
            name: "IX_Invitations_LaboratoryId",
            table: "LaboratoryInvitations",
            newName: "IX_LaboratoryInvitations_LaboratoryId");

        migrationBuilder.RenameIndex(
            name: "IX_Invitations_CreatedById",
            table: "LaboratoryInvitations",
            newName: "IX_LaboratoryInvitations_CreatedById");

        migrationBuilder.RenameIndex(
            name: "IX_Invitations_AcceptedById",
            table: "LaboratoryInvitations",
            newName: "IX_LaboratoryInvitations_AcceptedById");

        migrationBuilder.RenameIndex(
            name: "IX_InvitationEmails_LaboratoryInvitationId",
            table: "LaboratoryInvitationEmails",
            newName: "IX_LaboratoryInvitationEmails_LaboratoryInvitationId");

        migrationBuilder.RenameIndex(
            name: "IX_InvitationEmails_LaboratoryId",
            table: "LaboratoryInvitationEmails",
            newName: "IX_LaboratoryInvitationEmails_LaboratoryId");

        migrationBuilder.AddPrimaryKey(
            name: "PK_LaboratoryMemberships",
            table: "LaboratoryMemberships",
            columns: new[] { "LaboratoryId", "MemberId" });

        migrationBuilder.AddPrimaryKey(
            name: "PK_LaboratoryInvitations",
            table: "LaboratoryInvitations",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_LaboratoryInvitationEmails",
            table: "LaboratoryInvitationEmails",
            column: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_LaboratoryInvitationEmails_Laboratories_LaboratoryId",
            table: "LaboratoryInvitationEmails",
            column: "LaboratoryId",
            principalTable: "Laboratories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_LaboratoryInvitationEmails_LaboratoryInvitations_LaboratoryInvitationId",
            table: "LaboratoryInvitationEmails",
            column: "LaboratoryInvitationId",
            principalTable: "LaboratoryInvitations",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_LaboratoryInvitations_Laboratories_LaboratoryId",
            table: "LaboratoryInvitations",
            column: "LaboratoryId",
            principalTable: "Laboratories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_LaboratoryInvitations_Users_AcceptedById",
            table: "LaboratoryInvitations",
            column: "AcceptedById",
            principalTable: "Users",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_LaboratoryInvitations_Users_CreatedById",
            table: "LaboratoryInvitations",
            column: "CreatedById",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_LaboratoryMemberships_Laboratories_LaboratoryId",
            table: "LaboratoryMemberships",
            column: "LaboratoryId",
            principalTable: "Laboratories",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_LaboratoryMemberships_Users_MemberId",
            table: "LaboratoryMemberships",
            column: "MemberId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
