using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class _1skj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Users_UserId",
                schema: "Fleet",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_UserId",
                schema: "Fleet",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                schema: "Fleet",
                table: "Drivers");

            migrationBuilder.RenameTable(
                name: "Drivers",
                schema: "Fleet",
                newName: "Drivers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Drivers",
                newName: "Drivers",
                newSchema: "Fleet");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                schema: "Fleet",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_UserId",
                schema: "Fleet",
                table: "Drivers",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Users_UserId",
                schema: "Fleet",
                table: "Drivers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
