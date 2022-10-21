using repairman.Data;
using repairman.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace repairman.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly DBContext _context;

        public ServiceRequestRepository(DBContext context)
        {
            _context = context;
        }

        // get cats
        public IQueryable<ServiceRequestCat> FindCats(string keyword = null)
        {
            var result = _context.ServiceRequestCats.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.desc.Contains(keyword));
            }

            return result;
        }


        public IQueryable<ServiceRequestSubCat> GetSubCats(long? cat_id)
        {
            if (cat_id != null)
            {
                return _context.ServiceRequestSubCats.Where(c => c.cat_id == cat_id.Value);
            }

            return _context.ServiceRequestSubCats;
        }


        public ServiceRequestCat Update(ServiceRequestCat cat)
        {
            _context.ServiceRequestCats.Update(cat);

            return cat;
        }

        public ServiceRequestSubCat Update(ServiceRequestSubCat subcat)
        {
            _context.ServiceRequestSubCats.Update(subcat);

            return subcat;
        }

        public bool DelCatUnsafe(long ID)
        {
            var s = new ServiceRequestCat { ID = ID };

            _context.ServiceRequestCats.Attach(s);
            _context.ServiceRequestCats.Remove(s);

            return true;
        }

        public bool DelCat(ServiceRequestCat s)
        {
            _context.ServiceRequestCats.Remove(s);
            return true;
        }

        public bool DelSubCatUnsafe(long ID)
        {
            var s = new ServiceRequestSubCat { ID = ID };

            _context.ServiceRequestSubCats.Attach(s);
            _context.ServiceRequestSubCats.Remove(s);

            return true;
        }

        public async Task<ServiceRequestCat> Create(ServiceRequestCat cat)
        {
            await _context.ServiceRequestCats.AddAsync(cat);

            return cat;
        }

        public async Task<ServiceRequestSubCat> Create(ServiceRequestSubCat subcat)
        {
            await _context.ServiceRequestSubCats.AddAsync(subcat);

            return subcat;
        }

        public async Task<ServiceRequestCat> GetCat(long ID, params string[] includeFields)
        {
            var q = _context.ServiceRequestCats.AsQueryable();

            if (includeFields != null)
            {
                foreach (var field in includeFields)
                    q = q.Include(field);
            }

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);

            return a;
        }
        public async Task<ServiceRequestSubCat> GetSubCat(long ID)
        {
            var query = _context.ServiceRequestSubCats.AsQueryable();

            var a = await query.FirstOrDefaultAsync(u => u.ID == ID);

            return a;
        }

        public IQueryable<ServiceRequest> GetRequests()
        {
            var a = _context.ServiceRequests.AsQueryable();

            return a;
        }


        public async Task<ServiceRequest> Get(long ID, params string[] includeFields)
        {
            var q = _context.ServiceRequests.AsQueryable();

            if (includeFields != null)
            {
                foreach (var field in includeFields)
                    q = q.Include(field);
            }

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);

            return a;
        }

        public IQueryable<ServiceRequest> Find(string keyword = null)
        {
            var result = _context.ServiceRequests.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.desc.Contains(keyword));
            }

            return result;
        }


        public async Task<ServiceRequestReply> GetReply(long ID, params string[] includeFields)
        {
            var q = _context.ServiceRequestReplies.AsQueryable();

            if (includeFields != null)
            {
                foreach (var field in includeFields)
                    q = q.Include(field);
            }

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);

            return a;
        }

        public async Task<ServiceRequestFile> GetFile(long ID)
        {
            var q = _context.ServiceRequestFiles.AsQueryable();

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);

            return a;
        }

        public async Task<ServiceRequestReplyFile> GetReplyFile(long ID)
        {
            var q = _context.ServiceRequestReplyFiles.AsQueryable();

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);

            return a;
        }

        public async Task<ServiceRequestPic> GetPic(long ID)
        {
            var q = _context.ServiceRequestPics.AsQueryable();

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);

            return a;
        }

        public async Task<ServiceRequestReplyPic> GetReplyPic(long ID)
        {
            var q = _context.ServiceRequestReplyPics.AsQueryable();

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);

            return a;
        }

        public async Task<ServiceRequest> Create(ServiceRequest r)
        {
            r.modify_date = System.DateTime.UtcNow;

            if (r.date == default(System.DateTime))
                r.date = r.modify_date;

            await _context.ServiceRequests.AddAsync(r);
            return r;
        }

        public ServiceRequest Update(ServiceRequest r)
        {
            r.modify_date = System.DateTime.UtcNow;

            return r;
        }

        public bool DelServiceRequest(long ID)
        {
            var s = new ServiceRequest { ID = ID };

            _context.ServiceRequests.Attach(s);
            _context.ServiceRequests.Remove(s);

            return true;
        }

        public async Task<ServiceRequestReply> Create(ServiceRequestReply m)
        {
            m.date = m.modify_date = System.DateTime.UtcNow;

            await _context.ServiceRequestReplies.AddAsync(m);
            return m;
        }


        // get all 
        public IQueryable<Dept> GetDepts()
        {
            return _context.Depts;
        }

        public async Task<Dept> GetDept(long id)
        {
            return await _context.Depts.FirstOrDefaultAsync( d => d.ID == id);
        }

        public async Task<Dept> Create(Dept d)
        {
            await _context.Depts.AddAsync(d);

            return d;
        }

        public Dept Update(Dept dept)
        {
            _context.Depts.Update(dept);

            return dept;
        }

        public bool DelDeptUnsafe(long ID)
        {
            var s = new Dept { ID = ID };

            _context.Depts.Attach(s);
            _context.Depts.Remove(s);

            return true;
        }

    }
}
