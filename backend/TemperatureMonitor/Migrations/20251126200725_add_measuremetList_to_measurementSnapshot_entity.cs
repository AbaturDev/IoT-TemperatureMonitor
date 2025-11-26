using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TemperatureMonitor.Migrations
{
    /// <inheritdoc />
    public partial class add_measuremetList_to_measurementSnapshot_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MeasurementSnapshotId",
                table: "Measurements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_MeasurementSnapshotId",
                table: "Measurements",
                column: "MeasurementSnapshotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_MeasurementSnapshots_MeasurementSnapshotId",
                table: "Measurements",
                column: "MeasurementSnapshotId",
                principalTable: "MeasurementSnapshots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_MeasurementSnapshots_MeasurementSnapshotId",
                table: "Measurements");

            migrationBuilder.DropIndex(
                name: "IX_Measurements_MeasurementSnapshotId",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "MeasurementSnapshotId",
                table: "Measurements");
        }
    }
}
