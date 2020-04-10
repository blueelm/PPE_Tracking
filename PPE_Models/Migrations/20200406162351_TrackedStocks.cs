using Microsoft.EntityFrameworkCore.Migrations;

namespace PPE_Models.Migrations
{
    public partial class TrackedStocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackedStocks",
                columns: table => new
                {
                    StockID = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedStocks", x => x.StockID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackedStocks");
        }
    }
}
