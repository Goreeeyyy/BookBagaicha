using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookBagaicha.Migrations
{
    /// <inheritdoc />
    public partial class Newdiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AppliedDiscountIsLoyalty",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppliedDiscountIsLoyalty",
                table: "Orders");
        }
    }
}
