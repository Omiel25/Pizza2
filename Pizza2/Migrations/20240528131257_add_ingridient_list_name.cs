using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza2.Migrations
{
    public partial class add_ingridient_list_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IngridientListName",
                table: "PizzaIngridients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IngridientListName",
                table: "PizzaIngridients");
        }
    }
}
