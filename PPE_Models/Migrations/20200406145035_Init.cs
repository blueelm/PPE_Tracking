using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PPE_Models.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockHistory",
                columns: table => new
                {
                    StockID = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Quantity = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockHistory", x => new { x.StockID, x.DateTime });
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    StockID = table.Column<string>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    StockNumber = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Inventory = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    Manufacturer = table.Column<string>(nullable: true),
                    Packaging = table.Column<string>(nullable: true),
                    UnitOfIssue = table.Column<string>(nullable: true),
                    Quantity = table.Column<long>(nullable: false),
                    QuantityBackordered = table.Column<long>(nullable: false),
                    UnitOfIssueUnitSmallest = table.Column<long>(nullable: false),
                    Vendor = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.StockID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockHistory");

            migrationBuilder.DropTable(
                name: "Stocks");
        }
    }
}
