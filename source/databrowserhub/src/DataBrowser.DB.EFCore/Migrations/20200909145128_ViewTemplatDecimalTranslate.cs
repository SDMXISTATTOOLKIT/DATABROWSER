using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBrowser.DB.EFCore.Migrations
{
    public partial class ViewTemplatDecimalTranslate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "ViewTemplates",
                table => new
                {
                    ViewTemplateId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DatasetId = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    DefaultView = table.Column<string>(nullable: true),
                    Criteria = table.Column<string>(nullable: true),
                    Layouts = table.Column<string>(nullable: true),
                    HiddenDimensions = table.Column<string>(nullable: true),
                    BlockAxes = table.Column<bool>(nullable: false),
                    EnableCriteria = table.Column<bool>(nullable: false),
                    EnableVariation = table.Column<bool>(nullable: false),
                    DecimalNumber = table.Column<int>(nullable: false),
                    ViewTemplateCreationDate = table.Column<DateTime>(nullable: false),
                    TitleFK = table.Column<int>(nullable: true),
                    NodeFK = table.Column<int>(nullable: false),
                    UserFK = table.Column<int>(nullable: true),
                    DecimalSeparatorFk = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewTemplates", x => x.ViewTemplateId);
                    table.ForeignKey(
                        "FK_ViewTemplates_TransatableItem_DecimalSeparatorFk",
                        x => x.DecimalSeparatorFk,
                        "TransatableItem",
                        "TransatableItemId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        "FK_ViewTemplates_TransatableItem_TitleFK",
                        x => x.TitleFK,
                        "TransatableItem",
                        "TransatableItemId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                "IX_ViewTemplates_DecimalSeparatorFk",
                "ViewTemplates",
                "DecimalSeparatorFk",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_ViewTemplates_TitleFK",
                "ViewTemplates",
                "TitleFK",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "ViewTemplates");
        }
    }
}