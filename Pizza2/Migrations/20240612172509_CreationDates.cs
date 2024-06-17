using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza2.Migrations
{
    public partial class CreationDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "menuName",
                table: "Menu",
                newName: "MenuName");

            migrationBuilder.RenameColumn(
                name: "menuId",
                table: "Menu",
                newName: "MenuId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Pizzas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Pizzas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "PizzaIngridients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcceptingUser",
                table: "OrdersHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Menu",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Menu",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Ingridients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Ingridients",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Pizzas");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Pizzas");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "PizzaIngridients");

            migrationBuilder.DropColumn(
                name: "AcceptingUser",
                table: "OrdersHistory");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Menu");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Menu");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Ingridients");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Ingridients");

            migrationBuilder.RenameColumn(
                name: "MenuName",
                table: "Menu",
                newName: "menuName");

            migrationBuilder.RenameColumn(
                name: "MenuId",
                table: "Menu",
                newName: "menuId");
        }
    }
}
