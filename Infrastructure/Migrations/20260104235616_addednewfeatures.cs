using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addednewfeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Vehicles_VehicleId",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_LicenseNumber",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_VehicleId",
                table: "Drivers");

            migrationBuilder.EnsureSchema(
                name: "Fleet");

            migrationBuilder.RenameTable(
                name: "Vehicles",
                newName: "Vehicles",
                newSchema: "Fleet");

            migrationBuilder.RenameTable(
                name: "MaintenanceRecords",
                newName: "MaintenanceRecords",
                newSchema: "Fleet");

            migrationBuilder.RenameTable(
                name: "Drivers",
                newName: "Drivers",
                newSchema: "Fleet");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                schema: "Fleet",
                table: "Drivers",
                newName: "CurrentVehicleId");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "Fleet",
                table: "Drivers",
                newName: "FullName");

            migrationBuilder.AddColumn<int>(
                name: "CurrentMileage",
                schema: "Fleet",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastMaintenanceMileage",
                schema: "Fleet",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MileageAtMaintenance",
                schema: "Fleet",
                table: "MaintenanceRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "Fleet",
                table: "MaintenanceRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Fleet",
                table: "Drivers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "Fleet",
                table: "Drivers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LicensePlate",
                schema: "Fleet",
                table: "Vehicles",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_CurrentVehicleId",
                schema: "Fleet",
                table: "Drivers",
                column: "CurrentVehicleId",
                unique: true,
                filter: "[CurrentVehicleId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Vehicles_CurrentVehicleId",
                schema: "Fleet",
                table: "Drivers",
                column: "CurrentVehicleId",
                principalSchema: "Fleet",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Vehicles_CurrentVehicleId",
                schema: "Fleet",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_LicensePlate",
                schema: "Fleet",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_CurrentVehicleId",
                schema: "Fleet",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CurrentMileage",
                schema: "Fleet",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "LastMaintenanceMileage",
                schema: "Fleet",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "MileageAtMaintenance",
                schema: "Fleet",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "Fleet",
                table: "MaintenanceRecords");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Fleet",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Fleet",
                table: "Drivers");

            migrationBuilder.RenameTable(
                name: "Vehicles",
                schema: "Fleet",
                newName: "Vehicles");

            migrationBuilder.RenameTable(
                name: "MaintenanceRecords",
                schema: "Fleet",
                newName: "MaintenanceRecords");

            migrationBuilder.RenameTable(
                name: "Drivers",
                schema: "Fleet",
                newName: "Drivers");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Drivers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CurrentVehicleId",
                table: "Drivers",
                newName: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_LicenseNumber",
                table: "Drivers",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_VehicleId",
                table: "Drivers",
                column: "VehicleId",
                unique: true,
                filter: "[VehicleId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Vehicles_VehicleId",
                table: "Drivers",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
