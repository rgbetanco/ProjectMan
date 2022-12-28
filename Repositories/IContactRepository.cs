using projectman.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace projectman.Repositories
{
    public interface IContactRepository
    {
        IQueryable<Contact> Find(string keyword = null, string phone = null);
        Task<Contact> Create(Contact u);
        Task<Contact> Get(long ID, params string[] includeFields);
        Task<ContactCompany> CreateContactCompany(ContactCompany u);
        IQueryable<CompanyContactsViewModel> GetCompanyContacts(long comp_id);
        IQueryable<CompanyContactsViewModel> GetContacts();
        string GetCompanyName(long company_id);
        bool Del(Contact s);
    }
}
