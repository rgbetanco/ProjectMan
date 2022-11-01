using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectman.Migrations
{
    public partial class init5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "company_id",
                table: "invoice",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                table: "invoice",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_invoice_company_id",
                table: "invoice",
                column: "company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_invoice_company_company_id",
                table: "invoice",
                column: "company_id",
                principalTable: "company",
                principalColumn: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoice_company_company_id",
                table: "invoice");

            migrationBuilder.DropIndex(
                name: "IX_invoice_company_id",
                table: "invoice");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "invoice");

            migrationBuilder.DropColumn(
                name: "created",
                table: "invoice");
        }
    }
}
