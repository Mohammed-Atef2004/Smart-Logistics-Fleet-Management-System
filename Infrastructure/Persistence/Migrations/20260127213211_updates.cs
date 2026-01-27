using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Vehicles_CurrentVehicleId",
                table: "Drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Shipments_ShipmentId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_ApplicationUserId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Routes_RouteId",
                table: "Shipments");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackingUpdates_Shipments_ShipmentId",
                table: "TrackingUpdates");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Drivers_CurrentDriverId",
                schema: "Fleet",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_RouteId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Roles_ApplicationUserId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_Name",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Payments_InvoiceId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_CurrentVehicleId",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_CurrentDriverId",
                schema: "Fleet",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_LicensePlate",
                schema: "Fleet",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrackingUpdates",
                table: "TrackingUpdates");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TotalCurrency",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ClaimAmount",
                table: "InsuranceClaims");

            migrationBuilder.DropColumn(
                name: "ClaimCurrency",
                table: "InsuranceClaims");

            migrationBuilder.DropColumn(
                name: "CurrentVehicleId",
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
                name: "TrackingUpdates",
                newName: "TrackingUpdate");

            migrationBuilder.RenameColumn(
                name: "Shelf",
                table: "InventoryItems",
                newName: "Location_Shelf");

            migrationBuilder.RenameColumn(
                name: "Bin",
                table: "InventoryItems",
                newName: "Location_Bin");

            migrationBuilder.RenameColumn(
                name: "Aisle",
                table: "InventoryItems",
                newName: "Location_Aisle");

            migrationBuilder.RenameIndex(
                name: "IX_TrackingUpdates_ShipmentId",
                table: "TrackingUpdate",
                newName: "IX_TrackingUpdate_ShipmentId");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedVehicleId",
                table: "Shipments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "Shipments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DispatchedAt",
                table: "Shipments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedDeliveryDate",
                table: "Shipments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Route_Destination_City",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Route_Destination_Country",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Route_Destination_PostalCode",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Route_Destination_State",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Route_Destination_Street",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Route_EstimatedDistance",
                table: "Shipments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Route_Origin_City",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Route_Origin_Country",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Route_Origin_PostalCode",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Route_Origin_State",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Route_Origin_Street",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<Guid>(
                name: "ShipmentId",
                table: "Packages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<decimal>(
                name: "DeclaredValue",
                table: "Packages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Packages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Location_Shelf",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Location_Bin",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Location_Aisle",
                table: "InventoryItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "LicensePlate",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MaintenanceRecords",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "TrackingUpdate",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "TrackingUpdate",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrackingUpdate",
                table: "TrackingUpdate",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CurrentDriverId",
                table: "Vehicles",
                column: "CurrentDriverId",
                unique: true,
                filter: "[CurrentDriverId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Shipments_ShipmentId",
                table: "Packages",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackingUpdate_Shipments_ShipmentId",
                table: "TrackingUpdate",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Drivers_CurrentDriverId",
                table: "Vehicles",
                column: "CurrentDriverId",
                principalTable: "Drivers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Shipments_ShipmentId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackingUpdate_Shipments_ShipmentId",
                table: "TrackingUpdate");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Drivers_CurrentDriverId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_CurrentDriverId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrackingUpdate",
                table: "TrackingUpdate");

            migrationBuilder.DropColumn(
                name: "AssignedVehicleId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DispatchedAt",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "EstimatedDeliveryDate",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Route_Destination_City",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Route_Destination_Country",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Route_Destination_PostalCode",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Route_Destination_State",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Route_Destination_Street",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Route_EstimatedDistance",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Route_Origin_City",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Route_Origin_Country",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Route_Origin_PostalCode",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Route_Origin_State",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Route_Origin_Street",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DeclaredValue",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "TrackingUpdate");

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
                name: "TrackingUpdate",
                newName: "TrackingUpdates");

            migrationBuilder.RenameColumn(
                name: "Location_Shelf",
                table: "InventoryItems",
                newName: "Shelf");

            migrationBuilder.RenameColumn(
                name: "Location_Bin",
                table: "InventoryItems",
                newName: "Bin");

            migrationBuilder.RenameColumn(
                name: "Location_Aisle",
                table: "InventoryItems",
                newName: "Aisle");

            migrationBuilder.RenameIndex(
                name: "IX_TrackingUpdate_ShipmentId",
                table: "TrackingUpdates",
                newName: "IX_TrackingUpdates_ShipmentId");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "Shipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "RouteId",
                table: "Shipments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Roles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Payments",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "ShipmentId",
                table: "Packages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Packages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TotalCurrency",
                table: "Invoices",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Shelf",
                table: "InventoryItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Bin",
                table: "InventoryItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Aisle",
                table: "InventoryItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "ClaimAmount",
                table: "InsuranceClaims",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ClaimCurrency",
                table: "InsuranceClaims",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Drivers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Drivers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentVehicleId",
                table: "Drivers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicensePlate",
                schema: "Fleet",
                table: "Vehicles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "Fleet",
                table: "MaintenanceRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "TrackingUpdates",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrackingUpdates",
                table: "TrackingUpdates",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FromCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FromPostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FromState = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FromStreet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ToCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ToCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ToPostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ToState = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ToStreet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_RouteId",
                table: "Shipments",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_ApplicationUserId",
                table: "Roles",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvoiceId",
                table: "Payments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_CurrentVehicleId",
                table: "Drivers",
                column: "CurrentVehicleId",
                unique: true,
                filter: "[CurrentVehicleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CurrentDriverId",
                schema: "Fleet",
                table: "Vehicles",
                column: "CurrentDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LicensePlate",
                schema: "Fleet",
                table: "Vehicles",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Vehicles_CurrentVehicleId",
                table: "Drivers",
                column: "CurrentVehicleId",
                principalSchema: "Fleet",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Shipments_ShipmentId",
                table: "Packages",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_ApplicationUserId",
                table: "Roles",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Routes_RouteId",
                table: "Shipments",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackingUpdates_Shipments_ShipmentId",
                table: "TrackingUpdates",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Drivers_CurrentDriverId",
                schema: "Fleet",
                table: "Vehicles",
                column: "CurrentDriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
