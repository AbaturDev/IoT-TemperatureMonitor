using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TemperatureMonitor.Migrations
{
    /// <inheritdoc />
    public partial class change_measurementSnapshot_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Temperature",
                table: "MeasurementSnapshots",
                newName: "TemperatureMin");

            migrationBuilder.RenameColumn(
                name: "Humidity",
                table: "MeasurementSnapshots",
                newName: "TemperatureMax");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "MeasurementSnapshots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "HumidityAvg",
                table: "MeasurementSnapshots",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TemperatureAvg",
                table: "MeasurementSnapshots",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "MeasurementSnapshots");

            migrationBuilder.DropColumn(
                name: "HumidityAvg",
                table: "MeasurementSnapshots");

            migrationBuilder.DropColumn(
                name: "TemperatureAvg",
                table: "MeasurementSnapshots");

            migrationBuilder.RenameColumn(
                name: "TemperatureMin",
                table: "MeasurementSnapshots",
                newName: "Temperature");

            migrationBuilder.RenameColumn(
                name: "TemperatureMax",
                table: "MeasurementSnapshots",
                newName: "Humidity");
        }
    }
}
