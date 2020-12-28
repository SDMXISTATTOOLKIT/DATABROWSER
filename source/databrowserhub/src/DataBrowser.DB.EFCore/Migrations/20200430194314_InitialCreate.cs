using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBrowser.DB.EFCore.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "TransatableItem",
                table => new
                {
                    TransatableItemId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_TransatableItem", x => x.TransatableItemId); });

            migrationBuilder.CreateTable(
                "Hubs",
                table => new
                {
                    HubId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LogoURL = table.Column<string>(nullable: true),
                    BackgroundMediaURL = table.Column<string>(nullable: true),
                    SupportedLanguages = table.Column<string>(nullable: true),
                    DefaultLanguage = table.Column<string>(nullable: true),
                    MaxObservationsAfterCriteria = table.Column<int>(nullable: false),
                    DecimalSeparator = table.Column<string>(nullable: true),
                    DecimalNumber = table.Column<int>(nullable: false),
                    EmptyCellDefaultValue = table.Column<string>(nullable: true),
                    DefaultView = table.Column<string>(nullable: true),
                    TitleFk = table.Column<int>(nullable: true),
                    SloganFk = table.Column<int>(nullable: true),
                    DescriptionFk = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hubs", x => x.HubId);
                    table.ForeignKey(
                        "FK_Hubs_TransatableItem_DescriptionFk",
                        x => x.DescriptionFk,
                        "TransatableItem",
                        "TransatableItemId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        "FK_Hubs_TransatableItem_SloganFk",
                        x => x.SloganFk,
                        "TransatableItem",
                        "TransatableItemId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        "FK_Hubs_TransatableItem_TitleFk",
                        x => x.TitleFk,
                        "TransatableItem",
                        "TransatableItemId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                "Nodes",
                table => new
                {
                    NodeId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Agency = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Logo = table.Column<string>(nullable: true),
                    EndPoint = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    EnableHttpAuth = table.Column<bool>(nullable: false),
                    AuthHttpUsername = table.Column<string>(nullable: true),
                    AuthHttpPassword = table.Column<string>(nullable: true),
                    EnableProxy = table.Column<bool>(nullable: false),
                    UseProxySystem = table.Column<bool>(nullable: false),
                    ProxyAddress = table.Column<string>(nullable: true),
                    ProxyPort = table.Column<int>(nullable: false),
                    ProxyUsername = table.Column<string>(nullable: true),
                    ProxyPassword = table.Column<string>(nullable: true),
                    BackgroundMediaURL = table.Column<string>(nullable: true),
                    EmptyCellDefaultValue = table.Column<string>(nullable: true),
                    DefaultView = table.Column<string>(nullable: true),
                    ShowDataflowUncategorized = table.Column<bool>(nullable: false),
                    CriteriaSelectionMode = table.Column<string>(nullable: true),
                    LabelDimensionTerritorial = table.Column<string>(nullable: true),
                    LabelDimensionTemporal = table.Column<string>(nullable: true),
                    CategorySchemaExcludes = table.Column<string>(nullable: true),
                    EndPointFormatSupported = table.Column<string>(nullable: true),
                    DecimalNumber = table.Column<int>(nullable: true),
                    TitleFk = table.Column<int>(nullable: true),
                    SloganFk = table.Column<int>(nullable: true),
                    DescriptionFk = table.Column<int>(nullable: true),
                    DecimalSeparatorFk = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.NodeId);
                    table.ForeignKey(
                        "FK_Nodes_TransatableItem_DecimalSeparatorFk",
                        x => x.DecimalSeparatorFk,
                        "TransatableItem",
                        "TransatableItemId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        "FK_Nodes_TransatableItem_DescriptionFk",
                        x => x.DescriptionFk,
                        "TransatableItem",
                        "TransatableItemId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        "FK_Nodes_TransatableItem_SloganFk",
                        x => x.SloganFk,
                        "TransatableItem",
                        "TransatableItemId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        "FK_Nodes_TransatableItem_TitleFk",
                        x => x.TitleFk,
                        "TransatableItem",
                        "TransatableItemId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                "TransatableItemValue",
                table => new
                {
                    Language = table.Column<string>(nullable: false),
                    TransatableItemFk = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransatableItemValue", x => new {x.TransatableItemFk, x.Language});
                    table.ForeignKey(
                        "FK_TransatableItemValue_TransatableItem_TransatableItemFk",
                        x => x.TransatableItemFk,
                        "TransatableItem",
                        "TransatableItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Extra",
                table => new
                {
                    ExtraId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    ValueType = table.Column<string>(nullable: true),
                    IsPublic = table.Column<bool>(nullable: false),
                    TransatableItemFk = table.Column<int>(nullable: true),
                    NodeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Extra", x => x.ExtraId);
                    table.ForeignKey(
                        "FK_Extra_Nodes_NodeId",
                        x => x.NodeId,
                        "Nodes",
                        "NodeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Extra_TransatableItem_TransatableItemFk",
                        x => x.TransatableItemFk,
                        "TransatableItem",
                        "TransatableItemId",
                        onDelete: ReferentialAction.SetNull);
                });


            migrationBuilder.CreateIndex(
                "IX_Extra_NodeId",
                "Extra",
                "NodeId");

            migrationBuilder.CreateIndex(
                "IX_Extra_TransatableItemFk",
                "Extra",
                "TransatableItemFk",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Hubs_DescriptionFk",
                "Hubs",
                "DescriptionFk",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Hubs_SloganFk",
                "Hubs",
                "SloganFk",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Hubs_TitleFk",
                "Hubs",
                "TitleFk",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Nodes_Code",
                "Nodes",
                "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Nodes_DecimalSeparatorFk",
                "Nodes",
                "DecimalSeparatorFk",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Nodes_DescriptionFk",
                "Nodes",
                "DescriptionFk",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Nodes_SloganFk",
                "Nodes",
                "SloganFk",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Nodes_TitleFk",
                "Nodes",
                "TitleFk",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Extra");

            migrationBuilder.DropTable(
                "Hubs");

            migrationBuilder.DropTable(
                "TransatableItemValue");

            migrationBuilder.DropTable(
                "Nodes");

            migrationBuilder.DropTable(
                "TransatableItem");
        }
    }
}