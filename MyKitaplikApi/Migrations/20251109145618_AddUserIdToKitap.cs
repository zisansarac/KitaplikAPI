using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyKitaplikApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToKitap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Kitaplar",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Kitaplar");
        }
    }
}
