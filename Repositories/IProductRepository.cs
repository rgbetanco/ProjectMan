using projectman.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectman.Repositories
{
    public interface IProductRepository
    {
        Task<Product> Create(Product u);
        bool Del(Product s);
        IQueryable<Product> Find(string keyword = null);
        Task<Product> Get(long ID, params string[] includeFields);
        IQueryable<ProductBrand> GetProductBrands();
    }
}