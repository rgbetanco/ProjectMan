using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projectman.Areas.Man.Controllers;
using projectman.Models;
using projectman.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using CSHelper.Extensions;
using CSHelper.Authorization;
using System.Data;

namespace projectman.Areas.man.Controllers
{
    [Area("man")]
    [Authorize(AuthenticationSchemes = ManDefaults.AuthenticationScheme)]
    public class CompanyController : BaseController
    {
        private readonly ICompanyRepository _comp;
        private readonly IContactRepository _contact;
        public CompanyController(ITransaction tran, ILookupRepository lookup, ICompanyRepository comp, IContactRepository persona)
           : base(tran, lookup)
        {
            _comp = comp;
            _contact = persona;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CompanyQuery(QueryVM request)
        {
            var result = _comp.FindCompanies(request.search);
            var credits = _comp.GetCreditRatings();

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                name = r.name,
                credit = r.credit_rating_code,
                remarks = r.remarks
            });
        }

        [HttpGet]
        public IActionResult New()
        {
            ViewData["credit"] = GetCreditRatingList();
            return View();
        }
        protected IEnumerable<SelectListItem> GetCreditRatingList(string ID = null)
        {
            var r = _comp.GetCreditRatings().AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.code, Text = m.name, Selected = ID == m.code });

            return result;
        }
        // new category - save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("New")]
        public async Task<Company> NewPost()
        {
            var m = new Company();

            await TryUpdateModelAsync<Company>(
                m,
                "",
                a => a.name,
                a => a.credit_rating_code,
                a => a.remarks,
                a => a.vatid,
                a => a.website
            );

            await this.TryUpdateModelListAsync(m, a => a.phones, b => b.number, b => b.type);
            await this.TryUpdateModelListAsync(m, a => a.addresses, b => b.addr, b => b.type);
            await this.TryUpdateModelListAsync(m, a => a.emails, b => b.email, b => b.type);

            m = await _comp.CreateCompany(m);

            var result = await CommitModel(m);

            return m;
        }

        public async Task<IActionResult> Edit(long ID)
        {
            Company company = await _comp.GetCompany(ID, "phones", "addresses", "emails");
            ViewData["credit"] = GetCreditRatingList(company.credit_rating_code);
            ViewData["personas"] = _contact.GetCompanyContacts(ID);
            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> Update(long ID)
        {
            Company company = await _comp.GetCompany(ID, "phones", "addresses", "emails");
            if (company == null)
            {
                return NotFound();
            }

            await this.TryUpdateModelListAsync(company, a => a.phones, b => b.number, b => b.type);
            await this.TryUpdateModelListAsync(company, a => a.addresses, b => b.addr, b => b.type);
            await this.TryUpdateModelListAsync(company, a => a.emails, b => b.email, b => b.type);

            await TryUpdateModelAsync<Company>(
                company,
                "",
                a => a.name,
                a => a.credit_rating_code,
                a => a.remarks,
                a => a.vatid,
                a => a.website
            );

            return await CommitModel(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] long ID)
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

        public IActionResult PickerWithoutIndividual()
        {
            return PartialView("_PickerPopupWithoutIndividual");
        }

        public async Task<IActionResult> PickerQuery(QueryVM request)
        {
            var result = _comp.FindCompanies(request.search, request.showIndividual);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                name = r.name,
                vatid = r.vatid
            }, true);
        }
        public async Task<IActionResult> CreditRatingSetting()
        {
            return View(await _comp.GetCreditRating().ToListAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CreditRatingSetting")]
        [AuthorizeRole(UserPermission.Edit | UserPermission.Add)]
        public async Task<IActionResult> CreditRatingSettingPost()
        {
            await this.TryUpdateTableModelAsync<CreditRating, string>(
                "CreditRating",   // data-form-table-group name
                async t =>
                {
                    await _comp.Create(t);
                    return true;
                },
                t =>
                {
                    _comp.DelCreditRatingUnsafe(t);
                    return Task.FromResult(true);
                },
                async t => await _comp.GetCreditRatingAsync(t)
            );

            var result = await CommitModel(null);

            return result;
        }

    }
}
