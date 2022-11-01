using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectman.Migrations
{
    public partial class init1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invoice",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    issue_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "invoice_item",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    invoice_id = table.Column<long>(type: "bigint", nullable: true),
                    incoming_payment_id = table.Column<long>(type: "bigint", nullable: true),
                    amount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_item", x => x.ID);
                    table.ForeignKey(
                        name: "FK_invoice_item_incoming_payment_incoming_payment_id",
                        column: x => x.incoming_payment_id,
                        principalTable: "incoming_payment",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_invoice_item_invoice_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "invoice",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_invoice_item_incoming_payment_id",
                table: "invoice_item",
                column: "incoming_payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_item_invoice_id",
                table: "invoice_item",
                column: "invoice_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invoice_item");

            migrationBuilder.DropTable(
                name: "invoice");
        }
    }
}
