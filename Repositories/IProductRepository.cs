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
        IQueryable<Product> Find(string keyword = null, int brand = -1, string category = null);
        Task<Product> Get(long ID, params string[] includeFields);
        IQueryable<ProductBrand> GetProductBrands();
        Task<ProductBrand> Create(ProductBrand t);
        Task<ProductBrand> GetProductBrandAsync(string t);
        void DelProductBrandUnsafe(string t);
    }
}