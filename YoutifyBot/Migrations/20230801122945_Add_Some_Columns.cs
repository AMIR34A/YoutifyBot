using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YoutifyBot.Migrations
{
    /// <inheritdoc />
    public partial class Add_Some_Columns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaximumDownloadVolume",
                table: "Users",
                newName: "TotalDownloadVolume");

            migrationBuilder.RenameColumn(
                name: "TotalCountDownload",
                table: "Rules",
                newName: "BaseDownloadVolume");

            migrationBuilder.AddColumn<int>(
                name: "BaseCountDownload",
                table: "Rules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseCountDownload",
                table: "Rules");

            migrationBuilder.RenameColumn(
                name: "TotalDownloadVolume",
                table: "Users",
                newName: "MaximumDownloadVolume");

            migrationBuilder.RenameColumn(
                name: "BaseDownloadVolume",
                table: "Rules",
                newName: "TotalCountDownload");
        }
    }
}
