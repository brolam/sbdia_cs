using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Data.Migrations
{
    public partial class CreateSensorCost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SensorCost",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SensorId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(maxLength: 15, nullable: false),
                    Value = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorCost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorCost_Sensor_SensorId",
                        column: x => x.SensorId,
                        principalTable: "Sensor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SensorCost_SensorId",
                table: "SensorCost",
                column: "SensorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorCost");
        }
    }
}
