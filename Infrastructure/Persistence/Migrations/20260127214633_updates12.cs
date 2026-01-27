using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updates12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackingUpdate_Shipments_ShipmentId",
                table: "TrackingUpdate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrackingUpdate",
                table: "TrackingUpdate");

            migrationBuilder.EnsureSchema(
                name: "Shipment");

            migrationBuilder.RenameTable(
                name: "TrackingUpdate",
                newName: "TrackingUpdates",
                newSchema: "Shipment");

            migrationBuilder.RenameIndex(
                name: "IX_TrackingUpdate_ShipmentId",
                schema: "Shipment",
                table: "TrackingUpdates",
                newName: "IX_TrackingUpdates_ShipmentId");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                schema: "Shipment",
                table: "TrackingUpdates",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                schema: "Shipment",
                table: "TrackingUpdates",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrackingUpdates",
                schema: "Shipment",
                table: "TrackingUpdates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackingUpdates_Shipments_ShipmentId",
                schema: "Shipment",
                table: "TrackingUpdates",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackingUpdates_Shipments_ShipmentId",
                schema: "Shipment",
                table: "TrackingUpdates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrackingUpdates",
                schema: "Shipment",
                table: "TrackingUpdates");

            migrationBuilder.RenameTable(
                name: "TrackingUpdates",
                schema: "Shipment",
                newName: "TrackingUpdate");

            migrationBuilder.RenameIndex(
                name: "IX_TrackingUpdates_ShipmentId",
                table: "TrackingUpdate",
                newName: "IX_TrackingUpdate_ShipmentId");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "TrackingUpdate",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "TrackingUpdate",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrackingUpdate",
                table: "TrackingUpdate",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackingUpdate_Shipments_ShipmentId",
                table: "TrackingUpdate",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
