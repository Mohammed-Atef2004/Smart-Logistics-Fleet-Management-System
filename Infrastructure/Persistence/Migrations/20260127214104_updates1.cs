using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updates1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.EnsureSchema(
                name: "Fleet");

            migrationBuilder.EnsureSchema(
                name: "Users");

            migrationBuilder.RenameTable(
                name: "Vehicles",
                newName: "Vehicles",
                newSchema: "Fleet");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Roles",
                newSchema: "Users");

            migrationBuilder.RenameTable(
                name: "MaintenanceRecords",
                newName: "MaintenanceRecords",
                newSchema: "Fleet");

            migrationBuilder.RenameTable(
                name: "Drivers",
                newName: "Drivers",
                newSchema: "Fleet");

            migrationBuilder.RenameColumn(
                name: "Route_Origin_Street",
                table: "Shipments",
                newName: "OriginStreet");

            migrationBuilder.RenameColumn(
                name: "Route_Origin_State",
                table: "Shipments",
                newName: "OriginState");

            migrationBuilder.RenameColumn(
                name: "Route_Origin_PostalCode",
                table: "Shipments",
                newName: "OriginPostalCode");

            migrationBuilder.RenameColumn(
                name: "Route_Origin_Country",
                table: "Shipments",
                newName: "OriginCountry");

            migrationBuilder.RenameColumn(
                name: "Route_Origin_City",
                table: "Shipments",
                newName: "OriginCity");

            migrationBuilder.RenameColumn(
                name: "Route_EstimatedDistance",
                table: "Shipments",
                newName: "EstimatedDistance");

            migrationBuilder.RenameColumn(
                name: "Route_Destination_Street",
                table: "Shipments",
                newName: "DestinationStreet");

            migrationBuilder.RenameColumn(
                name: "Route_Destination_State",
                table: "Shipments",
                newName: "DestinationState");

            migrationBuilder.RenameColumn(
                name: "Route_Destination_PostalCode",
                table: "Shipments",
                newName: "DestinationPostalCode");

            migrationBuilder.RenameColumn(
                name: "Route_Destination_Country",
                table: "Shipments",
                newName: "DestinationCountry");

            migrationBuilder.RenameColumn(
                name: "Route_Destination_City",
                table: "Shipments",
                newName: "DestinationCity");

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

            migrationBuilder.AlterColumn<string>(
                name: "OriginStreet",
                table: "Shipments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OriginState",
                table: "Shipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OriginPostalCode",
                table: "Shipments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OriginCountry",
                table: "Shipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OriginCity",
                table: "Shipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationStreet",
                table: "Shipments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationState",
                table: "Shipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationPostalCode",
                table: "Shipments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationCountry",
                table: "Shipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationCity",
                table: "Shipments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
                schema: "Users",
                table: "Roles",
                type: "nvarchar(250)",
                maxLength: 250,
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
                name: "LicenseNumber",
                schema: "Fleet",
                table: "Drivers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                schema: "Fleet",
                table: "Drivers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvoiceId",
                table: "Payments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_LicensePlate",
                schema: "Fleet",
                table: "Vehicles",
                column: "LicensePlate",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Shipments_ShipmentId",
                table: "Packages",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackingUpdate_Shipments_ShipmentId",
                table: "TrackingUpdate",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Drivers_CurrentDriverId",
                schema: "Fleet",
                table: "Vehicles",
                column: "CurrentDriverId",
                principalSchema: "Fleet",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
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
                schema: "Fleet",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Payments_InvoiceId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_LicensePlate",
                schema: "Fleet",
                table: "Vehicles");

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

            migrationBuilder.RenameTable(
                name: "Vehicles",
                schema: "Fleet",
                newName: "Vehicles");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "Users",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "MaintenanceRecords",
                schema: "Fleet",
                newName: "MaintenanceRecords");

            migrationBuilder.RenameTable(
                name: "Drivers",
                schema: "Fleet",
                newName: "Drivers");

            migrationBuilder.RenameColumn(
                name: "OriginStreet",
                table: "Shipments",
                newName: "Route_Origin_Street");

            migrationBuilder.RenameColumn(
                name: "OriginState",
                table: "Shipments",
                newName: "Route_Origin_State");

            migrationBuilder.RenameColumn(
                name: "OriginPostalCode",
                table: "Shipments",
                newName: "Route_Origin_PostalCode");

            migrationBuilder.RenameColumn(
                name: "OriginCountry",
                table: "Shipments",
                newName: "Route_Origin_Country");

            migrationBuilder.RenameColumn(
                name: "OriginCity",
                table: "Shipments",
                newName: "Route_Origin_City");

            migrationBuilder.RenameColumn(
                name: "EstimatedDistance",
                table: "Shipments",
                newName: "Route_EstimatedDistance");

            migrationBuilder.RenameColumn(
                name: "DestinationStreet",
                table: "Shipments",
                newName: "Route_Destination_Street");

            migrationBuilder.RenameColumn(
                name: "DestinationState",
                table: "Shipments",
                newName: "Route_Destination_State");

            migrationBuilder.RenameColumn(
                name: "DestinationPostalCode",
                table: "Shipments",
                newName: "Route_Destination_PostalCode");

            migrationBuilder.RenameColumn(
                name: "DestinationCountry",
                table: "Shipments",
                newName: "Route_Destination_Country");

            migrationBuilder.RenameColumn(
                name: "DestinationCity",
                table: "Shipments",
                newName: "Route_Destination_City");

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

            migrationBuilder.AlterColumn<string>(
                name: "Route_Origin_Street",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Route_Origin_State",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Route_Origin_PostalCode",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Route_Origin_Country",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Route_Origin_City",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Route_Destination_Street",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Route_Destination_State",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Route_Destination_PostalCode",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Route_Destination_Country",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Route_Destination_City",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Packages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

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
                name: "LicensePlate",
                table: "Vehicles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MaintenanceRecords",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

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
    }
}
