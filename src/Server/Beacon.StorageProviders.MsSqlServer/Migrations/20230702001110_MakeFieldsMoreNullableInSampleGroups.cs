using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beacon.StorageProviders.MsSqlServer.Migrations;

/// <inheritdoc />
public partial class MakeFieldsMoreNullableInSampleGroups : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<double>(
            name: "Volume",
            table: "SampleGroups",
            type: "float",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<int>(
            name: "Quantity",
            table: "SampleGroups",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<bool>(
            name: "IsLightSensitive",
            table: "SampleGroups",
            type: "bit",
            nullable: true,
            oldClrType: typeof(bool),
            oldType: "bit");

        migrationBuilder.AlterColumn<bool>(
            name: "IsHazardous",
            table: "SampleGroups",
            type: "bit",
            nullable: true,
            oldClrType: typeof(bool),
            oldType: "bit");

        migrationBuilder.AlterColumn<string>(
            name: "ContainerType",
            table: "SampleGroups",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<double>(
            name: "Volume",
            table: "SampleGroups",
            type: "float",
            nullable: false,
            defaultValue: 0.0,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<int>(
            name: "Quantity",
            table: "SampleGroups",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "IsLightSensitive",
            table: "SampleGroups",
            type: "bit",
            nullable: false,
            defaultValue: false,
            oldClrType: typeof(bool),
            oldType: "bit",
            oldNullable: true);

        migrationBuilder.AlterColumn<bool>(
            name: "IsHazardous",
            table: "SampleGroups",
            type: "bit",
            nullable: false,
            defaultValue: false,
            oldClrType: typeof(bool),
            oldType: "bit",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "ContainerType",
            table: "SampleGroups",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);
    }
}
