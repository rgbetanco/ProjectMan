﻿using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using repairman.Data;
using repairman.Models;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;

namespace repairman.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DBContext _context;
        public InvoiceRepository(DBContext context)
        {
            _context = context;
        }
        public IQueryable<IncomingPaymentModel> GetIncomingInvoiceListByProject(long ID)
        {
            return _context.IncomingPayments.AsQueryable().Where(m => m.project_id == ID);
        }
        public async Task<InvoiceModel> CreateInvoice(InvoiceModel u)
        {
            await _context.Invoices.AddAsync(u);
            return u;
        }
        public IQueryable<InvoiceModel> GetInvoices()
        {
            return _context.Invoices;
        }
        public IQueryable<InvoiceModel> FindInvoices(string keyword = null)
        {
            var result = _context.Invoices.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.number.Contains(keyword));
            }
            return result;
        }
        public async Task<InvoiceModel> GetInvoice(long ID, params string[] includeFields)
        {
            var q = _context.Invoices.AsQueryable();
            
            if (includeFields != null)
            {
                foreach (var field in includeFields)
                    q = q.Include(field);
            }

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);
            return a;
        }
        public bool DelInvoice(InvoiceModel s)
        {
            _context.Invoices.Remove(s);
            return true;
        }

        public bool UpdateIncomingPaymentItem(long incoming_payment_id, long invoice_id)
        {
            var invoice = _context.Invoices.AsQueryable().Where(m => m.ID == invoice_id).FirstOrDefault();
            var payment = _context.IncomingPayments.AsQueryable().Where(m => m.ID == incoming_payment_id).FirstOrDefault();

            payment.invoice = invoice.number;
            
            return true;
        }
    }
}