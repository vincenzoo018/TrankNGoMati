using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackNGoMati.Migrations
{
    /// <inheritdoc />
    public partial class AddCitizenFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_SubmittedByUserId",
                table: "Documents");

            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtractedText",
                table: "DocumentMetadatas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CitizenFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrackingNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DateSubmitted = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenFeedbacks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_AssignedToUserId",
                table: "Documents",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_AssignedToUserId",
                table: "Documents",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_SubmittedByUserId",
                table: "Documents",
                column: "SubmittedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_AssignedToUserId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_SubmittedByUserId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "CitizenFeedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Documents_AssignedToUserId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ExtractedText",
                table: "DocumentMetadatas");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_SubmittedByUserId",
                table: "Documents",
                column: "SubmittedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
