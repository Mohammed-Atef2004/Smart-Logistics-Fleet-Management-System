using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialInsuranceClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InsuranceClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ClaimAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    ClaimCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    ApprovedCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DocFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DocFileUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DocContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DocFileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    DocUploadedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClaimItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UnitAmount = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimItems_InsuranceClaims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "InsuranceClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimItems_ClaimId",
                table: "ClaimItems",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_ClaimNumber",
                table: "InsuranceClaims",
                column: "ClaimNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_CustomerId",
                table: "InsuranceClaims",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_ShipmentId",
                table: "InsuranceClaims",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_Status",
                table: "InsuranceClaims",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_SubmittedAt",
                table: "InsuranceClaims",
                column: "SubmittedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimItems");

            migrationBuilder.DropTable(
                name: "InsuranceClaims");
        }
    }
}
