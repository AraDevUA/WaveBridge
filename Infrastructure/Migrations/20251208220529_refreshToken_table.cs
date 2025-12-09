using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class refreshToken_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                table: "UserOAuthConnections");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "RefreshTokens",
                newName: "TokenHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "RefreshTokens",
                newName: "Token");

            migrationBuilder.AddColumn<int>(
                name: "Provider",
                table: "UserOAuthConnections",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
