using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnHub.Infrastructure.Data.MigraSrc
{
    /// <inheritdoc />
    public partial class AddSectionQuizzesAndFinalExams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SectionId",
                table: "Quizzes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Quizzes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Section");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CourseId_SectionId_Type",
                table: "Quizzes",
                columns: new[] { "CourseId", "SectionId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_SectionId",
                table: "Quizzes",
                column: "SectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Sections_SectionId",
                table: "Quizzes",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Sections_SectionId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_CourseId_SectionId_Type",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_SectionId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Quizzes");
        }
    }
}
