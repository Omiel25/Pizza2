using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza2.Migrations
{
    public partial class ChangedIngridientProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsVege",
                table: "Ingridients",
                newName: "ContainsMeat");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContainsMeat",
                table: "Ingridients",
                newName: "IsVege");
        }
    }
}
