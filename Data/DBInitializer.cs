using Microsoft.Extensions.Configuration;
using repairman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace repairman.Data
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

            //Add credit if none
            if (context.Credits.Any())
            {
                return;   // DB has been seeded
            }

            var credits = new CreditModel[]
            {
                new CreditModel{
                    name="狀態 A"
                },
                new CreditModel{
                    name="狀態 B"
                },
                new CreditModel{
                    name="狀態 C"
                },
            };
            foreach (CreditModel c in credits)
            {
                context.Credits.Add(c);
            }
            context.SaveChanges();

            // Add brands if none
            if (context.ProductBrands.Any())
            {
                return;   // DB has been seeded
            }

            var brands = new ProductBrandModel[]
            {
                new ProductBrandModel{
                    category = ProductCategoryEnum.CategoryA,
                    brand_name="商牌 A"
                },
                new ProductBrandModel{
                    category = ProductCategoryEnum.CategoryA,
                    brand_name="商牌 A-1"
                },
                new ProductBrandModel{
                    category = ProductCategoryEnum.CategoryB,
                    brand_name="商牌 B"
                },
                new ProductBrandModel{
                    category = ProductCategoryEnum.CategoryB,
                    brand_name="商牌 B-1"
                },

            };
            foreach (ProductBrandModel b in brands)
            {
                context.ProductBrands.Add(b);
            }
            context.SaveChanges();

            // Add models if none
            if (context.ProductModels.Any())
            {
                return;   // DB has been seeded
            }

            var models = new ProductModelModel[]
            {
                new ProductModelModel{
                    category = ProductCategoryEnum.CategoryA,
                    model_name="型號 A"
                },
                new ProductModelModel{
                    category = ProductCategoryEnum.CategoryA,
                    model_name="型號 A-1"
                },
                new ProductModelModel{
                    category = ProductCategoryEnum.CategoryB,
                    model_name="型號 B"
                },
                new ProductModelModel{
                    category = ProductCategoryEnum.CategoryB,
                    model_name="型號 B-1"
                },

            };
            foreach (ProductModelModel m in models)
            {
                context.ProductModels.Add(m);
            }
            context.SaveChanges();

            // Add Companies if none
            if (context.Companies.Any())
            {
                return;   // DB has been seeded
            }

            var companies = new CompanyModel[]
            {
                new CompanyModel{
                    name = "CompA",
                    credit_id = 0,
                    nationalID = "123",
                    remarks = "Remarks for company A"
                },
                new CompanyModel{
                    name = "CompB",
                    credit_id = 0,
                    nationalID = "456",
                    remarks = "Remarks for company B"
                }

            };
            foreach (CompanyModel c in companies)
            {
                context.Companies.Add(c);
            }
            context.SaveChanges();
            // Add Persona if none
            if (context.Personas.Any())
            {
                return;   // DB has been seeded
            }

            var persons = new PersonaModel[]
            {
                new PersonaModel
                {
                    name = "Persona A",
                    department = "Marketing",
                    remarks = "Remarks for pesona A"
                },
                new PersonaModel
                {
                    name = "Persona B",
                    department = "IT",
                    remarks = "Remarks for pesona B"
                }

            };
            foreach (PersonaModel p in persons)
            {
                context.Personas.Add(p);
            }
            context.SaveChanges();
        }
    }
}
