using repairman.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace repairman.Repositories
{
    public interface ICompanyRepository
    {
        IQueryable<CompanyModel> GetCompanies();
        IList<CompanyInPersonasViewModel> GetPersonsInCompanyByPersonID(long ID);
        IQueryable<CompanyModel> FindCompanies(string keyword = null);
        CreditModel GetCredits(long ID);
        Task<CompanyModel> CreateCompany(CompanyModel u);
        Task<CompanyModel> GetCompany(long ID, params string[] includeFields);
        IQueryable<CreditModel> GetCredits();
        public bool DelCompany(CompanyModel s);
    }
}