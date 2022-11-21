using projectman.Models;
using System.Linq;
using System.Threading.Tasks;

namespace projectman.Repositories
{
    public interface IInvoiceRepository
    {
        IQueryable<ProjectIncomingPayment> GetIncomingInvoiceListByProject(long ID);
        Task<Invoice> CreateInvoice(Invoice u);
        IQueryable<Invoice> GetInvoices();
        IQueryable<Invoice> FindInvoices(string keyword = null);
        Task<Invoice> GetInvoice(long ID, params string[] includeFields);
        bool DelInvoice(Invoice s);
        bool UpdateIncomingPaymentItem(long incoming_payment_id, long invoice_id);
    }
}
