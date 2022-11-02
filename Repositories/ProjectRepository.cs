using repairman.Data;
using repairman.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.InteropServices;
using Microsoft.IdentityModel.Tokens;

namespace repairman.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DBContext _context;

        public ProjectRepository(DBContext context)
        {
            _context = context;
        }
        //GET A LIST OF BRANDS
        public IEnumerable<SelectListItem> GetCategoryBrandList(ProductCategoryEnum category_id)
        {
            return _context.ProductBrands
                .AsNoTracking()
                .Where(m => m.category == category_id)
                .ToList()
                .Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.brand_name });
        }
        //GET A LIST OF MODELS
        public IEnumerable<SelectListItem> GetCategoryModelList(ProductCategoryEnum category_id)
        {
            return _context.ProductModels
                .AsNoTracking()
                .Where(m => m.category == category_id)
                .ToList()
                .Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.model_name });
        }

        //GET BRAND 
        public IQueryable<ProductBrandModel> GetBrands(int category_id = -1)
        {
            if(category_id > -1)
            {
                ProductCategoryEnum cat = (ProductCategoryEnum)category_id;
                return _context.ProductBrands.AsQueryable().Where(m => m.category == cat);
            } else
            {
                return _context.ProductBrands.AsQueryable().Where(m => m.category == ProductCategoryEnum.CategoryA);
            }
        }
        //GET MODELS 
        public IQueryable<ProductModelModel> GetModels(int category_id = -1)
        {
            if (category_id > -1)
            {
                ProductCategoryEnum cat = (ProductCategoryEnum)category_id;
                return _context.ProductModels.AsQueryable().Where(m => m.category == cat);
            }
            else
            {
                return _context.ProductModels.AsQueryable().Where(m => m.category == ProductCategoryEnum.CategoryA);
            }
        }
        //GET ALL CREDIT
        public IQueryable<CreditModel> GetCredits()
        {
            return _context.Credits.AsQueryable();
        }
        public CreditModel GetCredits(long ID)
        {
            var q = _context.Credits.AsQueryable();
            return q.FirstOrDefault(r => r.ID == ID);
        }
        // GET ALL IMPORTANCES
        public IEnumerable<ImportanceEnum> GetImportances()
        {
            return Enum.GetValues(typeof(ImportanceEnum)).Cast<ImportanceEnum>(); ;
        }
        // GET ALL SERVICES
        public IEnumerable<ServiceTypeEnum> GetServiceType()
        {
            return Enum.GetValues(typeof(ServiceTypeEnum)).Cast<ServiceTypeEnum>();
        }
        // GET ALL CATEGORIES
        public IEnumerable<ProductCategoryEnum> GetCategoryList()
        {
            return Enum.GetValues(typeof(ProductCategoryEnum)).Cast<ProductCategoryEnum>();
        }
        public IQueryable<CompanyModel> GetCompanies()
        {
            return _context.Companies;
        }

        public IQueryable<IncomingPaymentModel> GetNextDueIncomingPayment()
        {
            return _context.IncomingPayments.AsQueryable();
            //var r =_context.IncomingPayments.AsQueryable().Where(r => r.project_id == ID).Where(r => String.IsNullOrEmpty(r.invoice)).OrderByDescending(m => m.issueDate).FirstOrDefaultAsync();
            //return r.issueDate.ToShortDateString();
        }

        public IQueryable<ProjectModel> FindProjects(ProjectStatusEnum status, ServiceTypeEnum service_type, string keyword = null)
        {
            var result = _context.Projects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.remarks.Contains(keyword));
            }
            if (status != ProjectStatusEnum.Undefined)
            {
                result = result.Where(u => u.status == status);
            }
            if (service_type != ServiceTypeEnum.Undefined)
            {
                result = result.Where(u => u.service_type == service_type);
            }

            return result;
        }

        public IQueryable<IncomingPaymentModel> FindIncomingPaymentByCompanyId(long company_id)
        {
            var result = _context.IncomingPayments.AsQueryable();

            if (company_id > -1)
            {
                result = result.Where(u => u.project.company_id == company_id);
            }

            return result;
        }

        public IQueryable<ProjectModel> FindAllProject()
        {
            return _context.Projects.AsQueryable();
        }

        public IQueryable<ProjectModel> GetAllProject()
        {
            return _context.Projects.Include(m => m.incoming_payment.Where(m => m.invoice != null).OrderByDescending(m => m.issueDate)).OrderBy(m => m.starting_datetime);
        }

        public async Task<ProjectModel> CreateProject(ProjectModel u)
        {
            await _context.Projects.AddAsync(u);

            return u;
        }

        public async Task<ProjectModel> DupProject(ProjectModel u)
        {
            ProjectModel n = new ProjectModel();
            n.name = u.name;
            n.number = u.number;
            n.remarks = u.remarks;
            n.service_type = u.service_type;
            n.status = u.status;
            n.starting_datetime = u.starting_datetime;
            n.ending_datetime = u.ending_datetime;
            n.user_id  = u.user_id;
            n.importance_id = u.importance_id;
            n.company_id = u.company_id;
            n.persona_id = u.persona_id;
            n.contact_address = u.contact_address;
            n.contact_phone = u.contact_phone;
            n.connected_project_id = u.ID;

            await _context.Projects.AddAsync(n);
            await _context.SaveChangesAsync();

            var proj = await _context.Projects.FindAsync(n.ID);
            if (proj != null)
            {
                IList<ProductModel> product_list = new List<ProductModel>();
                foreach(var i in u.product_list)
                {
                    product_list.Add( new ProductModel 
                    { 
                        project_id = proj.ID,
                        product_brand_id = i.product_brand_id,
                        product_model_id = i.product_model_id,
                        category = i.category,
                        serial_number = i.serial_number
                    });
                }

                IList<IncomingPaymentModel> incoming_payment_list = new List<IncomingPaymentModel>();
                foreach (var i in u.incoming_payment)
                {
                    incoming_payment_list.Add(new IncomingPaymentModel 
                    {
                        project_id = proj.ID,
                        issueDate = i.issueDate,
                        item = i.item,
                        amount = i.amount,
                        invoice = i.invoice
                    });
                }

                IList<OutgoingPaymentModel> outgoing_payment_list = new List<OutgoingPaymentModel>();
                foreach (var i in u.outgoing_payment)
                {
                    outgoing_payment_list.Add(new OutgoingPaymentModel 
                    {
                        project_id = proj.ID,
                        issueDate = i.issueDate,
                        company_id = i.company_id,
                        amount = i.amount
                    });
                }

                proj.product_list = product_list;
                proj.incoming_payment = incoming_payment_list;
                proj.outgoing_payment = outgoing_payment_list;

                await _context.SaveChangesAsync();
            }

            return n;
        }

        public bool DelCompany(CompanyModel s)
        {
            _context.Companies.Remove(s);
            return true;
        }

        public async Task<ProjectModel> GetProject(long ID, params string[] includeFields)
        {
            var q = _context.Projects.AsQueryable();

            if (includeFields != null)
            {
                foreach (var field in includeFields)
                    q = q.Include(field);
            }

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);
            return a;
        }

        public bool DelProject(ProjectModel s)
        {
            _context.Projects.Remove(s);
            return true;
        }

        public IQueryable<ProjectModel> FindProjects(string keyword = null)
        {
            var result = _context.Projects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.remarks.Contains(keyword));
            }

            return result;
        }
    }
}
