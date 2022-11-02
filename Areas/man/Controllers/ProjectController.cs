﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CSHelper.Extensions;
using repairman.Repositories;
using repairman.Models;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using static repairman.Models.History;
using Microsoft.Identity.Client;
using Azure.Core;

namespace repairman.Areas.Man.Controllers
{
    [Area("man")]
    [Authorize(AuthenticationSchemes = ManDefaults.AuthenticationScheme)]
    public class ProjectController : BaseController
    {
        private readonly IProjectRepository _proj;
        private readonly IUserRepository _user;
        private readonly ICompanyRepository _comp;
        private readonly IPersonaRepository _persona;
        public ProjectController(ITransaction tran, ILookupRepository lookup, IProjectRepository proj, IUserRepository user, ICompanyRepository comp, IPersonaRepository persona)
           : base(tran, lookup)
        {
            _proj = proj;
            _user = user;
            _comp = comp;
            _persona = persona;
        }

        protected IEnumerable<SelectListItem> GetSalePersonList()
        {
            var r = _user.GetSalesPersons().AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name });

            return result;
        }

        protected IEnumerable<SelectListItem> GetImportanceList()
        {
            var r = _proj.GetImportances();

            var result = r.Select(m => new SelectListItem { Value = ""+(int)m, Text = m.ToString() });

            return result;
        }

        protected IEnumerable<SelectListItem> GetServiceTypeList()
        {
            var r = _proj.GetServiceType();

            var result = r.Select(m => new SelectListItem { Value = "" + (int)m, Text = m.ToString() });

            return result;
        }

        protected IEnumerable<SelectListItem> GetCategoryList()
        {
            var r = _proj.GetCategoryList();

            var result = r.Select(m => new SelectListItem { Value = "" + (int)m, Text = m.ToString() });

            return result;
        }
        
        protected IEnumerable<SelectListItem> GetPersonasList(long comp_id)
        {
            var r = _persona.GetPersonaPerCompany(comp_id).Select(n => new SelectListItem { Value = n.ID.ToString(), Text = n.name });
            if (!r.Any())
            {
                r.Append(new SelectListItem { Value = "所有", Text = "所有" });
            }
            return r;
        }

        public async Task<IActionResult> ProjectQuery(QueryVM request)
        {
            var result = _proj.FindProjects((ProjectStatusEnum)request.status, (ServiceTypeEnum)request.service_type, request.search);
            var _nextInvoices = _proj.GetNextDueIncomingPayment();
            var _nextInvoice = _nextInvoices.Where(r => String.IsNullOrEmpty(r.invoice)).OrderByDescending(r => r.issueDate);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                number = r.number,
                name = r.name,
                remarks = r.remarks,
                nextInvoice = String.IsNullOrEmpty(_nextInvoice.FirstOrDefault(a => a.project_id == r.ID).issueDate.ToString()) ? "沒有" : _nextInvoice.FirstOrDefault(a => a.project_id == r.ID).issueDate.ToShortDateString(),
            });
        }

        public async Task<IActionResult> ProjectAllOld(QueryVM request)
        {
            var result = _proj.FindAllProject();
            var _nextInvoices = _proj.GetNextDueIncomingPayment();
            var _nextInvoice = _nextInvoices.Where(r => String.IsNullOrEmpty(r.invoice)).OrderByDescending(r => r.issueDate);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                number = r.number,
                name = r.name,
                remarks = r.remarks,
                nextInvoice = String.IsNullOrEmpty(_nextInvoice.FirstOrDefault(a => a.project_id == r.ID).issueDate.ToString()) ? "沒有" : _nextInvoice.FirstOrDefault(a => a.project_id == r.ID).issueDate.ToShortDateString(),
                item = String.IsNullOrEmpty(_nextInvoice.FirstOrDefault(a => a.project_id == r.ID).item) ? "沒有" : _nextInvoice.FirstOrDefault(a => a.project_id == r.ID).item
            });
        }

        public async Task<IActionResult> ProjectAll(QueryVM request)
        {
            var result = _proj.GetAllProject();
            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                number = r.number,
                name = r.name,
                remarks = r.remarks,
                starting_date = r.starting_datetime,
                nextInvoice = r.incoming_payment.FirstOrDefault().issueDate,
                item = r.incoming_payment.FirstOrDefault().item
            }); 
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ProjectNew()
        {
            var a = new ProjectModel();
            ViewData["sales_person"] = GetSalePersonList();
            ViewData["importance"] = GetImportanceList();
            ViewData["service_type"] = GetServiceTypeList();
            ViewData["categories"] = GetCategoryList();
            // generate list of modules for dropdown lists
            ViewData["brands"] = _proj.GetBrands().Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.brand_name });
            ViewData["models"] = _proj.GetModels().Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.model_name });
            ViewData["personas"] = GetPersonasList(-1);

            a.starting_datetime = DateTime.UtcNow;
            a.ending_datetime = a.starting_datetime.AddMonths(3);

            

            return View(a);
        }

        // new category - save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ProjectNew")]
        public async Task<IActionResult> ProjectNewPost()
        {
            var m = new ProjectModel();

            await TryUpdateModelAsync<ProjectModel>(
                m,
                "",
                a => a.name,
                a => a.number,
                a => a.service_type,
                a => a.status,
                a => a.starting_datetime,
                a => a.ending_datetime,
                a => a.user_id,
                a => a.importance_id,
                a => a.company_id,
                a => a.persona_id,
                a => a.contact_address,
                a => a.contact_phone,
                a => a.remarks
            );

            await TryUpdateModelListAsync(m, a => a.product_list, b => b.product_brand_id, b => b.product_model_id, b => b.serial_number);
            await TryUpdateModelListAsync(m, a => a.incoming_payment, b => b.issueDate, b => b.item, b => b.amount, b => b.invoice);
            await TryUpdateModelListAsync(m, a => a.outgoing_payment, b => b.issueDate, b => b.company_id, b => b.amount);
            
            await _proj.CreateProject(m);
            var result = await CommitModel(m);

            return result;
        }

        [ActionName("ProjectEdit")]
        public async Task<IActionResult> ProjectEdit(long ID)
        {
            ProjectModel project = await _proj.GetProject(ID, "product_list", "incoming_payment", "outgoing_payment");

            ViewData["sales_person"] = GetSalePersonList();
            ViewData["importance"] = GetImportanceList();
            ViewData["service_type"] = GetServiceTypeList();
            ViewData["categories"] = GetCategoryList();
            // generate list of modules for dropdown lists
            ViewData["brands"] = _proj.GetBrands().Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.brand_name });
            ViewData["models"] = _proj.GetModels().Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.model_name });
            ViewData["personas"] = GetPersonasList(-1);
            if (project.company_id != null)
            {
                ViewData["personas"] = GetPersonasList((long)project.company_id);
                project.company = await _comp.GetCompany((long)project.company_id);   
            }

            foreach(OutgoingPaymentModel m in project.outgoing_payment)
            {
                m.company = await _comp.GetCompany((long)m.company_id);
            }
            
            if (project.user_id != null)
            {
                project.user = await _user.Get((long)project.user_id);
            }
            if (project.persona_id != null)
            {
                project.persona = await _persona.GetPersona((long)project.persona_id);
            }
            return View(project);
        }

        [HttpPost]
        public async Task<IActionResult> ProjectUpdate(long ID)
        {
            ProjectModel project = await _proj.GetProject(ID, "product_list", "incoming_payment", "outgoing_payment");
            if (project == null)
            {
                return NotFound();
            }

            await TryUpdateModelListAsync(project, a => a.product_list, b => b.category, b => b.product_brand_id, b => b.product_model_id, b => b.serial_number);
            await TryUpdateModelListAsync(project, a => a.incoming_payment, b => b.issueDate, b => b.item, b => b.amount, b => b.invoice);
            await TryUpdateModelListAsync(project, a => a.outgoing_payment, b => b.issueDate, b => b.company_id, b => b.amount);

            await TryUpdateModelAsync<ProjectModel>(
                project,
                "",
                a => a.name,
                a => a.number,
                a => a.service_type,
                a => a.status,
                a => a.starting_datetime,
                a => a.ending_datetime,
                a => a.user_id,
                a => a.importance_id,
                a => a.company_id,
                a => a.persona_id,
                a => a.contact_address,
                a => a.contact_phone,
                a => a.remarks
            );

            return await CommitModel(project);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ListPersonaPerCompany(long ID)
        {
            return Json(GetPersonasList(ID));
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ListBrandForCategory(long ID)
        {
            foreach(ProductCategoryEnum i in Enum.GetValues(typeof(ProductCategoryEnum))){
                if (((long)i) == ID) {
                    return Json(_proj.GetCategoryBrandList(i));
                }
            }
            return Json(_proj.GetCategoryBrandList(ProductCategoryEnum.CategoryA));
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ListModelForCategory(long ID)
        {
            foreach (ProductCategoryEnum i in Enum.GetValues(typeof(ProductCategoryEnum)))
            {
                if (((long)i) == ID)
                {
                    return Json(_proj.GetCategoryModelList(i));
                }
            }
            return Json(_proj.GetCategoryModelList(ProductCategoryEnum.CategoryA));
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewContract([FromForm]long ID)
        {
            var c = await _proj.GetProject(ID, "product_list", "incoming_payment", "outgoing_payment");

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
            var result = _proj.FindIncomingPaymentByCompanyId((long)request.company_id);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                item = r.item,
                amount = r.amount
            }, true);
        }
    }
}
