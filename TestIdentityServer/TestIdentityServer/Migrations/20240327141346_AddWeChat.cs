using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestIdentityServer.Migrations
{
    /// <inheritdoc />
    public partial class AddWeChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WeChat",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeChat",
                table: "AspNetUsers");
        }
    }
}
