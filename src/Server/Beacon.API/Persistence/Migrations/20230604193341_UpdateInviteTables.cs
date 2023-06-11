using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInviteTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveryStatus_SentOn",
                table: "LaboratoryInvitationEmails",
                newName: "SentOn");

            migrationBuilder.RenameColumn(
                name: "DeliveryStatus_OperationId",
                table: "LaboratoryInvitationEmails",
                newName: "OperationId");

            migrationBuilder.RenameColumn(
                name: "DeliveryStatus_ExpiresOn",
                table: "LaboratoryInvitationEmails",
                newName: "ExpiresOn");

            migrationBuilder.AddColumn<Guid>(
                name: "AcceptedById",
                table: "LaboratoryInvitations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "SentOn",
                table: "LaboratoryInvitationEmails",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ExpiresOn",
                table: "LaboratoryInvitationEmails",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LaboratoryInvitations_AcceptedById",
                table: "LaboratoryInvitations",
                column: "AcceptedById");

            migrationBuilder.AddForeignKey(
                name: "FK_LaboratoryInvitations_Users_AcceptedById",
                table: "LaboratoryInvitations",
                column: "AcceptedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LaboratoryInvitations_Users_AcceptedById",
                table: "LaboratoryInvitations");

            migrationBuilder.DropIndex(
                name: "IX_LaboratoryInvitations_AcceptedById",
                table: "LaboratoryInvitations");

            migrationBuilder.DropColumn(
                name: "AcceptedById",
                table: "LaboratoryInvitations");

            migrationBuilder.RenameColumn(
                name: "SentOn",
                table: "LaboratoryInvitationEmails",
                newName: "DeliveryStatus_SentOn");

            migrationBuilder.RenameColumn(
                name: "OperationId",
                table: "LaboratoryInvitationEmails",
                newName: "DeliveryStatus_OperationId");

            migrationBuilder.RenameColumn(
                name: "ExpiresOn",
                table: "LaboratoryInvitationEmails",
                newName: "DeliveryStatus_ExpiresOn");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeliveryStatus_SentOn",
                table: "LaboratoryInvitationEmails",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DeliveryStatus_ExpiresOn",
                table: "LaboratoryInvitationEmails",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");
        }
    }
}
