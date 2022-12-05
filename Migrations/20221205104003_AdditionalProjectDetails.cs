using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectman.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalProjectDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invoice_item");

            migrationBuilder.DropTable(
                name: "invoice");

            migrationBuilder.RenameColumn(
                name: "invoice",
                table: "project_incoming_payment",
                newName: "orderslip_number");

            migrationBuilder.AddColumn<DateTime>(
                name: "invoice_date",
                table: "project_incoming_payment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "invoice_number",
                table: "project_incoming_payment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "orderslip_date",
                table: "project_incoming_payment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "group_id",
                table: "project",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "internal_company_id",
                table: "project",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "internal_company",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vatid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_internal_company", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "project_timeline_entry",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    duedate = table.Column<DateTime>(name: "due_date", type: "datetime2", nullable: false),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    completedate = table.Column<DateTime>(name: "complete_date", type: "datetime2", nullable: true),
                    projectid = table.Column<long>(name: "project_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_timeline_entry", x => x.ID);
                    table.ForeignKey(
                        name: "FK_project_timeline_entry_project_project_id",
                        column: x => x.projectid,
                        principalTable: "project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSubtypes",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSubtypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "project_subtype_entry",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subtypeID = table.Column<long>(type: "bigint", nullable: true),
                    subtypeid = table.Column<long>(name: "subtype_id", type: "bigint", nullable: false),
                    projectid = table.Column<long>(name: "project_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_subtype_entry", x => x.ID);
                    table.ForeignKey(
                        name: "FK_project_subtype_entry_ProjectSubtypes_subtypeID",
                        column: x => x.subtypeID,
                        principalTable: "ProjectSubtypes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_project_subtype_entry_project_project_id",
                        column: x => x.projectid,
                        principalTable: "project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_project_group_id",
                table: "project",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_internal_company_id",
                table: "project",
                column: "internal_company_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_subtype_entry_project_id",
                table: "project_subtype_entry",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_subtype_entry_subtypeID",
                table: "project_subtype_entry",
                column: "subtypeID");

            migrationBuilder.CreateIndex(
                name: "IX_project_timeline_entry_project_id",
                table: "project_timeline_entry",
                column: "project_id");

            migrationBuilder.AddForeignKey(
                name: "FK_project_group_group_id",
                table: "project",
                column: "group_id",
                principalTable: "group",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_project_internal_company_internal_company_id",
                table: "project",
                column: "internal_company_id",
                principalTable: "internal_company",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_project_group_group_id",
                table: "project");

            migrationBuilder.DropForeignKey(
                name: "FK_project_internal_company_internal_company_id",
                table: "project");

            migrationBuilder.DropTable(
                name: "internal_company");

            migrationBuilder.DropTable(
                name: "project_subtype_entry");

            migrationBuilder.DropTable(
                name: "project_timeline_entry");

            migrationBuilder.DropTable(
                name: "ProjectSubtypes");

            migrationBuilder.DropIndex(
                name: "IX_project_group_id",
                table: "project");

            migrationBuilder.DropIndex(
                name: "IX_project_internal_company_id",
                table: "project");

            migrationBuilder.DropColumn(
                name: "invoice_date",
                table: "project_incoming_payment");

            migrationBuilder.DropColumn(
                name: "invoice_number",
                table: "project_incoming_payment");

            migrationBuilder.DropColumn(
                name: "orderslip_date",
                table: "project_incoming_payment");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "project");

            migrationBuilder.DropColumn(
                name: "internal_company_id",
                table: "project");

            migrationBuilder.RenameColumn(
                name: "orderslip_number",
                table: "project_incoming_payment",
                newName: "invoice");

            migrationBuilder.CreateTable(
                name: "invoice",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    companyid = table.Column<long>(name: "company_id", type: "bigint", nullable: true),
                    created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    issuedate = table.Column<DateTime>(name: "issue_date", type: "datetime2", nullable: false),
                    number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    totalamount = table.Column<decimal>(name: "total_amount", type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice", x => x.ID);
                    table.ForeignKey(
                        name: "FK_invoice_company_company_id",
                        column: x => x.companyid,
                        principalTable: "company",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "invoice_item",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    incomingpaymentid = table.Column<long>(name: "incoming_payment_id", type: "bigint", nullable: true),
                    invoiceid = table.Column<long>(name: "invoice_id", type: "bigint", nullable: true),
                    amount = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_item", x => x.ID);
                    table.ForeignKey(
                        name: "FK_invoice_item_invoice_invoice_id",
                        column: x => x.invoiceid,
                        principalTable: "invoice",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_invoice_item_project_incoming_payment_incoming_payment_id",
                        column: x => x.incomingpaymentid,
                        principalTable: "project_incoming_payment",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_invoice_company_id",
                table: "invoice",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_item_incoming_payment_id",
                table: "invoice_item",
                column: "incoming_payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_item_invoice_id",
                table: "invoice_item",
                column: "invoice_id");
        }
    }
}
