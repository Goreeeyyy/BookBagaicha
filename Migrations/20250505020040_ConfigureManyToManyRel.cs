using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookBagaicha.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureManyToManyRel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorBook_Authors_AuthorsAuthorId",
                table: "AuthorBook");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorBook_Books_BooksBookId",
                table: "AuthorBook");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorBook",
                table: "AuthorBook");

            migrationBuilder.RenameTable(
                name: "AuthorBook",
                newName: "BookAuthors");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorBook_BooksBookId",
                table: "BookAuthors",
                newName: "IX_BookAuthors_BooksBookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookAuthors",
                table: "BookAuthors",
                columns: new[] { "AuthorsAuthorId", "BooksBookId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookAuthors_Authors_AuthorsAuthorId",
                table: "BookAuthors",
                column: "AuthorsAuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookAuthors_Books_BooksBookId",
                table: "BookAuthors",
                column: "BooksBookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookAuthors_Authors_AuthorsAuthorId",
                table: "BookAuthors");

            migrationBuilder.DropForeignKey(
                name: "FK_BookAuthors_Books_BooksBookId",
                table: "BookAuthors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookAuthors",
                table: "BookAuthors");

            migrationBuilder.RenameTable(
                name: "BookAuthors",
                newName: "AuthorBook");

            migrationBuilder.RenameIndex(
                name: "IX_BookAuthors_BooksBookId",
                table: "AuthorBook",
                newName: "IX_AuthorBook_BooksBookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorBook",
                table: "AuthorBook",
                columns: new[] { "AuthorsAuthorId", "BooksBookId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorBook_Authors_AuthorsAuthorId",
                table: "AuthorBook",
                column: "AuthorsAuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorBook_Books_BooksBookId",
                table: "AuthorBook",
                column: "BooksBookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
