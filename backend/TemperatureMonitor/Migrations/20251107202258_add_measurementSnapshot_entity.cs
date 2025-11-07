using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TemperatureMonitor.Migrations
{
    /// <inheritdoc />
    public partial class add_measurementSnapshot_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasurementSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    Humidity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementSnapshots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementSnapshots_Id",
                table: "MeasurementSnapshots",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasurementSnapshots");
        }
    }
}
