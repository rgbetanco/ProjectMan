using Microsoft.EntityFrameworkCore;
using projectman.Data;
using projectman.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace projectman.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly DBContext _context;

        public ContactRepository(DBContext context)
        {
            _context = context;
        }

        public IQueryable<Contact> Find(string keyword = null, string phone = null)
        {
            var result = _context.Contacts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.remarks.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                result = result.Where(u => u.phones.Any(a => a.number == phone));
            }

            return result;
        }

        public async Task<Contact> Create(Contact u)
        {
            await _context.Contacts.AddAsync(u);
            return u;
        }

        public async Task<ContactCompany> CreateContactCompany(ContactCompany u)
        {
            await _context.ContactCompanies.AddAsync(u);
            return u;
        }

        public string GetCompanyName(long company_id)
        {
            var a = _context.Companies.AsNoTracking().FirstOrDefault(a => a.ID == company_id);
            return a.name;
        }

        public async Task<Contact> Get(long ID, params string[] includeFields)
        {
            var q = _context.Contacts.AsQueryable();

            if (includeFields != null)
            {
                foreach (var field in includeFields)
                    q = q.Include(field);
            }

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);
            return a;
        }

        public IQueryable<CompanyContactsViewModel> GetCompanyContacts(long comp_id)
        {
            var q = (from pc in _context.ContactCompanies join p in _context.Contacts on pc.contact_id equals p.ID where pc.company_id == comp_id select new CompanyContactsViewModel { ID = p.ID, name = p.name, job_title = pc.job_title, phone = p.phones.Where(m => m.is_default == true).FirstOrDefault().number }).AsNoTracking();
            return q;
        }

        public bool Del(Contact s)
        {
            _context.Contacts.Remove(s);
            return true;
        }
    }
}
