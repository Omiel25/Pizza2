using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza2.Migrations
{
    public partial class AddedKeyToIngridientsList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "recordId",
                table: "PizzaIngridients",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PizzaIngridients",
                table: "PizzaIngridients",
                column: "recordId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PizzaIngridients",
                table: "PizzaIngridients");

            migrationBuilder.DropColumn(
                name: "recordId",
                table: "PizzaIngridients");
        }
    }
}
