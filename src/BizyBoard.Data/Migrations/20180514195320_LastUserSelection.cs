using Microsoft.EntityFrameworkCore.Migrations;

namespace BizyBoard.Data.Migrations
{
    public partial class LastUserSelection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastErpCompanyId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastErpFiscalYear",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastErpCompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastErpFiscalYear",
                table: "AspNetUsers");
        }
    }
}
