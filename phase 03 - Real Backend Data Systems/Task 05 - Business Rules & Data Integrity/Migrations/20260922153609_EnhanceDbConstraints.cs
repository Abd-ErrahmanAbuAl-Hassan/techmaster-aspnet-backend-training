using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task_05_Business_Rules_Data_Integrity.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceDbConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "TrainingTracks",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalGrade",
                table: "Enrollments",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_TrainingTracks_Capacity_Positive",
                table: "TrainingTracks",
                sql: "[Capacity] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TrainingTracks_DateRange",
                table: "TrainingTracks",
                sql: "[EndDate] > [StartDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TrainingTracks_Price_Positive",
                table: "TrainingTracks",
                sql: "[Price] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Students_PhoneNumber",
                table: "Students",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_Amount_Positive",
                table: "Payments",
                sql: "[Amount] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_PhoneNumber",
                table: "Instructors",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_TrainingTrackId",
                table: "Enrollments",
                columns: new[] { "StudentId", "TrainingTrackId" },
                unique: true,
                filter: "[Status] <> 3");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Enrollments_FinalGrade",
                table: "Enrollments",
                sql: "[FinalGrade] >= 0 AND [FinalGrade] <= 100");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Enrollments_ProgressPercentage",
                table: "Enrollments",
                sql: "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_TrainingTracks_Capacity_Positive",
                table: "TrainingTracks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TrainingTracks_DateRange",
                table: "TrainingTracks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TrainingTracks_Price_Positive",
                table: "TrainingTracks");

            migrationBuilder.DropIndex(
                name: "IX_Students_PhoneNumber",
                table: "Students");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_Amount_Positive",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Instructors_PhoneNumber",
                table: "Instructors");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId_TrainingTrackId",
                table: "Enrollments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Enrollments_FinalGrade",
                table: "Enrollments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Enrollments_ProgressPercentage",
                table: "Enrollments");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "TrainingTracks",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Students",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<double>(
                name: "FinalGrade",
                table: "Enrollments",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");
        }
    }
}
