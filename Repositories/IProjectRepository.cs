using Microsoft.AspNetCore.Mvc.Rendering;
using repairman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace repairman.Repositories
{
    public interface IProjectRepository
    {
        IEnumerable<ImportanceEnum> GetImportances();
        IEnumerable<ServiceTypeEnum> GetServiceType();
        IQueryable<ProductBrandModel> GetBrands(int category_id = -1);
        IQueryable<ProductModelModel> GetModels(int category_id = -1);
        IEnumerable<SelectListItem> GetCategoryBrandList(ProductCategoryEnum category_id);
        IEnumerable<SelectListItem> GetCategoryModelList(ProductCategoryEnum category_id);
        IEnumerable<ProductCategoryEnum> GetCategoryList();
        IQueryable<CompanyModel> GetCompanies();
        IQueryable<ProjectModel> FindProjects(ProjectStatusEnum status, ServiceTypeEnum service_type, string keyword = null);
        IQueryable<ProjectModel> FindAllProject();
        IQueryable<ProjectModel> GetAllProject();
        CreditModel GetCredits(long ID);
        Task<ProjectModel> CreateProject(ProjectModel u);
        Task<ProjectModel> GetProject(long ID, params string[] includeFields);
        IQueryable<CreditModel> GetCredits();
        bool DelCompany(CompanyModel s);
        IQueryable<IncomingPaymentModel> GetNextDueIncomingPayment();
        bool DelProject(ProjectModel s);
        Task<ProjectModel> DupProject(ProjectModel u);
        IQueryable<ProjectModel> FindProjects(string keyword = null);
        IQueryable<IncomingPaymentModel> FindIncomingPaymentByCompanyId(long company_id);
    }
}