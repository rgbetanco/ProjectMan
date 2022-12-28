using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CSHelper.Extensions;
using projectman.Repositories;
using projectman.Models;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using static projectman.Models.History;
using Microsoft.Identity.Client;
using Azure.Core;
using CSHelper.Authorization;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using Microsoft.CodeAnalysis.Options;
using static System.Net.Mime.MediaTypeNames;

namespace projectman.Areas.Man.Controllers
{
    [Area("man")]
    [Authorize(AuthenticationSchemes = ManDefaults.AuthenticationScheme)]
    public class ProjectController : BaseController
    {
        private readonly IProjectRepository _proj;
        private readonly IUserRepository _user;
        private readonly ICompanyRepository _comp;
        private readonly IContactRepository _persona;
        public ProjectController(ITransaction tran, ILookupRepository lookup, IProjectRepository proj, IUserRepository user, ICompanyRepository comp, IContactRepository persona)
           : base(tran, lookup)
        {
            _proj = proj;
            _user = user;
            _comp = comp;
            _persona = persona;
        }

        protected IEnumerable<SelectListItem> GetGroupList()
        {
            var r = _user.GetGroups().AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name });

            return result;
        }

        protected IEnumerable<SelectListItem> GetInternalCompanyList()
        {
            var r = _comp.GetInternalCompanies().AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name });

            return result;
        }

        protected IEnumerable<SelectListItem> GetSalePersonList()
        {
            var r = _user.GetSalesPersons().AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name });

            return result;
        }

        protected IEnumerable<SelectListItem> GetImportanceList()
        {
            var r = _proj.GetImportances().AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.code, Text = m.name });

            return result;
        }

        protected IEnumerable<SelectListItem> GetServiceTypeList()
        {
            var r = _proj.GetServiceType();

            var result = r.Select(m => new SelectListItem { Value = "" + (int)m, Text = m.GetDisplayName() });

            return result;
        }
        protected object GetProjectSubtypeList(ProjectType type, IEnumerable<long> selectedValues = null)
        {
            var r = _proj.GetSubtypes(type).AsNoTracking();
            var obj = new { results = new List<ProjectSubtypeSelect2> { } };
            foreach(ProjectSubtype o in r)
            {
                obj.results.Add(new ProjectSubtypeSelect2 { id = o.ID, text = o.name });
            }
            return obj;
        }
        protected object GetSubtypesByCompanyID(long ID)
        {
            var r = _proj.GetProject(ID, "subtypes", "subtypes.subtype");
            var subtypes = r.Result.subtypes;
            var obj = new { results = new List<ProjectSubtypeSelect2> { } };
            foreach (ProjectSubtypeEntry o in subtypes)
            {
                obj.results.Add(new ProjectSubtypeSelect2 { id = o.ID, text = o.subtype.name, selected = true });
            }
            return obj;
        }

        protected IEnumerable<SelectListItem> GetSubtypesByCompanyID2(long ID)
        {
            var r = _proj.GetProject(ID, "subtypes", "subtypes.subtype");

            var result = r.Result.subtypes.Select(m => new SelectListItem { Value = "" + (int)m.subtype_id, Text = m.subtype.name, Selected = true });

            return result;
        }

        protected IEnumerable<SelectListItem> GetCompanyContactList(long comp_id)
        {
            if (comp_id == 1)
            {
                var c = _persona.GetContacts().Select(n => new SelectListItem { Value = n.ID.ToString(), Text = n.name });
                if (!c.Any())
                {
                    c.Append(new SelectListItem { Value = "所有", Text = "所有" });
                }
                return c;
            }

            var r = _persona.GetCompanyContacts(comp_id).Select(n => new SelectListItem { Value = n.ID.ToString(), Text = n.name });
            if (!r.Any())
            {
                r.Append(new SelectListItem { Value = "所有", Text = "所有" });
            }
            return r;
        }

        public async Task<IActionResult> ProjectQuery(QueryVM request)
        {
            var result = _proj.FindProjects((ProjectStatus)request.status, (ProjectType)request.service_type, request.search);
            var _nextInvoices = _proj.GetNextDueIncomingPayment();
            var _nextInvoice = _nextInvoices.Where(r => String.IsNullOrEmpty(r.invoice_number)).OrderByDescending(r => r.due_date);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                number = r.number,
                name = r.name,
                remarks = r.remarks,
                nextInvoice = String.IsNullOrEmpty(_nextInvoice.FirstOrDefault(a => a.project_id == r.ID).due_date.ToString()) ? "無" : _nextInvoice.FirstOrDefault(a => a.project_id == r.ID).due_date.ToShortDateString(),
            });
        }

        public async Task<IActionResult> ProjectAllOld(QueryVM request)
        {
            var result = _proj.FindAllProject();
            var _nextInvoices = _proj.GetNextDueIncomingPayment();
            var _nextInvoice = _nextInvoices.Where(r => String.IsNullOrEmpty(r.invoice_number)).OrderByDescending(r => r.due_date);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                number = r.number,
                name = r.name,
                remarks = r.remarks,
                nextInvoice = String.IsNullOrEmpty(_nextInvoice.FirstOrDefault(a => a.project_id == r.ID).due_date.ToString()) ? "無" : _nextInvoice.FirstOrDefault(a => a.project_id == r.ID).due_date.ToShortDateString(),
                item = String.IsNullOrEmpty(_nextInvoice.FirstOrDefault(a => a.project_id == r.ID).item) ? "沒有" : _nextInvoice.FirstOrDefault(a => a.project_id == r.ID).item
            });
        }

        public IActionResult ProjectAll()
        {
            List<DashboardView> v = new List<DashboardView>();

            var result = _proj.GetAllProject();
            
            foreach(Project p in result)
            {
                foreach(ProjectTimelineEntry e in p.timelines)
                {
                    v.Add(new DashboardView
                    {
                        project_id = p.ID,
                        project_name = p.name,
                        entry_name = e.desc,
                        due_date = e.due_date
                    });
                }
                foreach(ProjectIncomingPayment i in p.incoming_payments)
                {
                    v.Add(new DashboardView 
                    {
                        project_id = p.ID,
                        project_name = p.name,
                        entry_name = i.item,
                        due_date = i.due_date
                    });
                }
            }

            return Json(v); 
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult New()
        {
            var a = new Project();
            a.type = ProjectType.DevelopmentContract;
            ViewData["group"] = GetGroupList();
            ViewData["internal_company"] = GetInternalCompanyList();
            ViewData["sales_person"] = GetSalePersonList();
            ViewData["importance"] = GetImportanceList();
            ViewData["service_type"] = GetServiceTypeList();
            // generate list of modules for dropdown lists
            ViewData["subtypes"] = GetProjectSubtypeList(a.type);
            ViewData["personas"] = GetCompanyContactList(-1);

            a.starting_datetime = DateTime.UtcNow;
            a.ending_datetime = a.starting_datetime.AddMonths(3);

            

            return View(a);
        }

        // new category - save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("New")]
        public async Task<IActionResult> NewPost(string[] subtypes)
        {
            var m = new Project();

            await TryUpdateModelAsync<Project>(
                m,
                "",
                a => a.name,
                a => a.number,
                a => a.type,
                a => a.status,
                a => a.starting_datetime,
                a => a.ending_datetime,
                a => a.user_id,
                a => a.group_id,
                a => a.internal_company_id,
                a => a.importance_id,
                a => a.company_id,
                a => a.contact_id,
                a => a.contact_address,
                a => a.contact_phone,
                a => a.remarks,
                a => a.total_income_amount,
                a => a.total_pay_amount,
                a => a.net_income
            );

            await this.TryUpdateModelListAsync(m, a => a.products, b => b.product_id, b => b.serial_number);
            await this.TryUpdateModelListAsync(m, a => a.timelines, b => b.due_date, b => b.desc, b => b.complete_date);
            await this.TryUpdateModelListAsync(m, a => a.incoming_payments, b => b.due_date, b => b.item, b => b.amount, b => b.invoice_number, b => b.orderslip_number, b => b.invoice_date, b => b.orderslip_date);
            await this.TryUpdateModelListAsync(m, a => a.outgoing_payments, b => b.due_date, b => b.company_id, b => b.amount);

            var s = new List<ProjectSubtypeEntry>();
            foreach(string sub in subtypes)
            {
                s.Add(new ProjectSubtypeEntry { subtype_id = long.Parse(sub) });
            }

            m.subtypes = s;

            await _proj.CreateProject(m);
            var result = await CommitModel(m);

            return result;
        }

        public async Task<IActionResult> View(long ID)
        {
            Project project = await _proj.GetProject(ID, "products", "products.product", "products.product.brand","incoming_payments", "outgoing_payments", "subtypes", "timelines");

            ViewData["group"] = GetGroupList();
            ViewData["internal_company"] = GetInternalCompanyList();
            ViewData["sales_person"] = GetSalePersonList();
            ViewData["importance"] = GetImportanceList();
            ViewData["service_type"] = GetServiceTypeList();
            ViewData["subtypes"] = GetSubtypesByCompanyID2(ID);

            // generate list of modules for dropdown lists
            ViewData["personas"] = GetCompanyContactList(-1);
            if (project.company_id != null)
            {
                ViewData["personas"] = GetCompanyContactList((long)project.company_id);
                project.company = await _comp.GetCompany((long)project.company_id);   
            }

            foreach(ProjectOutgoingPayment m in project.outgoing_payments)
            {
                m.company = await _comp.GetCompany((long)m.company_id);
            }
            
            if (project.user_id != null)
            {
                project.user = await _user.Get((long)project.user_id);
            }
            if (project.contact_id != null)
            {
                project.contact = await _persona.Get((long)project.contact_id);
            }
            return View(project);
        }

        [HttpPost]
        public async Task<IActionResult> Update(long ID)
        {
            Project project = await _proj.GetProject(ID, "products", "products.product", "products.product.brand", "incoming_payments", "outgoing_payments","subtypes","timelines");
            if (project == null)
            {
                return NotFound();
            }

            await this.TryUpdateModelListAsync(project, a => a.products, b => b.product_id, b => b.serial_number);
            await this.TryUpdateModelListAsync(project, a => a.incoming_payments, b => b.due_date, b => b.item, b => b.amount, b => b.invoice_number, b => b.orderslip_number, b => b.invoice_date, b => b.orderslip_date);
            await this.TryUpdateModelListAsync(project, a => a.outgoing_payments, b => b.due_date, b => b.company_id, b => b.amount);
            await this.TryUpdateModelListAsync(project, a => a.subtypes, b => b.subtype_id);
            await this.TryUpdateModelListAsync(project, a => a.timelines, b => b.due_date, b => b.desc, b => b.complete_date);

            await TryUpdateModelAsync<Project>(
                project,
                "",
                a => a.name,
                a => a.number,
                a => a.type,
                a => a.group_id,
                a => a.internal_company_id,
                a => a.status,
                a => a.starting_datetime,
                a => a.ending_datetime,
                a => a.user_id,
                a => a.importance_id,
                a => a.company_id,
                a => a.contact_id,
                a => a.contact_address,
                a => a.contact_phone,
                a => a.remarks,
                a => a.total_income_amount,
                a => a.total_pay_amount,
                a => a.net_income
            );

            return await CommitModel(project);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ListSubTypeByCompanyId(long ID)
        {
            return Json(GetSubtypesByCompanyID(ID));
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ListSubType(string comp_type)
        {
            if (String.IsNullOrEmpty(comp_type))
            {
                return Json(GetProjectSubtypeList(ProjectType.DevelopmentContract));
            }
            ProjectType t = Enum.Parse<ProjectType>(comp_type);
            return Json(GetProjectSubtypeList(t));
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ListPersonaPerCompany(long ID)
        {
            return Json(GetCompanyContactList(ID));
        }
            
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ConfirmDeleteProject(long ID)
        {
            ViewData["ID"] = ID;
            return PartialView("_ConfirmDeleteProjectPopup");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProject([FromForm] long ID)
        {
            var c = await _proj.GetProject(ID);

            if (c == null)
            {
                return NotFound();
            }

            _proj.DelProject(c);

            ViewData["ID"] = ID;

            await _tran.Commit();

            return Ok();
        }

        public IActionResult RenewContractPopUp(long ID)
        {
            ViewData["ID"] = ID;
            return PartialView("_RenewContractPopUp");
        }
        
        public async Task<IActionResult> SubtypeSetting()
        {
            ViewData["product_type"] = GetServiceTypeList();
            return View(await _proj.GetProjectSubtypes().ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SubtypeSetting")]
        [AuthorizeRole(UserPermission.Edit | UserPermission.Add)]
        public async Task<IActionResult> SubtypeSettingPost()
        {
            await this.TryUpdateTableModelAsync<ProjectSubtype, string>(
                "ProjectSubtype",   // data-form-table-group name
                async t =>
                {
                    await _proj.CreateProjectSubtype(t);
                    return true;
                },
                t =>
                {
                    _proj.DelProjectSubtypeUnsafe(long.Parse(t));
                    return Task.FromResult(true);
                },
                async t => await _proj.GetProjectSubtypeAsync(long.Parse(t))
            );

            var result = await CommitModel(null);

            return result;
        }

        public async Task<IActionResult> ImportanceSetting()
        {
            return View(await _proj.GetImportances().ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ImportanceSetting")]
        [AuthorizeRole(UserPermission.Edit | UserPermission.Add)]
        public async Task<IActionResult> ImportanceSettingPost()
        {
            await this.TryUpdateTableModelAsync<ProjectImportance, string>(
                "Importance",   // data-form-table-group name
                async t =>
                {
                    await _proj.Create(t);
                    return true;
                },
                t =>
                {
                    _proj.DelImportanceUnsafe(t);
                    return Task.FromResult(true);
                },
                async t => await _proj.GetImportanceAsync(t)
            );

            var result = await CommitModel(null);

            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewContract([FromForm]long ID)
        {
            var c = await _proj.GetProject(ID, "products", "incoming_payments", "outgoing_payments");

            if (c == null)
            {
                return NotFound();
            }

            await _proj.DupProject(c);

            ViewData["ID"] = ID;

            await _tran.Commit();
            return Ok();
        }

        // picker
        public IActionResult Picker(string company_id)
        {
            ViewBag.company_id = company_id;   
            return PartialView("_PickerPopup");
        }

        public async Task<IActionResult> PickerQuery(QueryVM request)
        {
            var result = _proj.FindIncomingPaymentByCompanyId((long)request.company_id, request.search);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                item = r.item,
                amount = r.amount
            }, true);
        }
    }
}
