using Microsoft.EntityFrameworkCore;
using projectman.Models;
using Microsoft.AspNetCore.Http;
using CSHelper.Extensions;

namespace projectman.Data
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

        // Project related
        public DbSet<ProjectImportance> ProjectImportances { get; set; }    
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<Product> Products { get; set; }

        //COMPANIES
        public DbSet<Company> Companies { get; set; }
        public DbSet<CreditRating> CreditRatings { get; set; }
        public DbSet<CompanyPhone> CompanyPhones { get; set; }
        public DbSet<CompanyAddress> CompanyAddresses { get; set; }
        public DbSet<CompanyEmail> CompanyEmails { get; set; }

        // Contacts
        public DbSet<ContactPhone> ContactPhones { get; set; }
        public DbSet<ContactAddress> ContactAddresses { get; set; }
        public DbSet<ContactEmail> ContactEmails { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactCompany> ContactCompanies { get; set; }

        public DbSet<ProjectProduct> ProjectProducts { get; set; }
        public DbSet<ProjectIncomingPayment> IncomingPayments { get; set; }
        public DbSet<ProjectOutgoingPayment> OutgoingPayments { get; set; }

        public DbSet<ProjectSubtypeEntry> ProjectSubtypeEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Invoice
            builder.Entity<Invoice>()
                .HasMany(e => e.items)
                .WithOne(e => e.invoice)
                .OnDelete(DeleteBehavior.Cascade);
            // Project product list
            builder.Entity<Project>()
                .HasMany(e => e.products)
                .WithOne(e => e.project)
                .OnDelete(DeleteBehavior.Cascade);
            // Project incoming payment
            builder.Entity<Project>()
                .HasMany(e => e.incoming_payments)
                .WithOne(e => e.project)
                .OnDelete(DeleteBehavior.Cascade);
            // Project outcoming payment
            builder.Entity<Project>()
                .HasMany(e => e.outgoing_payments)
                .WithOne(e => e.project)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Project>()
                .HasMany(e => e.subtypes)
                .WithOne(e => e.project)
                .OnDelete(DeleteBehavior.Cascade);
            // Company phones
            builder.Entity<Company>()
                .HasMany(e => e.phones)
                .WithOne(e => e.company)
                .OnDelete(DeleteBehavior.Cascade);
            // Company adress
            builder.Entity<Company>()
                .HasMany(e => e.addresses)
                .WithOne(e => e.company)
                .OnDelete(DeleteBehavior.Cascade);
            // Company email
            builder.Entity<Company>()
                .HasMany(e => e.emails)
                .WithOne(e => e.company)
                .OnDelete(DeleteBehavior.Cascade);
            // Persona phones
            builder.Entity<Contact>()
                .HasMany(e => e.phones)
                .WithOne(e => e.contact)
                .OnDelete(DeleteBehavior.Cascade);
            // Persona adress
            builder.Entity<Contact>()
                .HasMany(e => e.addresses)
                .WithOne(e => e.contact)
                .OnDelete(DeleteBehavior.Cascade);
            // Persona email
            builder.Entity<Contact>()
                .HasMany(e => e.emails)
                .WithOne(e => e.contact)
                .OnDelete(DeleteBehavior.Cascade);
            // Company members
            builder.Entity<Contact>()
                .HasMany(e => e.companies)
                .WithOne(e => e.contact)
                .HasForeignKey(e => e.contact_id)
                .OnDelete(DeleteBehavior.Cascade);
            

            // apply utc kind to all date/time that has date+time value
            builder.UseUTCForDateTime();

            builder.Entity<User>()
                .HasIndex(u => u.username)
                .IsUnique();

        }
    }
}