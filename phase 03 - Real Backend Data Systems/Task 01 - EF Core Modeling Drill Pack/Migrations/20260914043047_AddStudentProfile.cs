using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_01___EF_Core_Modeling_Drill_Pack.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentsProfile",
                columns: table => new
                {
                    SSN = table.Column<string>(type: "nvarchar(14)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    EmergencyPhone = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    BirthOfDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentsProfile", x => x.SSN);
                    table.ForeignKey(
                        name: "FK_StudentsProfile_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentsProfile_StudentId",
                table: "StudentsProfile",
                column: "StudentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentsProfile");
        }
    }
}
