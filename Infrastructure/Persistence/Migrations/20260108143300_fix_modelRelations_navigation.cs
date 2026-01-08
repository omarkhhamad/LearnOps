using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fix_modelRelations_navigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Enrollments_EnrollmentId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Students_StudentId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_ClassGroups_ClassGroupGroupId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_ClassGroups_ClassGroupGroupId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_ClassGroupGroupId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_ClassGroupGroupId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "ClassGroupGroupId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "ClassGroupGroupId",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "ResultId",
                table: "ExamResults",
                newName: "ExamResultId");

            migrationBuilder.AlterColumn<string>(
                name: "Result",
                table: "ExamResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_GroupId",
                table: "Exams",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_GroupId",
                table: "Enrollments",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Enrollments_EnrollmentId",
                table: "Certificates",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "EnrollmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Students_StudentId",
                table: "Certificates",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_ClassGroups_GroupId",
                table: "Enrollments",
                column: "GroupId",
                principalTable: "ClassGroups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_ClassGroups_GroupId",
                table: "Exams",
                column: "GroupId",
                principalTable: "ClassGroups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Enrollments_EnrollmentId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_Students_StudentId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_ClassGroups_GroupId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_ClassGroups_GroupId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Exams_GroupId",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_GroupId",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "ExamResultId",
                table: "ExamResults",
                newName: "ResultId");

            migrationBuilder.AddColumn<int>(
                name: "ClassGroupGroupId",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Result",
                table: "ExamResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClassGroupGroupId",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Exams_ClassGroupGroupId",
                table: "Exams",
                column: "ClassGroupGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_ClassGroupGroupId",
                table: "Enrollments",
                column: "ClassGroupGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Enrollments_EnrollmentId",
                table: "Certificates",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "EnrollmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_Students_StudentId",
                table: "Certificates",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_ClassGroups_ClassGroupGroupId",
                table: "Enrollments",
                column: "ClassGroupGroupId",
                principalTable: "ClassGroups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_ClassGroups_ClassGroupGroupId",
                table: "Exams",
                column: "ClassGroupGroupId",
                principalTable: "ClassGroups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
