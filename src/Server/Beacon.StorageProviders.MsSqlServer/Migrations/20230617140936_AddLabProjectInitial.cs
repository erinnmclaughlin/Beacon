using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

/// <inheritdoc />
public partial class AddLabProjectInitial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Customers", x => x.Id);
                table.ForeignKey(
                    name: "FK_Customers_Laboratories_LaboratoryId",
                    column: x => x.LaboratoryId,
                    principalTable: "Laboratories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Projects",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProjectId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Projects", x => x.Id);
                table.ForeignKey(
                    name: "FK_Projects_Customers_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Customers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Projects_Laboratories_LaboratoryId",
                    column: x => x.LaboratoryId,
                    principalTable: "Laboratories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Projects_Users_CreatedById",
                    column: x => x.CreatedById,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Customers_LaboratoryId",
            table: "Customers",
            column: "LaboratoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Projects_CreatedById",
            table: "Projects",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_Projects_CustomerId",
            table: "Projects",
            column: "CustomerId");

        migrationBuilder.CreateIndex(
            name: "IX_Projects_LaboratoryId",
            table: "Projects",
            column: "LaboratoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Projects_ProjectId",
            table: "Projects",
            column: "ProjectId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Projects");

        migrationBuilder.DropTable(
            name: "Customers");
    }
}
