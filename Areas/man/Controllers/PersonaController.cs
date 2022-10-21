using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using repairman.Areas.Man.Controllers;
using repairman.Models;
using repairman.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace repairman.Areas.man.Controllers
{ 
    [Area("man")]
    [Authorize(AuthenticationSchemes = ManDefaults.AuthenticationScheme)]
    public class PersonaController : BaseController
    {
        private readonly IPersonaRepository _persona;
        private readonly ICompanyRepository _comp;
        public PersonaController(ITransaction tran, ILookupRepository lookup, IPersonaRepository persona, ICompanyRepository comp): base(tran, lookup)
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
            var result = _persona.FindPersonas(request.search, request.searchPhone);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                name = r.name,
                company = r.personas_company.FirstOrDefault().company.name,
                phone = r.phone.FirstOrDefault().number,
                remarks = r.remarks
            });
        }

        public async Task<IActionResult> PickerQuery (QueryVM request)
        {
            var result = _persona.FindPersonas(request.search);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                name = r.name
            }, true);
        }

        [HttpGet]
        public IActionResult PersonaNew()
        {
            return View();
        }
        
        // new category - save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("PersonaNew")]
        public async Task<IActionResult> PersonaNewPost()
        {
            var m = new PersonaModel();

            await TryUpdateModelAsync<PersonaModel>(
                m,
                "",
                a => a.name,
                a => a.department,
                a => a.remarks
            );

            await TryUpdateModelListAsync(m, a => a.phone, b => b.number, b => b.type, b => b.default_number);
            await TryUpdateModelListAsync(m, a => a.address, b => b.addr, b => b.type);
            await TryUpdateModelListAsync(m, a => a.email, b => b.email, b => b.type);
            await TryUpdateModelListAsync(m, a => a.personas_company, b => b.company_id, b => b.job_title);

            await _persona.CreatePersona(m);
            var result = await CommitModel(m);

            return result;
        }

        [ActionName("PersonaEdit")]
        public async Task<IActionResult> PersonaEdit(long ID)
        {
            PersonaModel persona = await _persona.GetPersona(ID, "phone", "address", "email", "personas_company");
            
            if (persona.personas_company.Any())
            {
                foreach(PersonaCompanyModel company in persona.personas_company)
                {
                    company.company = await _comp.GetCompany(company.company_id);
                }
            }
            return View(persona);
        }

        [HttpPost]
        public async Task<IActionResult> PersonaUpdate(long ID)
        {
            PersonaModel persona = await _persona.GetPersona(ID, "phone", "address", "email", "personas_company");
            if (persona == null)
            {
                return NotFound();
            }

            await TryUpdateModelListAsync(persona, a => a.phone, b => b.number, b => b.type, b => b.default_number);
            await TryUpdateModelListAsync(persona, a => a.address, b => b.addr, b => b.type);
            await TryUpdateModelListAsync(persona, a => a.email, b => b.email, b => b.type);
            await TryUpdateModelListAsync(persona, a => a.personas_company, b => b.company_id, b => b.job_title);

            await TryUpdateModelAsync<PersonaModel>(
                persona,
                "",
                a => a.name,
                a => a.department,
                a => a.remarks
            );

            return await CommitModel(persona);
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
            var c = await _persona.GetPersona(ID);

            if (c == null)
            {
                return NotFound();
            }

            _persona.DelPersona(c);

            ViewData["ID"] = ID;

            await _tran.Commit();

            return Ok();
        }
    
    }
}
