using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace donate.Migrations
{
    /// <inheritdoc />
    public partial class categoryname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoName",
                table: "Categorys",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Categorys",
                newName: "CategoName");
        }
    }
}
