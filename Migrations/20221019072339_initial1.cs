using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace repairman.Migrations
{
    public partial class initial1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "invoice",
                table: "incoming_payment",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "invoice",
                table: "incoming_payment");
        }
    }
}
