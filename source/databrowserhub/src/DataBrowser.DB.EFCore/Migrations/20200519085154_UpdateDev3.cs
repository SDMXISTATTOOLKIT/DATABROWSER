using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBrowser.DB.EFCore.Migrations
{
    public partial class UpdateDev3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                "MaxCells",
                "Hubs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "MaxCells",
                "Hubs");
        }
    }
}