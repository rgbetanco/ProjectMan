using repairman.Models;
using System.Linq;
using System.Threading.Tasks;

namespace repairman.Repositories
{
    public interface IServiceRequestRepository
    {
        Task<ServiceRequest> Create(ServiceRequest r);
        Task<ServiceRequestCat> Create(ServiceRequestCat cat);
        Task<ServiceRequestReply> Create(ServiceRequestReply m);
        Task<ServiceRequestSubCat> Create(ServiceRequestSubCat subcat);
        bool DelCatUnsafe(long ID);
        bool DelCat(ServiceRequestCat s);
        bool DelDeptUnsafe(long ID);
        bool DelServiceRequest(long ID);
        bool DelSubCatUnsafe(long ID);
        Task<ServiceRequestCat> GetCat(long ID, params string[] includeFields);
        IQueryable<ServiceRequestCat> FindCats(string keyword = null);
        IQueryable<Dept> GetDepts();
        Task<ServiceRequestFile> GetFile(long ID);
        Task<ServiceRequestPic> GetPic(long ID);
        Task<ServiceRequestReply> GetReply(long ID, params string[] includeFields);
        Task<ServiceRequestReplyFile> GetReplyFile(long ID);
        Task<ServiceRequestReplyPic> GetReplyPic(long ID);
        Task<ServiceRequest> Get(long ID, params string[] includeFields);
        IQueryable<ServiceRequest> Find(string keyword = null);
        IQueryable<ServiceRequest> GetRequests();
        Task<ServiceRequestSubCat> GetSubCat(long ID);
        IQueryable<ServiceRequestSubCat> GetSubCats(long ?cat_id);
        Dept Update(Dept dept);
        Task<Dept> GetDept(long id);
        Task<Dept> Create(Dept d);


        ServiceRequest Update(ServiceRequest r);
        ServiceRequestCat Update(ServiceRequestCat cat);
        ServiceRequestSubCat Update(ServiceRequestSubCat subcat);
    }
}