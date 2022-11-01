using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using repairman.Areas.Man.Controllers;
using repairman.Models;
using repairman.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace repairman.Areas.man.Controllers
{
    [Area("man")]
    [Authorize(AuthenticationSchemes = ManDefaults.AuthenticationScheme)]
    public class CompanyController : BaseController
    {
        private readonly ICompanyRepository _comp;
        private readonly IPersonaRepository _persona;
        public CompanyController(ITransaction tran, ILookupRepository lookup, ICompanyRepository comp, IPersonaRepository persona)
           : base(tran, lookup)
        {
            _comp = comp;
            _persona = persona;
        }
        public IActionResult Index()
        {
            var c = _comp.GetCompanies();
            return View(c);
        }

        public async Task<IActionResult> CompanyQuery(QueryVM request)
        {
            var result = _comp.FindCompanies(request.search);
            var credits = _comp.GetCredits();

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                name = r.name,
                credit = credits.FirstOrDefault(a => a.ID == r.credit_id).name,
                remarks = r.remarks
            });
        }

        [HttpGet]
        public IActionResult CompanyNew()
        {
            ViewData["credit"] = GetCreditList();
            return View();
        }
        protected IEnumerable<SelectListItem> GetCreditList(long ID = 0)
        {
            var r = _comp.GetCredits().AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name, Selected = ID == m.ID });

            return result;
        }
        // new category - save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CompanyNew")]
        public async Task<IActionResult> CompanyNewPost()
        {
            var m = new CompanyModel();

            await TryUpdateModelAsync<CompanyModel>(
                m,
                "",
                a => a.name,
                a => a.credit_id,
                a => a.remarks,
                a => a.nationalID,
                a => a.website
            );

            await TryUpdateModelListAsync(m, a => a.phone, b => b.number, b => b.type);
            await TryUpdateModelListAsync(m, a => a.address, b => b.addr, b => b.type);
            await TryUpdateModelListAsync(m, a => a.email, b => b.email, b => b.type);

            m = await _comp.CreateCompany(m);

            var result = await CommitModel(m);

            return result;
        }

        [ActionName("CompanyEdit")]
        public async Task<IActionResult> CompanyEdit(long ID)
        {
            Debug.WriteLine("Hello world from the controller");
            CompanyModel company = await _comp.GetCompany(ID, "phone", "address", "email");
            ViewData["credit"] = GetCreditList((long)company.credit_id);
            ViewData["personas"] = _persona.GetPersonaPerCompany(ID);
            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> CompanyUpdate(long ID)
        {
            CompanyModel company = await _comp.GetCompany(ID, "phone", "address", "email");
            if (company == null)
            {
                return NotFound();
            }

            await TryUpdateModelListAsync(company, a => a.phone, b => b.number, b => b.type);
            await TryUpdateModelListAsync(company, a => a.address, b => b.addr, b => b.type);
            await TryUpdateModelListAsync(company, a => a.email, b => b.email, b => b.type);

            await TryUpdateModelAsync<CompanyModel>(
                company,
                "",
                a => a.name,
                a => a.credit_id,
                a => a.remarks,
                a => a.nationalID,
                a => a.website
            );

            return await CommitModel(company);
        }
        public IActionResult ConfirmDeleteCompany(long ID)
        {
            ViewData["ID"] = ID;
            return PartialView("_ConfirmDeleteCompanyPopup");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompany([FromForm] long ID)
        {
            var c = await _comp.GetCompany(ID);

            if (c == null)
            {
                return NotFound();
            }

            _comp.DelCompany(c);

            ViewData["ID"] = ID;

            await _tran.Commit();

            return Ok();
        }

        // picker
        public IActionResult Picker()
        {
            return PartialView("_PickerPopup");
        }

        public async Task<IActionResult> PickerQuery(QueryVM request)
        {
            var result = _comp.FindCompanies(request.search);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                name = r.name,
                vatid = r.nationalID
            }, true);
        }

    }
}
