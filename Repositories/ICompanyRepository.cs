using projectman.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace projectman.Repositories
{
    public interface ICompanyRepository
    {
        IQueryable<Company> GetCompanies();
        IList<CompanyInPersonasViewModel> GetPersonsInCompanyByPersonID(long ID);
        IQueryable<Company> FindCompanies(string keyword = null);
        CreditRating GetCreditRating(string ID);
        Task<Company> CreateCompany(Company u);
        Task<Company> GetCompany(long ID, params string[] includeFields);
        IQueryable<CreditRating> GetCreditRatings();
        public bool DelCompany(Company s);
        IQueryable<CreditRating> GetCreditRating();
        Task<CreditRating> Create(CreditRating t);
        void DelCreditRatingUnsafe(string t);
        Task<CreditRating> GetCreditRatingAsync(string t);
    }
}