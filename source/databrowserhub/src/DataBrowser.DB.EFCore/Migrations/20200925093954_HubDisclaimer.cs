using Microsoft.EntityFrameworkCore.Migrations;
using SQLitePCL;

namespace DataBrowser.DB.EFCore.Migrations
{
    public partial class HubDisclaimer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsSqlite())
            {
                migrationBuilder.Sql("ALTER TABLE Hubs ADD COLUMN DisclaimerFk INTEGER NULL REFERENCES TransatableItem(TransatableItemId);");
            }
            else
            {
                migrationBuilder.AddColumn<int>(
                    name: "DisclaimerFk",
                    table: "Hubs",
                    nullable: true);

                migrationBuilder.CreateIndex(
                    name: "IX_Hubs_DisclaimerFk",
                    table: "Hubs",
                    column: "DisclaimerFk",
                    unique: true);

                migrationBuilder.AddForeignKey(
                    name: "FK_Hubs_TransatableItem_DisclaimerFk",
                    table: "Hubs",
                    column: "DisclaimerFk",
                    principalTable: "TransatableItem",
                    principalColumn: "TransatableItemId",
                    onDelete: ReferentialAction.SetNull);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hubs_TransatableItem_DisclaimerFk",
                table: "Hubs");

            migrationBuilder.DropIndex(
                name: "IX_Hubs_DisclaimerFk",
                table: "Hubs");

            migrationBuilder.DropColumn(
                name: "DisclaimerFk",
                table: "Hubs");
        }
    }
}
