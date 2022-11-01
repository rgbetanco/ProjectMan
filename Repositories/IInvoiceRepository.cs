using repairman.Models;
using System.Linq;
using System.Threading.Tasks;

namespace repairman.Repositories
{
    public interface IInvoiceRepository
    {
        IQueryable<IncomingPaymentModel> GetIncomingInvoiceListByProject(long ID);
        Task<InvoiceModel> CreateInvoice(InvoiceModel u);
        IQueryable<InvoiceModel> GetInvoices();
        IQueryable<InvoiceModel> FindInvoices(string keyword = null);
        Task<InvoiceModel> GetInvoice(long ID, params string[] includeFields);
        bool DelInvoice(InvoiceModel s);
        bool UpdateIncomingPaymentItem(long incoming_payment_id, long invoice_id);
    }
}
