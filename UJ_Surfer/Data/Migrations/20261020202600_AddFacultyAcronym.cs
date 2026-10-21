using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UJ_Surfer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFacultyAcronym : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Acronym",
                table: "Faculties",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acronym",
                table: "Faculties");
        }
    }
}
