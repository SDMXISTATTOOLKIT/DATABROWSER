using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBrowser.DB.EFCore.Migrations
{
    public partial class UpdateDev5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "AuthHttpDomain",
                "Nodes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "AuthHttpDomain",
                "Nodes");
        }
    }
}