using Microsoft.EntityFrameworkCore.Migrations;

namespace Users.API.Infrastructure.Migrations
{
    public partial class UpdateAuditableColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "AspNetUsers",
                newName: "ModifiedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "AspNetUsers",
                newName: "LastModified");
        }
    }
}
