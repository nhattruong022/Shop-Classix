using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_Classix.Migrations
{
    /// <inheritdoc />
    public partial class CapNhatModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "productComments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "productComments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
