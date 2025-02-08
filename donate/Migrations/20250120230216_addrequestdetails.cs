using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace donate.Migrations
{
    /// <inheritdoc />
    public partial class addrequestdetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Requestdetils",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Requestdetils",
                table: "Requests");
        }
    }
}
