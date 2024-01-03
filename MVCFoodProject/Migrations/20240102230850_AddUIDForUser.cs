using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVCFoodProject.Migrations
{
    public partial class AddUIDForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UID",
                table: "User",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UID",
                table: "User");
        }
    }
}
