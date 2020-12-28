using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBrowser.DB.EFCore.Migrations
{
    public partial class DashBoard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Dashboards",
                table => new
                {
                    DashboardId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DashboardConfig = table.Column<string>(nullable: true),
                    UserFk = table.Column<int>(nullable: false),
                    HubFk = table.Column<int>(nullable: true),
                    TitleFK = table.Column<int>(nullable: false),
                    Weight = table.Column<int>(nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboards", x => x.DashboardId);
                    table.ForeignKey(
                        "FK_Dashboards_TransatableItem_TitleFK",
                        x => x.TitleFK,
                        "TransatableItem",
                        "TransatableItemId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                "DashboardNodes",
                table => new
                {
                    DashboardId = table.Column<int>(nullable: false),
                    NodeId = table.Column<int>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    Weight = table.Column<int>(nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardNodes", x => new {x.DashboardId, x.NodeId});
                    table.ForeignKey(
                        "FK_DashboardNodes_Dashboards_DashboardId",
                        x => x.DashboardId,
                        "Dashboards",
                        "DashboardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "DashboardViewTemplates",
                table => new
                {
                    DashboardId = table.Column<int>(nullable: false),
                    ViewTemplateId = table.Column<int>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    Weight = table.Column<int>(nullable: false, defaultValue: 0)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardViewTemplates", x => new {x.DashboardId, x.ViewTemplateId});
                    table.ForeignKey(
                        "FK_DashboardViewTemplates_Dashboards_DashboardId",
                        x => x.DashboardId,
                        "Dashboards",
                        "DashboardId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Dashboards_TitleFK",
                "Dashboards",
                "TitleFK",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "DashboardNodes");

            migrationBuilder.DropTable(
                "DashboardViewTemplates");

            migrationBuilder.DropTable(
                "Dashboards");
        }
    }
}