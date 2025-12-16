using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineFlix_Models.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FilmGenres",
                table: "FilmGenres");

            migrationBuilder.AlterColumn<int>(
                name: "FilmGenreId",
                table: "FilmGenres",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FilmGenres",
                table: "FilmGenres",
                column: "FilmGenreId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmGenres_FilmId",
                table: "FilmGenres",
                column: "FilmId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FilmGenres",
                table: "FilmGenres");

            migrationBuilder.DropIndex(
                name: "IX_FilmGenres_FilmId",
                table: "FilmGenres");

            migrationBuilder.AlterColumn<int>(
                name: "FilmGenreId",
                table: "FilmGenres",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FilmGenres",
                table: "FilmGenres",
                columns: new[] { "FilmId", "GenreId" });
        }
    }
}
