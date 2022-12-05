using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza2.Migrations
{
    public partial class MenuChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "itemType",
                table: "Menu");

            migrationBuilder.RenameColumn(
                name: "itemId",
                table: "Menu",
                newName: "PizzaId");

            migrationBuilder.RenameColumn(
                name: "activeMenu",
                table: "Menu",
                newName: "IsActive");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PizzaId",
                table: "Menu",
                newName: "itemId");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Menu",
                newName: "activeMenu");

            migrationBuilder.AddColumn<string>(
                name: "itemType",
                table: "Menu",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
