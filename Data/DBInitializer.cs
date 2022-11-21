using Microsoft.Extensions.Configuration;
using projectman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectman.Data
{
    public class DBInitializer
    {

        public static void Initialize(DBContext context, IConfiguration config)
        {
            context.Database.EnsureCreated();

            // Look for any users
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            string defaultUser = config.GetValue<string>("DefaultAccount:User");
            string defaultPass = config.GetValue<string>("DefaultAccount:Password");

            var users = new User[]
            {
                new User{ 
                    name="Super Admin", 
                    perm=UserPermission.All, 
                    username=defaultUser, 
                    UnencryptedPassword=defaultPass, 
                    enabled=true,
                    date_created = DateTime.UtcNow
                },
            };

            foreach (User u in users)
            {
                context.Users.Add(u);
            }

            context.SaveChanges();
        }


        public static void InitializeTestData(DBContext context, IConfiguration config)
        {
            //Add credit if none
            if (!context.CreditRatings.Any())
            {
                var credits = new CreditRating[]
                {
                    new CreditRating{
                        name="性用 A",
                        code="A"
                    },
                    new CreditRating{
                        name="性用 B",
                        code="B"
                    },
                    new CreditRating{
                        name="性用 C",
                        code="C"
                    },
                };
                foreach (CreditRating c in credits)
                {
                    context.CreditRatings.Add(c);
                }
                context.SaveChanges();
            }

            // Add models if none
            if (!context.Products.Any())
            {
                var brands = new ProductBrand[] {
                    new ProductBrand{
                        name="HP"
                    },
                    new ProductBrand{
                        name="Google"
                    },
                    new ProductBrand{
                        name="Microsoft"
                    }
                };

                foreach (ProductBrand b in brands)
                {
                    context.ProductBrands.Add(b);
                }

                var models = new Product[]
                {
                    new Product{
                        category = ProductCategory.Software,
                        brand = brands[2],
                        name="MS Office 2010",
                        model_name="OFFICE10"
                    },
                    new Product{
                        category = ProductCategory.Hardware,
                        name="Laserjet 5",
                        brand = brands[0],
                        model_name="LJ200X"
                    },
                    new Product{
                        category = ProductCategory.Service,
                        brand = brands[1],
                        name="Google Cloud Platform (1 yr)",
                        model_name="GCP1YR"
                    },
                    new Product{
                        category = ProductCategory.Accessory,
                        brand = brands[0],
                        name = "Mighty Mouse",
                        model_name="MOUSE1"
                    }
                };

                foreach (var m in models)
                {
                    context.Products.Add(m);
                }
                context.SaveChanges();
            }

            // Add Companies if none
            if (!context.Companies.Any())
            {
                var companies = new Company[]
                {
                new Company{
                    name = "CompA",
                    credit_rating_code = "A",
                    vatid = "123",
                    remarks = "Remarks for company A"
                },
                new Company{
                    name = "CompB",
                    credit_rating_code = "B",
                    vatid = "456",
                    remarks = "Remarks for company B"
                }

                };
                foreach (Company c in companies)
                {
                    context.Companies.Add(c);
                }
                context.SaveChanges();

                var persons = new Contact[] {
                    new Contact
                    {
                        name = "Person A",
                        department = "Marketing",
                        remarks = "Remarks for person A"
                    },
                    new Contact
                    {
                        name = "Person B",
                        department = "IT",
                        remarks = "Remarks for person B"
                    }

                };

                foreach (Contact p in persons)
                {
                    context.Contacts.Add(p);
                }
                context.SaveChanges();
            }
        }

    }
}
