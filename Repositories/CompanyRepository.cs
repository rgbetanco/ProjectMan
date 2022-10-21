using repairman.Data;
using repairman.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace repairman.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DBContext _context;

        public CompanyRepository(DBContext context)
        {
            _context = context;
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

        public IQueryable<CompanyModel> GetCompanies()
        {
            return _context.Companies;
        }

        public IQueryable<CompanyModel> FindCompanies(string keyword = null)
        {
            var result = _context.Companies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.remarks.Contains(keyword));
            }

            return result;
        }

        public async Task<CompanyModel> CreateCompany(CompanyModel u)
        {
            await _context.Companies.AddAsync(u);

            return u;
        }

        public bool DelCompany(CompanyModel s)
        {
            _context.Companies.Remove(s);
            return true;
        }

        public async Task<CompanyModel> GetCompany(long ID, params string[] includeFields)
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
            var q = (from pc in _context.PersonaCompanies join c in _context.Companies on pc.company_id equals c.ID where pc.persona_id == ID select new CompanyInPersonasViewModel { company_name = c.name, job_title = pc.job_title, company_id = c.ID }).AsNoTracking().ToList();
            return q;
        }
    }
}
