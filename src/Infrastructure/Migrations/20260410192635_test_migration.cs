using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferOperation_AspNetUsers_UserId",
                table: "TransferOperation");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferPlaylist_TransferOperation_TransferOperationId",
                table: "TransferPlaylist");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferTrack_TransferPlaylist_TransferPlaylistId",
                table: "TransferTrack");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransferTrack",
                table: "TransferTrack");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransferPlaylist",
                table: "TransferPlaylist");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransferOperation",
                table: "TransferOperation");

            migrationBuilder.RenameTable(
                name: "TransferTrack",
                newName: "TransferTracks");

            migrationBuilder.RenameTable(
                name: "TransferPlaylist",
                newName: "TransferPlaylists");

            migrationBuilder.RenameTable(
                name: "TransferOperation",
                newName: "TransferOperations");

            migrationBuilder.RenameIndex(
                name: "IX_TransferTrack_TransferPlaylistId",
                table: "TransferTracks",
                newName: "IX_TransferTracks_TransferPlaylistId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferPlaylist_TransferOperationId",
                table: "TransferPlaylists",
                newName: "IX_TransferPlaylists_TransferOperationId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferOperation_UserId",
                table: "TransferOperations",
                newName: "IX_TransferOperations_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransferTracks",
                table: "TransferTracks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransferPlaylists",
                table: "TransferPlaylists",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransferOperations",
                table: "TransferOperations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferOperations_AspNetUsers_UserId",
                table: "TransferOperations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferPlaylists_TransferOperations_TransferOperationId",
                table: "TransferPlaylists",
                column: "TransferOperationId",
                principalTable: "TransferOperations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferTracks_TransferPlaylists_TransferPlaylistId",
                table: "TransferTracks",
                column: "TransferPlaylistId",
                principalTable: "TransferPlaylists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferOperations_AspNetUsers_UserId",
                table: "TransferOperations");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferPlaylists_TransferOperations_TransferOperationId",
                table: "TransferPlaylists");

            migrationBuilder.DropForeignKey(
                name: "FK_TransferTracks_TransferPlaylists_TransferPlaylistId",
                table: "TransferTracks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransferTracks",
                table: "TransferTracks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransferPlaylists",
                table: "TransferPlaylists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransferOperations",
                table: "TransferOperations");

            migrationBuilder.RenameTable(
                name: "TransferTracks",
                newName: "TransferTrack");

            migrationBuilder.RenameTable(
                name: "TransferPlaylists",
                newName: "TransferPlaylist");

            migrationBuilder.RenameTable(
                name: "TransferOperations",
                newName: "TransferOperation");

            migrationBuilder.RenameIndex(
                name: "IX_TransferTracks_TransferPlaylistId",
                table: "TransferTrack",
                newName: "IX_TransferTrack_TransferPlaylistId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferPlaylists_TransferOperationId",
                table: "TransferPlaylist",
                newName: "IX_TransferPlaylist_TransferOperationId");

            migrationBuilder.RenameIndex(
                name: "IX_TransferOperations_UserId",
                table: "TransferOperation",
                newName: "IX_TransferOperation_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransferTrack",
                table: "TransferTrack",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransferPlaylist",
                table: "TransferPlaylist",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransferOperation",
                table: "TransferOperation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferOperation_AspNetUsers_UserId",
                table: "TransferOperation",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferPlaylist_TransferOperation_TransferOperationId",
                table: "TransferPlaylist",
                column: "TransferOperationId",
                principalTable: "TransferOperation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferTrack_TransferPlaylist_TransferPlaylistId",
                table: "TransferTrack",
                column: "TransferPlaylistId",
                principalTable: "TransferPlaylist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
