using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBrowser.DB.EFCore.Migrations
{
    public partial class UpdateDevHubExtras : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "Extras",
                "Hubs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Extras",
                "Hubs");
        }
    }
}