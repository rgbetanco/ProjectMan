using Microsoft.EntityFrameworkCore;
using repairman.Models;
using Microsoft.AspNetCore.Http;
using CSHelper.Extensions;

namespace repairman.Data
{
    public class DBContext : DbContext
    {

        IHttpContextAccessor _httpContext;

        public DBContext(DbContextOptions<DBContext> options,
            IHttpContextAccessor httpContextAccessor
            ) : base(options)
        {
            this._httpContext = httpContextAccessor;
        }

        // Admin-related
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<PermGroup> PermGroups { get; set; }

        // User-related
        public DbSet<Member> Members { get; set; }

        // department related
        public DbSet<Dept> Depts { get; set; }

        // service request related
        public DbSet<ServiceRequestCat> ServiceRequestCats { get; set; }
        public DbSet<ServiceRequestSubCat> ServiceRequestSubCats { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<ServiceRequestPic> ServiceRequestPics { get; set; }
        public DbSet<ServiceRequestFile> ServiceRequestFiles { get; set; }
        public DbSet<ServiceRequestReply> ServiceRequestReplies { get; set; }
        public DbSet<ServiceRequestReplyFile> ServiceRequestReplyFiles { get; set; }
        public DbSet<ServiceRequestReplyPic> ServiceRequestReplyPics { get; set; }

        // Project related
        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<ProductBrandModel> ProductBrands { get; set; }
        public DbSet<ProductModelModel> ProductModels { get; set; }
        //COMPANIES
        public DbSet<CompanyModel> Companies { get; set; }
        public DbSet<CreditModel> Credits { get; set; }
        public DbSet<CompanyPhoneModel> CompanyPhones { get; set; }
        public DbSet<CompanyAddressModel> CompanyAddresses { get; set; }
        public DbSet<CompanyEmailModel> CompanyEmails { get; set; }
        public DbSet<PersonaPhoneModel> PersonaPhones { get; set; }
        public DbSet<PersonaAddressModel> PersonaAddresses { get; set; }
        public DbSet<PersonaEmailModel> PersonaEmails { get; set; }
        public DbSet<PersonaModel> Personas { get; set; }
        public DbSet<PersonaCompanyModel> PersonaCompanies { get; set; }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<IncomingPaymentModel> IncomingPayments { get; set; }
        public DbSet<OutgoingPaymentModel> OutgoingPayments { get; set; }
        public DbSet<InvoiceModel> Invoices { get; set; }
        public DbSet<InvoiceItemModel> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Invoice
            builder.Entity<InvoiceModel>()
                .HasMany(e => e.invoice_item)
                .WithOne(e => e.invoice)
                .OnDelete(DeleteBehavior.Cascade);
            // Project product list
            builder.Entity<ProjectModel>()
                .HasMany(e => e.product_list)
                .WithOne(e => e.project)
                .OnDelete(DeleteBehavior.Cascade);
            // Project incoming payment
            builder.Entity<ProjectModel>()
                .HasMany(e => e.incoming_payment)
                .WithOne(e => e.project)
                .OnDelete(DeleteBehavior.Cascade);
            // Project outcoming payment
            builder.Entity<ProjectModel>()
                .HasMany(e => e.outgoing_payment)
                .WithOne(e => e.project)
                .OnDelete(DeleteBehavior.Cascade);
            // Company phones
            builder.Entity<CompanyModel>()
                .HasMany(e => e.phone)
                .WithOne(e => e.company)
                .OnDelete(DeleteBehavior.Cascade);
            // Company adress
            builder.Entity<CompanyModel>()
                .HasMany(e => e.address)
                .WithOne(e => e.company)
                .OnDelete(DeleteBehavior.Cascade);
            // Company email
            builder.Entity<CompanyModel>()
                .HasMany(e => e.email)
                .WithOne(e => e.company)
                .OnDelete(DeleteBehavior.Cascade);
            // Persona phones
            builder.Entity<PersonaModel>()
                .HasMany(e => e.phone)
                .WithOne(e => e.persona)
                .OnDelete(DeleteBehavior.Cascade);
            // Persona adress
            builder.Entity<PersonaModel>()
                .HasMany(e => e.address)
                .WithOne(e => e.persona)
                .OnDelete(DeleteBehavior.Cascade);
            // Persona email
            builder.Entity<PersonaModel>()
                .HasMany(e => e.email)
                .WithOne(e => e.persona)
                .OnDelete(DeleteBehavior.Cascade);
            // Company members
            builder.Entity<PersonaModel>()
                .HasMany(e => e.personas_company)
                .WithOne(e => e.persona)
                .HasForeignKey(e => e.persona_id)
                .OnDelete(DeleteBehavior.Cascade);
            

            // apply utc kind to all date/time that has date+time value
            builder.UseUTCForDateTime();

            builder.Entity<User>()
                .HasIndex(u => u.username)
                .IsUnique();

            // prevent delete of sub-cat, if it's in use
            builder.Entity<ServiceRequest>()
                .HasOne(e => e.sub_cat)
                .WithMany(e => e.requests)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ServiceRequest>()
                .HasOne(e => e.dept)
                .WithMany(e => e.requests)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}