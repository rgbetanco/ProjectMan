using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectman.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contact",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "credit",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credit", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Depts",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "group",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "persona",
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
                    table.PrimaryKey("PK_persona", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "product_brand",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category = table.Column<int>(type: "int", nullable: false),
                    brand_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_brand", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "product_model",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category = table.Column<int>(type: "int", nullable: false),
                    model_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_model", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequestCats",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestCats", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    perm = table.Column<int>(type: "int", nullable: false),
                    win_user_sid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bad_password_count = table.Column<int>(type: "int", nullable: false),
                    isSalePerson = table.Column<bool>(type: "bit", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    enabled = table.Column<bool>(type: "bit", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    pass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "company",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    credit_id = table.Column<long>(type: "bigint", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nationalID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    website = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company", x => x.ID);
                    table.ForeignKey(
                        name: "FK_company_credit_credit_id",
                        column: x => x.credit_id,
                        principalTable: "credit",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "member",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bad_password_count = table.Column<int>(type: "int", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dept_id = table.Column<long>(type: "bigint", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    enabled = table.Column<bool>(type: "bit", nullable: false),
                    date_created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member", x => x.ID);
                    table.ForeignKey(
                        name: "FK_member_Depts_dept_id",
                        column: x => x.dept_id,
                        principalTable: "Depts",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "perm_group",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<int>(type: "int", nullable: false),
                    group_id = table.Column<long>(type: "bigint", nullable: true),
                    win_group_sid = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perm_group", x => x.ID);
                    table.ForeignKey(
                        name: "FK_perm_group_group_group_id",
                        column: x => x.group_id,
                        principalTable: "group",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "persona_address",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    persona_id = table.Column<long>(type: "bigint", nullable: false),
                    addr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persona_address", x => x.ID);
                    table.ForeignKey(
                        name: "FK_persona_address_persona_persona_id",
                        column: x => x.persona_id,
                        principalTable: "persona",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "persona_email",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    persona_id = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persona_email", x => x.ID);
                    table.ForeignKey(
                        name: "FK_persona_email_persona_persona_id",
                        column: x => x.persona_id,
                        principalTable: "persona",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "persona_phone",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    persona_id = table.Column<long>(type: "bigint", nullable: false),
                    number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    default_number = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persona_phone", x => x.ID);
                    table.ForeignKey(
                        name: "FK_persona_phone_persona_persona_id",
                        column: x => x.persona_id,
                        principalTable: "persona",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequestSubCats",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cat_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestSubCats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceRequestSubCats_ServiceRequestCats_cat_id",
                        column: x => x.cat_id,
                        principalTable: "ServiceRequestCats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_group",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    group_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_group", x => x.ID);
                    table.ForeignKey(
                        name: "FK_user_group_group_group_id",
                        column: x => x.group_id,
                        principalTable: "group",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_group_user_user_id",
                        column: x => x.user_id,
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
                    company_id = table.Column<long>(type: "bigint", nullable: false),
                    addr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_address", x => x.ID);
                    table.ForeignKey(
                        name: "FK_company_address_company_company_id",
                        column: x => x.company_id,
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
                    company_id = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_email", x => x.ID);
                    table.ForeignKey(
                        name: "FK_company_email_company_company_id",
                        column: x => x.company_id,
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
                    company_id = table.Column<long>(type: "bigint", nullable: false),
                    number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_phone", x => x.ID);
                    table.ForeignKey(
                        name: "FK_company_phone_company_company_id",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "persona_company",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    company_id = table.Column<long>(type: "bigint", nullable: false),
                    persona_id = table.Column<long>(type: "bigint", nullable: false),
                    job_title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persona_company", x => x.ID);
                    table.ForeignKey(
                        name: "FK_persona_company_company_company_id",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_persona_company_persona_persona_id",
                        column: x => x.persona_id,
                        principalTable: "persona",
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
                    service_type = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    starting_datetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ending_datetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    importance_id = table.Column<int>(type: "int", nullable: false),
                    company_id = table.Column<long>(type: "bigint", nullable: true),
                    persona_id = table.Column<long>(type: "bigint", nullable: true),
                    contact_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contact_phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    connected_project_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project", x => x.ID);
                    table.ForeignKey(
                        name: "FK_project_company_company_id",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_project_persona_persona_id",
                        column: x => x.persona_id,
                        principalTable: "persona",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_project_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sub_cat_id = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dept_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modify_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    member_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Depts_dept_id",
                        column: x => x.dept_id,
                        principalTable: "Depts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_member_member_id",
                        column: x => x.member_id,
                        principalTable: "member",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ServiceRequests_ServiceRequestSubCats_sub_cat_id",
                        column: x => x.sub_cat_id,
                        principalTable: "ServiceRequestSubCats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "incoming_payment",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    project_id = table.Column<long>(type: "bigint", nullable: true),
                    issueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    item = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amount = table.Column<double>(type: "float", nullable: false),
                    invoice = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_incoming_payment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_incoming_payment_project_project_id",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "outgoing_payment",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    project_id = table.Column<long>(type: "bigint", nullable: true),
                    issueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    company_id = table.Column<long>(type: "bigint", nullable: true),
                    amount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outgoing_payment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_outgoing_payment_company_company_id",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_outgoing_payment_project_project_id",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_list",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category = table.Column<int>(type: "int", nullable: false),
                    serial_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    project_id = table.Column<long>(type: "bigint", nullable: true),
                    product_brand_id = table.Column<long>(type: "bigint", nullable: true),
                    product_model_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_list", x => x.ID);
                    table.ForeignKey(
                        name: "FK_product_list_product_brand_product_brand_id",
                        column: x => x.product_brand_id,
                        principalTable: "product_brand",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_product_list_product_model_product_model_id",
                        column: x => x.product_model_id,
                        principalTable: "product_model",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_product_list_project_project_id",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequestFiles",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    request_id = table.Column<long>(type: "bigint", nullable: false),
                    source_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    filename = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestFiles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceRequestFiles_ServiceRequests_request_id",
                        column: x => x.request_id,
                        principalTable: "ServiceRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequestPics",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    request_id = table.Column<long>(type: "bigint", nullable: false),
                    source_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    thumb_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    output_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestPics", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceRequestPics_ServiceRequests_request_id",
                        column: x => x.request_id,
                        principalTable: "ServiceRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequestReplies",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modify_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    request_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestReplies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceRequestReplies_ServiceRequests_request_id",
                        column: x => x.request_id,
                        principalTable: "ServiceRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRequestReplies_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequestReplyFiles",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reply_id = table.Column<long>(type: "bigint", nullable: false),
                    source_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    filename = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestReplyFiles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceRequestReplyFiles_ServiceRequestReplies_reply_id",
                        column: x => x.reply_id,
                        principalTable: "ServiceRequestReplies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequestReplyPics",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reply_id = table.Column<long>(type: "bigint", nullable: false),
                    source_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    thumb_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    output_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestReplyPics", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ServiceRequestReplyPics_ServiceRequestReplies_reply_id",
                        column: x => x.reply_id,
                        principalTable: "ServiceRequestReplies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_company_credit_id",
                table: "company",
                column: "credit_id");

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
                name: "IX_incoming_payment_project_id",
                table: "incoming_payment",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_member_dept_id",
                table: "member",
                column: "dept_id");

            migrationBuilder.CreateIndex(
                name: "IX_outgoing_payment_company_id",
                table: "outgoing_payment",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_outgoing_payment_project_id",
                table: "outgoing_payment",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_perm_group_group_id",
                table: "perm_group",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_persona_address_persona_id",
                table: "persona_address",
                column: "persona_id");

            migrationBuilder.CreateIndex(
                name: "IX_persona_company_company_id",
                table: "persona_company",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_persona_company_persona_id",
                table: "persona_company",
                column: "persona_id");

            migrationBuilder.CreateIndex(
                name: "IX_persona_email_persona_id",
                table: "persona_email",
                column: "persona_id");

            migrationBuilder.CreateIndex(
                name: "IX_persona_phone_persona_id",
                table: "persona_phone",
                column: "persona_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_list_product_brand_id",
                table: "product_list",
                column: "product_brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_list_product_model_id",
                table: "product_list",
                column: "product_model_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_list_project_id",
                table: "product_list",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_company_id",
                table: "project",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_persona_id",
                table: "project",
                column: "persona_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_user_id",
                table: "project",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestFiles_request_id",
                table: "ServiceRequestFiles",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestPics_request_id",
                table: "ServiceRequestPics",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestReplies_request_id",
                table: "ServiceRequestReplies",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestReplies_user_id",
                table: "ServiceRequestReplies",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestReplyFiles_reply_id",
                table: "ServiceRequestReplyFiles",
                column: "reply_id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestReplyPics_reply_id",
                table: "ServiceRequestReplyPics",
                column: "reply_id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_dept_id",
                table: "ServiceRequests",
                column: "dept_id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_member_id",
                table: "ServiceRequests",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_sub_cat_id",
                table: "ServiceRequests",
                column: "sub_cat_id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestSubCats_cat_id",
                table: "ServiceRequestSubCats",
                column: "cat_id");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "company_address");

            migrationBuilder.DropTable(
                name: "company_email");

            migrationBuilder.DropTable(
                name: "company_phone");

            migrationBuilder.DropTable(
                name: "contact");

            migrationBuilder.DropTable(
                name: "incoming_payment");

            migrationBuilder.DropTable(
                name: "outgoing_payment");

            migrationBuilder.DropTable(
                name: "perm_group");

            migrationBuilder.DropTable(
                name: "persona_address");

            migrationBuilder.DropTable(
                name: "persona_company");

            migrationBuilder.DropTable(
                name: "persona_email");

            migrationBuilder.DropTable(
                name: "persona_phone");

            migrationBuilder.DropTable(
                name: "product_list");

            migrationBuilder.DropTable(
                name: "ServiceRequestFiles");

            migrationBuilder.DropTable(
                name: "ServiceRequestPics");

            migrationBuilder.DropTable(
                name: "ServiceRequestReplyFiles");

            migrationBuilder.DropTable(
                name: "ServiceRequestReplyPics");

            migrationBuilder.DropTable(
                name: "user_group");

            migrationBuilder.DropTable(
                name: "product_brand");

            migrationBuilder.DropTable(
                name: "product_model");

            migrationBuilder.DropTable(
                name: "project");

            migrationBuilder.DropTable(
                name: "ServiceRequestReplies");

            migrationBuilder.DropTable(
                name: "group");

            migrationBuilder.DropTable(
                name: "company");

            migrationBuilder.DropTable(
                name: "persona");

            migrationBuilder.DropTable(
                name: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "credit");

            migrationBuilder.DropTable(
                name: "member");

            migrationBuilder.DropTable(
                name: "ServiceRequestSubCats");

            migrationBuilder.DropTable(
                name: "Depts");

            migrationBuilder.DropTable(
                name: "ServiceRequestCats");
        }
    }
}
