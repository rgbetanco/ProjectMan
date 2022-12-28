﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using projectman.Data;

#nullable disable

namespace projectman.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20221214104804_ProjectInvoice")]
    partial class ProjectInvoice
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("projectman.Models.Company", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("credit_rating_code")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("vatid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("website")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("credit_rating_code");

                    b.ToTable("company");
                });

            modelBuilder.Entity("projectman.Models.CompanyAddress", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("addr")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("company_id")
                        .HasColumnType("bigint");

                    b.Property<short>("type")
                        .HasColumnType("smallint");

                    b.HasKey("ID");

                    b.HasIndex("company_id");

                    b.ToTable("company_address");
                });

            modelBuilder.Entity("projectman.Models.CompanyEmail", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<long>("company_id")
                        .HasColumnType("bigint");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("type")
                        .HasColumnType("smallint");

                    b.HasKey("ID");

                    b.HasIndex("company_id");

                    b.ToTable("company_email");
                });

            modelBuilder.Entity("projectman.Models.CompanyPhone", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<long>("company_id")
                        .HasColumnType("bigint");

                    b.Property<string>("number")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("type")
                        .HasColumnType("smallint");

                    b.HasKey("ID");

                    b.HasIndex("company_id");

                    b.ToTable("company_phone");
                });

            modelBuilder.Entity("projectman.Models.Contact", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("department")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("remarks")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("contact");
                });

            modelBuilder.Entity("projectman.Models.ContactAddress", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("addr")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("contact_id")
                        .HasColumnType("bigint");

                    b.Property<long?>("persona_id")
                        .HasColumnType("bigint");

                    b.Property<short>("type")
                        .HasColumnType("smallint");

                    b.HasKey("ID");

                    b.HasIndex("persona_id");

                    b.ToTable("contact_address");
                });

            modelBuilder.Entity("projectman.Models.ContactCompany", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<long>("company_id")
                        .HasColumnType("bigint");

                    b.Property<long>("contact_id")
                        .HasColumnType("bigint");

                    b.Property<string>("job_title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("persona_id")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("company_id");

                    b.HasIndex("contact_id");

                    b.ToTable("contact_company");
                });

            modelBuilder.Entity("projectman.Models.ContactEmail", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<long>("contact_id")
                        .HasColumnType("bigint");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("persona_id")
                        .HasColumnType("bigint");

                    b.Property<short>("type")
                        .HasColumnType("smallint");

                    b.HasKey("ID");

                    b.HasIndex("persona_id");

                    b.ToTable("contact_email");
                });

            modelBuilder.Entity("projectman.Models.ContactPhone", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<long>("contact_id")
                        .HasColumnType("bigint");

                    b.Property<bool>("is_default")
                        .HasColumnType("bit");

                    b.Property<string>("number")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("persona_id")
                        .HasColumnType("bigint");

                    b.Property<short>("type")
                        .HasColumnType("smallint");

                    b.HasKey("ID");

                    b.HasIndex("persona_id");

                    b.ToTable("contact_phone");
                });

            modelBuilder.Entity("projectman.Models.CreditRating", b =>
                {
                    b.Property<string>("code")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("desc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("code");

                    b.ToTable("credit_rating");
                });

            modelBuilder.Entity("projectman.Models.Group", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<DateTime>("date_created")
                        .HasColumnType("datetime2");

                    b.Property<string>("desc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("group");
                });

            modelBuilder.Entity("projectman.Models.InternalCompany", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("vatid")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("internal_company");
                });

            modelBuilder.Entity("projectman.Models.PermGroup", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<long?>("group_id")
                        .HasColumnType("bigint");

                    b.Property<int>("type")
                        .HasColumnType("int");

                    b.Property<string>("win_group_sid")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("group_id");

                    b.ToTable("perm_group");
                });

            modelBuilder.Entity("projectman.Models.Product", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<long>("brand_id")
                        .HasColumnType("bigint");

                    b.Property<int>("category")
                        .HasColumnType("int");

                    b.Property<string>("code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("desc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("model_name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("brand_id");

                    b.ToTable("product");
                });

            modelBuilder.Entity("projectman.Models.ProductBrand", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("product_brand");
                });

            modelBuilder.Entity("projectman.Models.Project", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<long?>("company_id")
                        .HasColumnType("bigint");

                    b.Property<long?>("connected_project_id")
                        .HasColumnType("bigint");

                    b.Property<string>("contact_address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("contact_id")
                        .HasColumnType("bigint");

                    b.Property<string>("contact_phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ending_datetime")
                        .HasColumnType("datetime2");

                    b.Property<long>("group_id")
                        .HasColumnType("bigint");

                    b.Property<string>("importance_id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("internal_company_id")
                        .HasColumnType("bigint");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("net_income")
                        .HasColumnType("money");

                    b.Property<string>("number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("remarks")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("starting_datetime")
                        .HasColumnType("datetime2");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.Property<decimal>("total_income_amount")
                        .HasColumnType("money");

                    b.Property<decimal>("total_pay_amount")
                        .HasColumnType("money");

                    b.Property<int>("type")
                        .HasColumnType("int");

                    b.Property<long?>("user_id")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("company_id");

                    b.HasIndex("contact_id");

                    b.HasIndex("group_id");

                    b.HasIndex("importance_id");

                    b.HasIndex("internal_company_id");

                    b.HasIndex("user_id");

                    b.ToTable("project");
                });

            modelBuilder.Entity("projectman.Models.ProjectImportance", b =>
                {
                    b.Property<string>("code")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("desc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("code");

                    b.ToTable("project_importance");
                });

            modelBuilder.Entity("projectman.Models.ProjectIncomingPayment", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<decimal>("amount")
                        .HasColumnType("money");

                    b.Property<DateTime>("due_date")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("invoice_date")
                        .HasColumnType("datetime2");

                    b.Property<string>("invoice_number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("item")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("orderslip_date")
                        .HasColumnType("datetime2");

                    b.Property<string>("orderslip_number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("project_id")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("project_id");

                    b.ToTable("project_incoming_payment");
                });

            modelBuilder.Entity("projectman.Models.ProjectOutgoingPayment", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<decimal>("amount")
                        .HasColumnType("money");

                    b.Property<long?>("company_id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("due_date")
                        .HasColumnType("datetime2");

                    b.Property<long>("project_id")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("company_id");

                    b.HasIndex("project_id");

                    b.ToTable("project_outgoing_payment");
                });

            modelBuilder.Entity("projectman.Models.ProjectProduct", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<long>("product_id")
                        .HasColumnType("bigint");

                    b.Property<long>("project_id")
                        .HasColumnType("bigint");

                    b.Property<string>("serial_number")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("product_id");

                    b.HasIndex("project_id");

                    b.ToTable("project_product");
                });

            modelBuilder.Entity("projectman.Models.ProjectSubtype", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("type")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("ProjectSubtypes");
                });

            modelBuilder.Entity("projectman.Models.ProjectSubtypeEntry", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<long>("project_id")
                        .HasColumnType("bigint");

                    b.Property<long>("subtype_id")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("project_id");

                    b.HasIndex("subtype_id");

                    b.ToTable("project_subtype_entry");
                });

            modelBuilder.Entity("projectman.Models.ProjectTimelineEntry", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<DateTime?>("complete_date")
                        .HasColumnType("datetime2");

                    b.Property<string>("desc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("due_date")
                        .HasColumnType("datetime2");

                    b.Property<long>("project_id")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("project_id");

                    b.ToTable("project_timeline_entry");
                });

            modelBuilder.Entity("projectman.Models.User", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<int>("bad_password_count")
                        .HasColumnType("int");

                    b.Property<DateTime>("date_created")
                        .HasColumnType("datetime2");

                    b.Property<string>("desc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("enabled")
                        .HasColumnType("bit");

                    b.Property<bool>("isSalePerson")
                        .HasColumnType("bit");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pass")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("perm")
                        .HasColumnType("int");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("win_user_sid")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("username")
                        .IsUnique();

                    b.ToTable("user");
                });

            modelBuilder.Entity("projectman.Models.UserGroup", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ID"));

                    b.Property<long>("group_id")
                        .HasColumnType("bigint");

                    b.Property<long>("user_id")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("group_id");

                    b.HasIndex("user_id");

                    b.ToTable("user_group");
                });

            modelBuilder.Entity("projectman.Models.Company", b =>
                {
                    b.HasOne("projectman.Models.CreditRating", "credit_rating")
                        .WithMany()
                        .HasForeignKey("credit_rating_code");

                    b.Navigation("credit_rating");
                });

            modelBuilder.Entity("projectman.Models.CompanyAddress", b =>
                {
                    b.HasOne("projectman.Models.Company", "company")
                        .WithMany("addresses")
                        .HasForeignKey("company_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("company");
                });

            modelBuilder.Entity("projectman.Models.CompanyEmail", b =>
                {
                    b.HasOne("projectman.Models.Company", "company")
                        .WithMany("emails")
                        .HasForeignKey("company_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("company");
                });

            modelBuilder.Entity("projectman.Models.CompanyPhone", b =>
                {
                    b.HasOne("projectman.Models.Company", "company")
                        .WithMany("phones")
                        .HasForeignKey("company_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("company");
                });

            modelBuilder.Entity("projectman.Models.ContactAddress", b =>
                {
                    b.HasOne("projectman.Models.Contact", "contact")
                        .WithMany("addresses")
                        .HasForeignKey("persona_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("contact");
                });

            modelBuilder.Entity("projectman.Models.ContactCompany", b =>
                {
                    b.HasOne("projectman.Models.Company", "company")
                        .WithMany()
                        .HasForeignKey("company_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("projectman.Models.Contact", "contact")
                        .WithMany("companies")
                        .HasForeignKey("contact_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("company");

                    b.Navigation("contact");
                });

            modelBuilder.Entity("projectman.Models.ContactEmail", b =>
                {
                    b.HasOne("projectman.Models.Contact", "contact")
                        .WithMany("emails")
                        .HasForeignKey("persona_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("contact");
                });

            modelBuilder.Entity("projectman.Models.ContactPhone", b =>
                {
                    b.HasOne("projectman.Models.Contact", "contact")
                        .WithMany("phones")
                        .HasForeignKey("persona_id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("contact");
                });

            modelBuilder.Entity("projectman.Models.PermGroup", b =>
                {
                    b.HasOne("projectman.Models.Group", "group")
                        .WithMany()
                        .HasForeignKey("group_id");

                    b.Navigation("group");
                });

            modelBuilder.Entity("projectman.Models.Product", b =>
                {
                    b.HasOne("projectman.Models.ProductBrand", "brand")
                        .WithMany()
                        .HasForeignKey("brand_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("brand");
                });

            modelBuilder.Entity("projectman.Models.Project", b =>
                {
                    b.HasOne("projectman.Models.Company", "company")
                        .WithMany()
                        .HasForeignKey("company_id");

                    b.HasOne("projectman.Models.Contact", "contact")
                        .WithMany()
                        .HasForeignKey("contact_id");

                    b.HasOne("projectman.Models.Group", "group")
                        .WithMany()
                        .HasForeignKey("group_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("projectman.Models.ProjectImportance", "importance")
                        .WithMany()
                        .HasForeignKey("importance_id");

                    b.HasOne("projectman.Models.InternalCompany", "internal_company")
                        .WithMany()
                        .HasForeignKey("internal_company_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("projectman.Models.User", "user")
                        .WithMany()
                        .HasForeignKey("user_id");

                    b.Navigation("company");

                    b.Navigation("contact");

                    b.Navigation("group");

                    b.Navigation("importance");

                    b.Navigation("internal_company");

                    b.Navigation("user");
                });

            modelBuilder.Entity("projectman.Models.ProjectIncomingPayment", b =>
                {
                    b.HasOne("projectman.Models.Project", "project")
                        .WithMany("incoming_payments")
                        .HasForeignKey("project_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("project");
                });

            modelBuilder.Entity("projectman.Models.ProjectOutgoingPayment", b =>
                {
                    b.HasOne("projectman.Models.Company", "company")
                        .WithMany()
                        .HasForeignKey("company_id");

                    b.HasOne("projectman.Models.Project", "project")
                        .WithMany("outgoing_payments")
                        .HasForeignKey("project_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("company");

                    b.Navigation("project");
                });

            modelBuilder.Entity("projectman.Models.ProjectProduct", b =>
                {
                    b.HasOne("projectman.Models.Product", "product")
                        .WithMany()
                        .HasForeignKey("product_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("projectman.Models.Project", "project")
                        .WithMany("products")
                        .HasForeignKey("project_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("product");

                    b.Navigation("project");
                });

            modelBuilder.Entity("projectman.Models.ProjectSubtypeEntry", b =>
                {
                    b.HasOne("projectman.Models.Project", "project")
                        .WithMany("subtypes")
                        .HasForeignKey("project_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("projectman.Models.ProjectSubtype", "subtype")
                        .WithMany()
                        .HasForeignKey("subtype_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("project");

                    b.Navigation("subtype");
                });

            modelBuilder.Entity("projectman.Models.ProjectTimelineEntry", b =>
                {
                    b.HasOne("projectman.Models.Project", "project")
                        .WithMany("timelines")
                        .HasForeignKey("project_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("project");
                });

            modelBuilder.Entity("projectman.Models.UserGroup", b =>
                {
                    b.HasOne("projectman.Models.Group", "group")
                        .WithMany()
                        .HasForeignKey("group_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("projectman.Models.User", "user")
                        .WithMany("groups")
                        .HasForeignKey("user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("group");

                    b.Navigation("user");
                });

            modelBuilder.Entity("projectman.Models.Company", b =>
                {
                    b.Navigation("addresses");

                    b.Navigation("emails");

                    b.Navigation("phones");
                });

            modelBuilder.Entity("projectman.Models.Contact", b =>
                {
                    b.Navigation("addresses");

                    b.Navigation("companies");

                    b.Navigation("emails");

                    b.Navigation("phones");
                });

            modelBuilder.Entity("projectman.Models.Project", b =>
                {
                    b.Navigation("incoming_payments");

                    b.Navigation("outgoing_payments");

                    b.Navigation("products");

                    b.Navigation("subtypes");

                    b.Navigation("timelines");
                });

            modelBuilder.Entity("projectman.Models.User", b =>
                {
                    b.Navigation("groups");
                });
#pragma warning restore 612, 618
        }
    }
}
