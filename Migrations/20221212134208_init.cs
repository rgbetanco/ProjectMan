using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectman.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contact",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "credit_rating",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credit_rating", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "group",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    datecreated = table.Column<DateTime>(name: "date_created", type: "datetime2", nullable: false),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group", x => x.ID);
                });

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
                name: "product_brand",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_brand", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "project_importance",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_importance", x => x.code);
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
                name: "user",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    perm = table.Column<int>(type: "int", nullable: false),
                    winusersid = table.Column<string>(name: "win_user_sid", type: "nvarchar(max)", nullable: true),
                    badpasswordcount = table.Column<int>(name: "bad_password_count", type: "int", nullable: false),
                    isSalePerson = table.Column<bool>(type: "bit", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    enabled = table.Column<bool>(type: "bit", nullable: false),
                    datecreated = table.Column<DateTime>(name: "date_created", type: "datetime2", nullable: false),
                    username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    pass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "contact_address",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    addr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    personaid = table.Column<long>(name: "persona_id", type: "bigint", nullable: true),
                    contactid = table.Column<long>(name: "contact_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_address", x => x.ID);
                    table.ForeignKey(
                        name: "FK_contact_address_contact_persona_id",
                        column: x => x.personaid,
                        principalTable: "contact",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact_email",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    personaid = table.Column<long>(name: "persona_id", type: "bigint", nullable: true),
                    contactid = table.Column<long>(name: "contact_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_email", x => x.ID);
                    table.ForeignKey(
                        name: "FK_contact_email_contact_persona_id",
                        column: x => x.personaid,
                        principalTable: "contact",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact_phone",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    isdefault = table.Column<bool>(name: "is_default", type: "bit", nullable: false),
                    personaid = table.Column<long>(name: "persona_id", type: "bigint", nullable: true),
                    contactid = table.Column<long>(name: "contact_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_phone", x => x.ID);
                    table.ForeignKey(
                        name: "FK_contact_phone_contact_persona_id",
                        column: x => x.personaid,
                        principalTable: "contact",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "company",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creditratingcode = table.Column<string>(name: "credit_rating_code", type: "nvarchar(450)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vatid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    website = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company", x => x.ID);
                    table.ForeignKey(
                        name: "FK_company_credit_rating_credit_rating_code",
                        column: x => x.creditratingcode,
                        principalTable: "credit_rating",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "perm_group",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<int>(type: "int", nullable: false),
                    groupid = table.Column<long>(name: "group_id", type: "bigint", nullable: true),
                    wingroupsid = table.Column<string>(name: "win_group_sid", type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perm_group", x => x.ID);
                    table.ForeignKey(
                        name: "FK_perm_group_group_group_id",
                        column: x => x.groupid,
                        principalTable: "group",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category = table.Column<int>(type: "int", nullable: false),
                    brandid = table.Column<long>(name: "brand_id", type: "bigint", nullable: false),
                    modelname = table.Column<string>(name: "model_name", type: "nvarchar(max)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => x.ID);
                    table.ForeignKey(
                        name: "FK_product_product_brand_brand_id",
                        column: x => x.brandid,
                        principalTable: "product_brand",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_group",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userid = table.Column<long>(name: "user_id", type: "bigint", nullable: false),
                    groupid = table.Column<long>(name: "group_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_group", x => x.ID);
                    table.ForeignKey(
                        name: "FK_user_group_group_group_id",
                        column: x => x.groupid,
                        principalTable: "group",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_group_user_user_id",
                        column: x => x.userid,
                        principalTable: "user",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "company_address",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    addr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    companyid = table.Column<long>(name: "company_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_address", x => x.ID);
                    table.ForeignKey(
                        name: "FK_company_address_company_company_id",
                        column: x => x.companyid,
                        principalTable: "company",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "company_email",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    companyid = table.Column<long>(name: "company_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_email", x => x.ID);
                    table.ForeignKey(
                        name: "FK_company_email_company_company_id",
                        column: x => x.companyid,
                        principalTable: "company",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "company_phone",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    companyid = table.Column<long>(name: "company_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_phone", x => x.ID);
                    table.ForeignKey(
                        name: "FK_company_phone_company_company_id",
                        column: x => x.companyid,
                        principalTable: "company",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact_company",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    companyid = table.Column<long>(name: "company_id", type: "bigint", nullable: false),
                    jobtitle = table.Column<string>(name: "job_title", type: "nvarchar(max)", nullable: true),
                    personaid = table.Column<long>(name: "persona_id", type: "bigint", nullable: true),
                    contactid = table.Column<long>(name: "contact_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_company", x => x.ID);
                    table.ForeignKey(
                        name: "FK_contact_company_company_company_id",
                        column: x => x.companyid,
                        principalTable: "company",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contact_company_contact_contact_id",
                        column: x => x.contactid,
                        principalTable: "contact",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    startingdatetime = table.Column<DateTime>(name: "starting_datetime", type: "datetime2", nullable: false),
                    endingdatetime = table.Column<DateTime>(name: "ending_datetime", type: "datetime2", nullable: false),
                    internalcompanyid = table.Column<long>(name: "internal_company_id", type: "bigint", nullable: false),
                    groupid = table.Column<long>(name: "group_id", type: "bigint", nullable: false),
                    userid = table.Column<long>(name: "user_id", type: "bigint", nullable: true),
                    importanceid = table.Column<string>(name: "importance_id", type: "nvarchar(450)", nullable: true),
                    companyid = table.Column<long>(name: "company_id", type: "bigint", nullable: true),
                    contactid = table.Column<long>(name: "contact_id", type: "bigint", nullable: true),
                    contactaddress = table.Column<string>(name: "contact_address", type: "nvarchar(max)", nullable: true),
                    contactphone = table.Column<string>(name: "contact_phone", type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    totalincomeamount = table.Column<decimal>(name: "total_income_amount", type: "money", nullable: false),
                    totalpayamount = table.Column<decimal>(name: "total_pay_amount", type: "money", nullable: false),
                    netincome = table.Column<decimal>(name: "net_income", type: "money", nullable: false),
                    connectedprojectid = table.Column<long>(name: "connected_project_id", type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project", x => x.ID);
                    table.ForeignKey(
                        name: "FK_project_company_company_id",
                        column: x => x.companyid,
                        principalTable: "company",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_project_contact_contact_id",
                        column: x => x.contactid,
                        principalTable: "contact",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_project_group_group_id",
                        column: x => x.groupid,
                        principalTable: "group",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_project_internal_company_internal_company_id",
                        column: x => x.internalcompanyid,
                        principalTable: "internal_company",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_project_project_importance_importance_id",
                        column: x => x.importanceid,
                        principalTable: "project_importance",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "FK_project_user_user_id",
                        column: x => x.userid,
                        principalTable: "user",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "project_incoming_payment",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    duedate = table.Column<DateTime>(name: "due_date", type: "datetime2", nullable: false),
                    item = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amount = table.Column<decimal>(type: "money", nullable: false),
                    orderslipnumber = table.Column<string>(name: "orderslip_number", type: "nvarchar(max)", nullable: true),
                    orderslipdate = table.Column<DateTime>(name: "orderslip_date", type: "datetime2", nullable: true),
                    invoicenumber = table.Column<string>(name: "invoice_number", type: "nvarchar(max)", nullable: true),
                    invoicedate = table.Column<DateTime>(name: "invoice_date", type: "datetime2", nullable: true),
                    projectid = table.Column<long>(name: "project_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_incoming_payment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_project_incoming_payment_project_project_id",
                        column: x => x.projectid,
                        principalTable: "project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_outgoing_payment",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    duedate = table.Column<DateTime>(name: "due_date", type: "datetime2", nullable: false),
                    companyid = table.Column<long>(name: "company_id", type: "bigint", nullable: true),
                    amount = table.Column<decimal>(type: "money", nullable: false),
                    projectid = table.Column<long>(name: "project_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_outgoing_payment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_project_outgoing_payment_company_company_id",
                        column: x => x.companyid,
                        principalTable: "company",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_project_outgoing_payment_project_project_id",
                        column: x => x.projectid,
                        principalTable: "project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_product",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productid = table.Column<long>(name: "product_id", type: "bigint", nullable: false),
                    serialnumber = table.Column<string>(name: "serial_number", type: "nvarchar(max)", nullable: true),
                    projectid = table.Column<long>(name: "project_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_product", x => x.ID);
                    table.ForeignKey(
                        name: "FK_project_product_product_product_id",
                        column: x => x.productid,
                        principalTable: "product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_project_product_project_project_id",
                        column: x => x.projectid,
                        principalTable: "project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_subtype_entry",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subtypeid = table.Column<long>(name: "subtype_id", type: "bigint", nullable: false),
                    projectid = table.Column<long>(name: "project_id", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_subtype_entry", x => x.ID);
                    table.ForeignKey(
                        name: "FK_project_subtype_entry_ProjectSubtypes_subtype_id",
                        column: x => x.subtypeid,
                        principalTable: "ProjectSubtypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_project_subtype_entry_project_project_id",
                        column: x => x.projectid,
                        principalTable: "project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "IX_company_credit_rating_code",
                table: "company",
                column: "credit_rating_code");

            migrationBuilder.CreateIndex(
                name: "IX_company_address_company_id",
                table: "company_address",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_company_email_company_id",
                table: "company_email",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_company_phone_company_id",
                table: "company_phone",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_contact_address_persona_id",
                table: "contact_address",
                column: "persona_id");

            migrationBuilder.CreateIndex(
                name: "IX_contact_company_company_id",
                table: "contact_company",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_contact_company_contact_id",
                table: "contact_company",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "IX_contact_email_persona_id",
                table: "contact_email",
                column: "persona_id");

            migrationBuilder.CreateIndex(
                name: "IX_contact_phone_persona_id",
                table: "contact_phone",
                column: "persona_id");

            migrationBuilder.CreateIndex(
                name: "IX_perm_group_group_id",
                table: "perm_group",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_brand_id",
                table: "product",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_company_id",
                table: "project",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_contact_id",
                table: "project",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_group_id",
                table: "project",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_importance_id",
                table: "project",
                column: "importance_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_internal_company_id",
                table: "project",
                column: "internal_company_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_user_id",
                table: "project",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_incoming_payment_project_id",
                table: "project_incoming_payment",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_outgoing_payment_company_id",
                table: "project_outgoing_payment",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_outgoing_payment_project_id",
                table: "project_outgoing_payment",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_product_product_id",
                table: "project_product",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_product_project_id",
                table: "project_product",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_subtype_entry_project_id",
                table: "project_subtype_entry",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_subtype_entry_subtype_id",
                table: "project_subtype_entry",
                column: "subtype_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_timeline_entry_project_id",
                table: "project_timeline_entry",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_username",
                table: "user",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_group_group_id",
                table: "user_group",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_group_user_id",
                table: "user_group",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "company_address");

            migrationBuilder.DropTable(
                name: "company_email");

            migrationBuilder.DropTable(
                name: "company_phone");

            migrationBuilder.DropTable(
                name: "contact_address");

            migrationBuilder.DropTable(
                name: "contact_company");

            migrationBuilder.DropTable(
                name: "contact_email");

            migrationBuilder.DropTable(
                name: "contact_phone");

            migrationBuilder.DropTable(
                name: "perm_group");

            migrationBuilder.DropTable(
                name: "project_incoming_payment");

            migrationBuilder.DropTable(
                name: "project_outgoing_payment");

            migrationBuilder.DropTable(
                name: "project_product");

            migrationBuilder.DropTable(
                name: "project_subtype_entry");

            migrationBuilder.DropTable(
                name: "project_timeline_entry");

            migrationBuilder.DropTable(
                name: "user_group");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "ProjectSubtypes");

            migrationBuilder.DropTable(
                name: "project");

            migrationBuilder.DropTable(
                name: "product_brand");

            migrationBuilder.DropTable(
                name: "company");

            migrationBuilder.DropTable(
                name: "contact");

            migrationBuilder.DropTable(
                name: "group");

            migrationBuilder.DropTable(
                name: "internal_company");

            migrationBuilder.DropTable(
                name: "project_importance");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "credit_rating");
        }
    }
}
