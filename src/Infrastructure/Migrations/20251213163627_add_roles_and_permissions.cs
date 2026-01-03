using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_roles_and_permissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "UserStreamingConnections",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AccessToken",
                table: "UserStreamingConnections",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "UserStreamingConnections",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "UserOAuthConnections",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccessToken",
                table: "UserOAuthConnections",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "UserOAuthConnections",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TokenHash",
                table: "RefreshTokens",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRevoked",
                table: "RefreshTokens",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedUtc",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "timezone('utc', now())",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedUtc",
                table: "AspNetRoles",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "timezone('utc', now())",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransferOperation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceService = table.Column<int>(type: "integer", nullable: false),
                    TargetService = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferOperation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferOperation_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferOperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceId = table.Column<string>(type: "text", nullable: false),
                    TargetId = table.Column<string>(type: "text", nullable: false),
                    TrackName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Artist = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Album = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TransferOperationId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferItem_TransferOperation_TransferOperationId",
                        column: x => x.TransferOperationId,
                        principalTable: "TransferOperation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferItem_TransferOperation_TransferOperationId1",
                        column: x => x.TransferOperationId1,
                        principalTable: "TransferOperation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserStreamingConnections_UserId1",
                table: "UserStreamingConnections",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserOAuthConnections_UserId1",
                table: "UserOAuthConnections",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferItem_TransferOperationId",
                table: "TransferItem",
                column: "TransferOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferItem_TransferOperationId1",
                table: "TransferItem",
                column: "TransferOperationId1");

            migrationBuilder.CreateIndex(
                name: "IX_TransferOperation_UserId",
                table: "TransferOperation",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserOAuthConnections_AspNetUsers_UserId1",
                table: "UserOAuthConnections",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStreamingConnections_AspNetUsers_UserId1",
                table: "UserStreamingConnections",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserOAuthConnections_AspNetUsers_UserId1",
                table: "UserOAuthConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStreamingConnections_AspNetUsers_UserId1",
                table: "UserStreamingConnections");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "TransferItem");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "TransferOperation");

            migrationBuilder.DropIndex(
                name: "IX_UserStreamingConnections_UserId1",
                table: "UserStreamingConnections");

            migrationBuilder.DropIndex(
                name: "IX_UserOAuthConnections_UserId1",
                table: "UserOAuthConnections");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserStreamingConnections");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserOAuthConnections");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "UserStreamingConnections",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "AccessToken",
                table: "UserStreamingConnections",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "UserOAuthConnections",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccessToken",
                table: "UserOAuthConnections",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "TokenHash",
                table: "RefreshTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRevoked",
                table: "RefreshTokens",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedUtc",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "timezone('utc', now())");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedUtc",
                table: "AspNetRoles",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "timezone('utc', now())");
        }
    }
}
