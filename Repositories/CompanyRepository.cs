using projectman.Data;
using projectman.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace projectman.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DBContext _context;

        public CompanyRepository(DBContext context)
        {
            _context = context;
        }

        //GET ALL CREDIT
        public IQueryable<CreditRating> GetCreditRatings()
        {
            return _context.CreditRatings.AsQueryable();
        }
        public CreditRating GetCreditRating(string ID)
        {
            var q = _context.CreditRatings.AsQueryable();
            return q.FirstOrDefault(r => r.code == ID);
        }

        public IQueryable<Company> GetCompanies()
        {
            return _context.Companies;
        }

        public IQueryable<Company> FindCompanies(string keyword = null)
        {
            var result = _context.Companies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.remarks.Contains(keyword));
            }

            return result;
        }

        public async Task<Company> CreateCompany(Company u)
        {
            await _context.Companies.AddAsync(u);

            return u;
        }

        public bool DelCompany(Company s)
        {
            _context.Companies.Remove(s);
            return true;
        }

        public async Task<Company> GetCompany(long ID, params string[] includeFields)
        {
            var q = _context.Companies.AsQueryable();

            if (includeFields != null)
            {
                foreach (var field in includeFields)
                    q = q.Include(field);
            }

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);
            return a;
        }

        public IList<CompanyInPersonasViewModel> GetPersonsInCompanyByPersonID(long ID)
        {
            var q = (from pc in _context.ContactCompanies join c in _context.Companies on pc.company_id equals c.ID where pc.contact_id == ID select new CompanyInPersonasViewModel { company_name = c.name, job_title = pc.job_title, company_id = c.ID }).AsNoTracking().ToList();
            return q;
        }

        //Credit rating list
        public IQueryable<CreditRating> GetCreditRating()
        {
            return _context.CreditRatings.AsQueryable();
        }

        public async Task<CreditRating> Create(CreditRating t)
        {
            await _context.CreditRatings.AddAsync(t);
            return t;
        }

        public void DelCreditRatingUnsafe(string t)
        {
            var s = new CreditRating { code = t };

            _context.CreditRatings.Attach(s);
            _context.CreditRatings.Remove(s);
        }

        public async Task<CreditRating> GetCreditRatingAsync(string t)
        {
            var q = GetCreditRating();
            var s = await q.FirstOrDefaultAsync(u => u.code == t);
            return s;
        }
    }
}
