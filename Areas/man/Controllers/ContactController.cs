using CSHelper.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using projectman.Areas.Man.Controllers;
using projectman.Models;
using projectman.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectman.Areas.man.Controllers
{ 
    [Area("man")]
    [Authorize(AuthenticationSchemes = ManDefaults.AuthenticationScheme)]
    public class ContactController : BaseController
    {
        private readonly IContactRepository _persona;
        private readonly ICompanyRepository _comp;
        public ContactController(ITransaction tran, ILookupRepository lookup, IContactRepository persona, ICompanyRepository comp): base(tran, lookup)
        {
            _persona = persona;
            _comp = comp;
        }
        public IActionResult Index()
        {
            return View();
        }

        // picker
        public IActionResult Picker()
        {
            return PartialView("_PickerPopup");
        }

        public async Task<IActionResult> PersonaQuery(QueryVM request)
        {
            var result = _persona.Find(request.search, request.searchPhone);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                name = r.name,
                company = r.companies.FirstOrDefault().company.name,
                phone = r.phones.FirstOrDefault().number,
                remarks = r.remarks
            });
        }

        public async Task<IActionResult> PickerQuery (QueryVM request)
        {
            var result = _persona.Find(request.search);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                name = r.name
            }, true);
        }

        [HttpGet]
        public async Task<IActionResult> New(long? company_id)
        {
            ViewData["company_id"] = "";
            ViewData["company_name"] = "";

            if (company_id.HasValue) {
                Company comp = await _comp.GetCompany((long)company_id);
                ViewData["company_id"] = comp.ID;
                ViewData["company_name"] = comp.name;
            }
            
            return View();
        }
        
        // new contact - save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("New")]
        public async Task<long> NewPost()
        {
            var m = new Contact();

            await TryUpdateModelAsync<Contact>(
                m,
                "",
                a => a.name,
                a => a.department,
                a => a.remarks
            );

            await this.TryUpdateModelListAsync(m, a => a.phones, b => b.number, b => b.type, b => b.is_default);
            await this.TryUpdateModelListAsync(m, a => a.addresses, b => b.addr, b => b.type);
            await this.TryUpdateModelListAsync(m, a => a.emails, b => b.email, b => b.type);
            await this.TryUpdateModelListAsync(m, a => a.companies, b => b.company_id, b => b.job_title);

            await _persona.Create(m);
            var result = await CommitModel(m);

            return m.ID;
        }

        public async Task<IActionResult> Edit(long ID)
        {
            Contact m = await _persona.Get(ID, "phones", "addresses", "emails", "companies");
            
            if (m.companies.Any())
            {
                foreach(var company in m.companies)
                {
                    company.company = await _comp.GetCompany(company.company_id);
                }
            }
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> Update(long ID)
        {
            Contact m = await _persona.Get(ID, "phones", "addresses", "emails", "companies");
            if (m == null)
            {
                return NotFound();
            }

            await this.TryUpdateModelListAsync(m, a => a.phones, b => b.number, b => b.type, b => b.is_default);
            await this.TryUpdateModelListAsync(m, a => a.addresses, b => b.addr, b => b.type);
            await this.TryUpdateModelListAsync(m, a => a.emails, b => b.email, b => b.type);
            await this.TryUpdateModelListAsync(m, a => a.companies, b => b.company_id, b => b.job_title);

            await TryUpdateModelAsync<Contact>(
                m,
                "",
                a => a.name,
                a => a.department,
                a => a.remarks
            );

            return await CommitModel(m);
        }

        public IActionResult ConfirmDeletePersona(long ID)
        {
            ViewData["ID"] = ID;
            return PartialView("_ConfirmDeletePersonaPopup");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePersona([FromForm] long ID)
        {
            var c = await _persona.Get(ID);

            if (c == null)
            {
                return NotFound();
            }

            _persona.Del(c);

            ViewData["ID"] = ID;

            await _tran.Commit();

            return Ok();
        }
    
    }
}
