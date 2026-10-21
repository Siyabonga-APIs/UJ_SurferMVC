using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UJ_Surfer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentMatricProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentMatricProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EnglishMark = table.Column<int>(type: "int", nullable: false),
                    SelectedMathType = table.Column<int>(type: "int", nullable: false),
                    MathMark = table.Column<int>(type: "int", nullable: false),
                    PhysicalScienceMark = table.Column<int>(type: "int", nullable: true),
                    LifeScienceMark = table.Column<int>(type: "int", nullable: true),
                    Subject5Mark = table.Column<int>(type: "int", nullable: true),
                    Subject6Mark = table.Column<int>(type: "int", nullable: true),
                    LifeOrientationMark = table.Column<int>(type: "int", nullable: true),
                    CalculatedAps = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentMatricProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentMatricProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentMatricProfiles_UserId",
                table: "StudentMatricProfiles",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentMatricProfiles");
        }
    }
}
