using projectman.Data;
using projectman.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.InteropServices;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace projectman.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DBContext _context;

        public ProjectRepository(DBContext context)
        {
            _context = context;
        }

        // GET ALL SERVICES
        public IEnumerable<ProjectType> GetServiceType()
        {
            return Enum.GetValues(typeof(ProjectType)).Cast<ProjectType>();
        }

        public IQueryable<ProjectIncomingPayment> GetNextDueIncomingPayment()
        {
            return _context.IncomingPayments.AsQueryable();
            //var r =_context.IncomingPayments.AsQueryable().Where(r => r.project_id == ID).Where(r => String.IsNullOrEmpty(r.invoice)).OrderByDescending(m => m.issueDate).FirstOrDefaultAsync();
            //return r.issueDate.ToShortDateString();
        }

        public IQueryable<Project> FindProjects(ProjectStatus status, ProjectType service_type, string keyword = null)
        {
            var result = _context.Projects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.remarks.Contains(keyword));
            }
            if (status != ProjectStatus.Undefined)
            {
                result = result.Where(u => u.status == status);
            }
            if (service_type != ProjectType.Undefined)
            {
                result = result.Where(u => u.type == service_type);
            }

            return result;
        }

        public IQueryable<ProjectIncomingPayment> FindIncomingPaymentByCompanyId(long company_id, string keyword)
        {
            var result = _context.IncomingPayments.Include("project").AsQueryable();

            if (company_id > -1)
            {
                result = result.Where(u => u.project.company_id == company_id);
            }

            if (!String.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.item == keyword);
            }

            return result;
        }

        public IQueryable<Project> FindAllProject()
        {
            return _context.Projects.AsQueryable();
        }

        public IQueryable<Project> GetAllProject()
        {
            return _context.Projects.Include(m => m.incoming_payments.OrderByDescending(m => m.due_date)).OrderBy(m => m.starting_datetime);
        }

        public async Task<Project> CreateProject(Project u)
        {
            await _context.Projects.AddAsync(u);

            return u;
        }

        public async Task<Project> DupProject(Project u)
        {
            Project n = new Project();
            n.name = u.name;
            n.number = u.number;
            n.remarks = u.remarks;
            n.type = u.type;
            n.status = u.status;
            n.starting_datetime = u.starting_datetime;
            n.ending_datetime = u.ending_datetime;
            n.user_id = u.user_id;
            n.importance_id = u.importance_id;
            n.company_id = u.company_id;
            n.contact_id = u.contact_id;
            n.contact_address = u.contact_address;
            n.contact_phone = u.contact_phone;
            n.connected_project_id = u.ID;

            await _context.Projects.AddAsync(n);
            await _context.SaveChangesAsync();

            var proj = await _context.Projects.FindAsync(n.ID);
            if (proj != null)
            {
                var product_list = new List<ProjectProduct>();
                foreach (var i in u.products)
                {
                    product_list.Add(new ProjectProduct
                    {
                        project_id = proj.ID,
                        product_id = i.product_id,
                        serial_number = i.serial_number
                    });
                }

                var incoming_payment_list = new List<ProjectIncomingPayment>();
                foreach (var i in u.incoming_payments)
                {
                    incoming_payment_list.Add(new ProjectIncomingPayment
                    {
                        project_id = proj.ID,
                        due_date = i.due_date,
                        item = i.item,
                        amount = i.amount,
                        invoice_date = i.invoice_date,
                        orderslip_date = i.orderslip_date,
                        invoice_number = i.invoice_number,
                        orderslip_number = i.orderslip_number
                    });
                }

                var outgoing_payment_list = new List<ProjectOutgoingPayment>();
                foreach (var i in u.outgoing_payments)
                {
                    outgoing_payment_list.Add(new ProjectOutgoingPayment
                    {
                        project_id = proj.ID,
                        due_date = i.due_date,
                        company_id = i.company_id,
                        amount = i.amount
                    });
                }

                proj.products = product_list;
                proj.incoming_payments = incoming_payment_list;
                proj.outgoing_payments = outgoing_payment_list;

                await _context.SaveChangesAsync();
            }

            return n;
        }

        public bool DelCompany(Company s)
        {
            _context.Companies.Remove(s);
            return true;
        }

        public async Task<Project> GetProject(long ID, params string[] includeFields)
        {
            var q = _context.Projects.AsQueryable();

            if (includeFields != null)
            {
                foreach (var field in includeFields)
                    q = q.Include(field);
            }

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);
            return a;
        }

        public bool DelProject(Project s)
        {
            _context.Projects.Remove(s);
            return true;
        }

        public IQueryable<Project> FindProjects(string keyword = null)
        {
            var result = _context.Projects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.remarks.Contains(keyword));
            }

            return result;
        }

        // GET PROJECT SUBTYPES
        public IQueryable<ProjectSubtype> GetSubtypes(ProjectType ?type)
        {
            var q = _context.ProjectSubtypes.AsQueryable();
            if( type!=null )
            {
                q = q.Where(t => t.type == type);
            }

            return q;
        }

        // GET ALL IMPORTANCES
        public IQueryable<ProjectImportance> GetImportances()
        {
            return _context.ProjectImportances.AsQueryable();
        }

        public async Task<ProjectImportance> Create(ProjectImportance t)
        {
            await _context.ProjectImportances.AddAsync(t);
            return t;
        }

        public async Task<ProjectImportance> GetImportanceAsync(string t)
        {
            var q = GetImportances();
            var s = await q.FirstOrDefaultAsync(u => u.code == t);
            return s;
        }

        public void DelImportanceUnsafe(string t)
        {
            var s = new ProjectImportance { code = t };

            _context.ProjectImportances.Attach(s);
            _context.ProjectImportances.Remove(s);
        }
    }
}
