using Microsoft.EntityFrameworkCore;
using repairman.Data;
using repairman.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace repairman.Repositories
{
    public class PersonaRepository : IPersonaRepository
    {
        private readonly DBContext _context;

        public PersonaRepository(DBContext context)
        {
            _context = context;
        }

        public IQueryable<PersonaModel> FindPersonas(string keyword = null, string phone = null)
        {
            var result = _context.Personas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.remarks.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                result = result.Where(u => u.phone.Any(a => a.number == phone));
            }

            return result;
        }

        public async Task<PersonaModel> CreatePersona(PersonaModel u)
        {
            await _context.Personas.AddAsync(u);
            return u;
        }

        public async Task<PersonaCompanyModel> CreateCompanyPersona(PersonaCompanyModel u)
        {
            await _context.PersonaCompanies.AddAsync(u);
            return u;
        }

        public string GetCompanyName(long company_id)
        {
            var a = _context.Companies.AsNoTracking().FirstOrDefault(a => a.ID == company_id);
            return a.name;
        }

        public async Task<PersonaModel> GetPersona(long ID, params string[] includeFields)
        {
            var q = _context.Personas.AsQueryable();

            if (includeFields != null)
            {
                foreach (var field in includeFields)
                    q = q.Include(field);
            }

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);
            return a;
        }

        public IQueryable<PersonaInCompanyViewModel> GetPersonaPerCompany(long comp_id)
        {
            var q = (from pc in _context.PersonaCompanies join p in _context.Personas on pc.persona_id equals p.ID where pc.company_id == comp_id select new PersonaInCompanyViewModel { name = p.name, job_title = pc.job_title, phone = p.phone.Where(m => m.default_number == true).FirstOrDefault().number }).AsNoTracking();
            return q;
        }

        public bool DelPersona(PersonaModel s)
        {
            _context.Personas.Remove(s);
            return true;
        }
    }
}
