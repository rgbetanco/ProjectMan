using projectman.Data;
using projectman.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace projectman.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DBContext _context;

        public ProductRepository(DBContext context)
        {
            _context = context;
        }

        public IQueryable<Product> Find(string keyword = null)
        {
            var result = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.desc.Contains(keyword));
            }

            return result;
        }

        public async Task<Product> Create(Product u)
        {
            await _context.Products.AddAsync(u);

            return u;
        }

        public bool Del(Product s)
        {
            _context.Products.Remove(s);
            return true;
        }

        public async Task<Product> Get(long ID, params string[] includeFields)
        {
            var q = _context.Products.AsQueryable();

            if (includeFields != null)
            {
                foreach (var field in includeFields)
                    q = q.Include(field);
            }

            var a = await q.FirstOrDefaultAsync(u => u.ID == ID);
            return a;
        }

        public IQueryable<ProductBrand> GetProductBrands()
        {
            var q = _context.ProductBrands.AsQueryable();

            return q;
        }
    }
}
