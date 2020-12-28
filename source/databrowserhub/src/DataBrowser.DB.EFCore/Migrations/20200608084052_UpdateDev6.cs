using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBrowser.DB.EFCore.Migrations
{
    public partial class UpdateDev6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                "TtlCatalog",
                "Nodes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                "TtlDataflow",
                "Nodes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "TtlCatalog",
                "Nodes");

            migrationBuilder.DropColumn(
                "TtlDataflow",
                "Nodes");
        }
    }
}