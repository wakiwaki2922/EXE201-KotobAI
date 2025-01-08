using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CursusJapaneseLearningPlatform.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Flashcard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaUrl",
                table: "Flashcards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MediaUrl",
                table: "Flashcards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
