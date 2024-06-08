using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeadHunter.Migrations
{
    /// <inheritdoc />
    public partial class AddedVacancyPublished : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Published",
                table: "Vacancies",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Published",
                table: "Vacancies");
        }
    }
}
