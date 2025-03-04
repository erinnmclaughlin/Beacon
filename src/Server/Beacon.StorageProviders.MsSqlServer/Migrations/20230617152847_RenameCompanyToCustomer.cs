using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

/// <inheritdoc />
public partial class RenameCompanyToCustomer : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "ProjectCode_CompanyCode",
            table: "Projects",
            newName: "ProjectCode_CustomerCode");

        migrationBuilder.RenameIndex(
            name: "IX_Projects_ProjectCode_CompanyCode_ProjectCode_Suffix",
            table: "Projects",
            newName: "IX_Projects_ProjectCode_CustomerCode_ProjectCode_Suffix");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "ProjectCode_CustomerCode",
            table: "Projects",
            newName: "ProjectCode_CompanyCode");

        migrationBuilder.RenameIndex(
            name: "IX_Projects_ProjectCode_CustomerCode_ProjectCode_Suffix",
            table: "Projects",
            newName: "IX_Projects_ProjectCode_CompanyCode_ProjectCode_Suffix");
    }
}
