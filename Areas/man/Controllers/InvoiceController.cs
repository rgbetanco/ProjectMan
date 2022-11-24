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
    public class InvoiceController : BaseController
    {
        public IInvoiceRepository _inv;
        public InvoiceController(ITransaction tran, ILookupRepository lookup, IInvoiceRepository inv) : base(tran, lookup)
        {
            _inv = inv;
        }

        public IActionResult Index()
        {
            var i = _inv.GetInvoices();
            return View(i);
        }
        [HttpGet]
        public async Task<IActionResult> View(long ID)
        {
            Invoice invoice = await _inv.GetInvoice(ID, "items","items.incoming_payment");
            return View(invoice);
        }
        [HttpPost]
        public async Task<IActionResult> Update(long ID)
        {
            Invoice invoice = await _inv.GetInvoice(ID, "items");
            if (invoice == null)
            {
                return NotFound();
            }

            await this.TryUpdateModelListAsync(invoice, a => a.items, b => b.incoming_payment_id, b => b.amount);

            foreach(var item in invoice.items)
            {
                UpdatePaymentItem((long)item.incoming_payment_id, (long)item.invoice_id);
            }

            await TryUpdateModelAsync<Invoice>(
                invoice,
                "",
                a => a.issue_date,
                a => a.number,
                a => a.total_amount
            );

            return await CommitModel(invoice);
        }

        private bool UpdatePaymentItem(long incoming_payment_id, long invoice_id)
        {
            _inv.UpdateIncomingPaymentItem(incoming_payment_id, invoice_id);
            return true;
        }

        [HttpGet]
        public IActionResult New()
        {
            var a = new Invoice();
            a.issue_date = DateTime.UtcNow;
            
            return View(a);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            var m = new Invoice();
            await TryUpdateModelAsync<Invoice>(
                m,
                "",
                a => a.issue_date,
                a => a.number,
                a => a.total_amount
            );
            await this.TryUpdateModelListAsync(m, a => a.items, b => b.incoming_payment_id, b => b.amount);
            m = await _inv.CreateInvoice(m);
            var result = await CommitModel(m);
            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ListPaymentForProject(long ID)
        {
            return Json(GetIncomingPaymentListByProject(ID));
        }
        private IEnumerable<SelectListItem> GetIncomingPaymentListByProject(long ID)
        {
            var r = _inv.GetIncomingInvoiceListByProject(ID).Select(n => new SelectListItem { Value = n.ID.ToString(), Text = n.item + " - " + n.amount });
            if (!r.Any())
            {
                r.Append(new SelectListItem { Value = "所有", Text = "所有" });
            }
            return r;
        }
        public async Task<IActionResult> InvoiceQuery(QueryVM request)
        {
            var result = _inv.FindInvoices(request.search);
            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                number = r.number,
                issue_date = r.issue_date,
                amount = r.total_amount
            });
        }
        public IActionResult ConfirmDeleteInvoice(long ID)
        {
            ViewData["ID"] = ID;
            return PartialView("_ConfirmDeleteInvoicePopup");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInvoice([FromForm] long ID)
        {
            var c = await _inv.GetInvoice(ID);
            if(c == null)
            {
                return NotFound();
            }
            _inv.DelInvoice(c);
            ViewData["ID"] = ID;
            await _tran.Commit();
            return Ok();
        } 
    }
}
