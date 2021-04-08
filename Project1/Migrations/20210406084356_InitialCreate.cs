using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Project1.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Category_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Min = table.Column<int>(type: "int", nullable: false),
                    Max = table.Column<int>(type: "int", nullable: false),
                    PhotoFile = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageMimeType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Category_ID);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Vehicle_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Owners_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Manufacturer = table.Column<int>(type: "int", nullable: false),
                    Year_of_Manufacture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Vehicle_ID);
                    table.ForeignKey(
                        name: "FK_Vehicles_Categories_Category_ID",
                        column: x => x.Category_ID,
                        principalTable: "Categories",
                        principalColumn: "Category_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Category_ID",
                table: "Vehicles",
                column: "Category_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
