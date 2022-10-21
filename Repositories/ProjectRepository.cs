using repairman.Data;
using repairman.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.InteropServices;

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
                return _context.ProductBrands.AsQueryable();
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
                return _context.ProductModels.AsQueryable();
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

        public IQueryable<ProjectModel> FindAllProject()
        {
            return _context.Projects.AsQueryable();
        }

        public async Task<ProjectModel> CreateProject(ProjectModel u)
        {
            await _context.Projects.AddAsync(u);

            return u;
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
    }
}
