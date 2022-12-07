using projectman.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectman.Repositories
{
    public interface IProjectRepository
    {
        Task<Project> CreateProject(Project u);
        bool DelCompany(Company s);
        bool DelProject(Project s);
        Task<Project> DupProject(Project u);
        IQueryable<Project> FindAllProject();
        IQueryable<ProjectIncomingPayment> FindIncomingPaymentByCompanyId(long company_id, string keyword);
        IQueryable<Project> FindProjects(string keyword = null);
        IQueryable<Project> FindProjects(ProjectStatus status, ProjectType service_type, string keyword = null);
        IQueryable<Project> GetAllProject();

        Task<ProjectImportance> Create(ProjectImportance t);
        Task<ProjectImportance> GetImportanceAsync(string t);
        IQueryable<ProjectImportance> GetImportances();
        void DelImportanceUnsafe(string t);

        IQueryable<ProjectSubtype> GetSubtypes(ProjectType? type);

        IQueryable<ProjectIncomingPayment> GetNextDueIncomingPayment();
        Task<Project> GetProject(long ID, params string[] includeFields);
        IEnumerable<ProjectType> GetServiceType();
        IQueryable<ProjectSubtype> GetProjectSubtypes();
        Task<ProjectSubtype> CreateProjectSubtype(ProjectSubtype t);
        void DelProjectSubtypeUnsafe(long ID);
        Task<ProjectSubtype> GetProjectSubtypeAsync(long t);
    }
}