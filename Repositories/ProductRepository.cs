using projectman.Data;
using projectman.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace projectman.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DBContext _context;

        public ProductRepository(DBContext context)
        {
            _context = context;
        }

        public IQueryable<Product> Find(string keyword = null, int brand = -1, string category = null)
        {
            var result = _context.Products.AsQueryable();
            result = result.Include("brand");

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.desc.Contains(keyword) || u.model_name.Contains(keyword));
            }

            if (brand > -1)
            {
                result = result.Where(u => u.brand_id == brand);
            }

            if (!string.IsNullOrWhiteSpace(category) && category != "-1")
            {
                //result = result.Where(u => u.category == (ProductCategory)Enum.Parse(typeof(ProductCategory), category));
                result = result.Where(u => u.category == (ProductCategory)Enum.Parse(typeof(ProductCategory), category));
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
        public async Task<ProductBrand> Create(ProductBrand t)
        {
            await _context.ProductBrands.AddAsync(t);
            return t;
        }

        public async Task<ProductBrand> GetProductBrandAsync(string t)
        {
            var q = GetProductBrands();
            var s = await q.FirstOrDefaultAsync(u => u.ID == long.Parse(t));
            return s;
        }

        public void DelProductBrandUnsafe(string t)
        {
            var s = new ProductBrand { ID = long.Parse(t) };

            _context.ProductBrands.Attach(s);
            _context.ProductBrands.Remove(s);
        }

    }
}
