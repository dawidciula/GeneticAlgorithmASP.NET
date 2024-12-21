using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genetic_algorithm.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedParameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OptimizationType",
                table: "OptimizationParameters",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CrossoverPoints",
                table: "OptimizationParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxGenerations",
                table: "OptimizationParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxStagnation",
                table: "OptimizationParameters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrossoverPoints",
                table: "OptimizationParameters");

            migrationBuilder.DropColumn(
                name: "MaxGenerations",
                table: "OptimizationParameters");

            migrationBuilder.DropColumn(
                name: "MaxStagnation",
                table: "OptimizationParameters");

            migrationBuilder.AlterColumn<int>(
                name: "OptimizationType",
                table: "OptimizationParameters",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
