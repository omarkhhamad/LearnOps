using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    public partial class add_enrollmentid_to_examresult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnrollmentId",
                table: "ExamResults",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_EnrollmentId",
                table: "ExamResults",
                column: "EnrollmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_Enrollments_EnrollmentId",
                table: "ExamResults",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "EnrollmentId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamResults_Enrollments_EnrollmentId",
                table: "ExamResults");

            migrationBuilder.DropIndex(
                name: "IX_ExamResults_EnrollmentId",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "EnrollmentId",
                table: "ExamResults");
        }
    }
}
