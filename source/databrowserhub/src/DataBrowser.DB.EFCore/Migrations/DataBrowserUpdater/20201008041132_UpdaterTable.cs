using System;
using DataBrowser.Interfaces;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBrowser.DB.EFCore.Migrations.DataBrowserUpdater
{
    public partial class UpdaterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataBrowserVersion",
                columns: table => new
                {
                    DataBrowserVersionId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    From = table.Column<DateTime>(nullable: false),
                    Major = table.Column<int>(nullable: false),
                    Minor = table.Column<int>(nullable: false),
                    Build = table.Column<int>(nullable: false),
                    Revision = table.Column<int>(nullable: false),
                    IsCurrentVersion = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataBrowserVersion", x => x.DataBrowserVersionId);
                });

            migrationBuilder.CreateTable(
                name: "DataBrowserVersionActionUpgraders",
                columns: table => new
                {
                    DataBrowserVersionActionUpgraderId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExecutionDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Success = table.Column<bool>(nullable: false),
                    Errors = table.Column<string>(nullable: true),
                    DataBrowserVersionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataBrowserVersionActionUpgraders", x => x.DataBrowserVersionActionUpgraderId);
                });

            migrationBuilder.InsertData(
                table: "DataBrowserVersion",
                columns: new[] { "From", "Major", "Minor", "Build", "Revision", "IsCurrentVersion" },
                values: new object[] { DateTime.UtcNow, VersionDataBrowser.Current.Major,
                                        VersionDataBrowser.Current.Minor, VersionDataBrowser.Current.Build,
                                        VersionDataBrowser.Current.Revision, true});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataBrowserVersion");

            migrationBuilder.DropTable(
                name: "DataBrowserVersionActionUpgraders");
        }
    }
}
