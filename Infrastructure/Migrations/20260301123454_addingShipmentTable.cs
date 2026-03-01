using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addingShipmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RecipientPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DestinationAddress_Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationAddress_State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationAddress_ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationAddress_Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationAddress_ApartmentUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tracking_TrackingNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tracking_Status = table.Column<int>(type: "int", nullable: false),
                    Tracking_StatusDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tracking_UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tracking_CarrierName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tracking_EstimatedDeliveryDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SpecialInstructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Weight_Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight_Unit = table.Column<int>(type: "int", nullable: false),
                    Dimensions_Length = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Dimensions_Width = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Dimensions_Height = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Dimensions_Unit = table.Column<int>(type: "int", nullable: false),
                    ContentCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFragile = table.Column<bool>(type: "bit", nullable: false),
                    RequiresRefrigeration = table.Column<bool>(type: "bit", nullable: false),
                    DeclaredValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentPackages_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentRoutePoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentRoutePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentRoutePoints_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentPackages_ShipmentId",
                table: "ShipmentPackages",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentRoutePoints_ShipmentId",
                table: "ShipmentRoutePoints",
                column: "ShipmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentPackages");

            migrationBuilder.DropTable(
                name: "ShipmentRoutePoints");

            migrationBuilder.DropTable(
                name: "Shipments");
        }
    }
}
