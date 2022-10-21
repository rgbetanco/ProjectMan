using repairman.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace repairman.Repositories
{
    public interface IPersonaRepository
    {
        IQueryable<PersonaModel> FindPersonas(string keyword = null, string phone = null);
        Task<PersonaModel> CreatePersona(PersonaModel u);
        Task<PersonaModel> GetPersona(long ID, params string[] includeFields);
        Task<PersonaCompanyModel> CreateCompanyPersona(PersonaCompanyModel u);
        IQueryable<PersonaInCompanyViewModel> GetPersonaPerCompany(long comp_id);
        string GetCompanyName(long company_id);
        bool DelPersona(PersonaModel s);
    }
}
