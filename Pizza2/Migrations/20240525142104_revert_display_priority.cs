using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza2.Migrations
{
    public partial class revert_display_priority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<decimal>(
            //    name: "DisplayPriority",
            //    table: "Ingridients",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<int>(
            //    name: "DisplayPriority",
            //    table: "Ingridients",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldType: "decimal(18,2)");
        }
    }
}
