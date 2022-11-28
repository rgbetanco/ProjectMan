using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projectman.Areas.Man.Controllers;
using projectman.Models;
using projectman.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using CSHelper.Extensions;
using System.Collections;
using System.Collections.Generic;
using CSHelper.Authorization;

namespace projectman.Areas.man.Controllers
{
    [Area("man")]
    [Authorize(AuthenticationSchemes = ManDefaults.AuthenticationScheme)]
    public class ProductController : BaseController
    {
        private readonly IProductRepository _prod;

        public ProductController(ITransaction tran, ILookupRepository lookup, IProductRepository prod)
           : base(tran, lookup)
        {
            _prod = prod;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Query(QueryVM request)
        {
            var result = _prod.Find(request.search);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                code = r.code,
                name = r.name,
                brand_name = r.brand.name,
                model_name = r.model_name,
                category = r.category.GetDisplayName()
            });
        }

        [HttpGet]
        public IActionResult New()
        {
            ViewData["brands"] = GetProductBrandList(0);
            return View();
        }

        private IEnumerable<SelectListItem> GetCategoryList()
        {
            var toReturn = new List<SelectListItem> { new SelectListItem { Value = "-1", Text = "全部", Selected = true } };
            var r = CSHelper.Extensions.RenderingExtension.GetSelectList<ProductCategory>();
            foreach(var item in r)
            {
                toReturn.Add(item);
            }
            return toReturn;
        }

        private IEnumerable<SelectListItem> GetProductBrandList(long? ID = null)
        {
            var toReturn = new List<SelectListItem> { new SelectListItem { Value = "-1", Text = "全部", Selected = ID == null } };
            
            var r = _prod.GetProductBrands().AsNoTracking();
            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name, Selected = ID == m.ID });
            foreach(var item in result)
            {
                toReturn.Add(item);
            }
            return toReturn;
            
        }

        // new category - save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("New")]
        public async Task<IActionResult> NewPost()
        {
            var m = new Product();

            await TryUpdateModelAsync<Product>(
                m,
                "",
                a => a.code,
                a => a.brand_id,
                a => a.model_name,
                a => a.name,
                a => a.desc,
                a => a.category
            );

            m = await _prod.Create(m);

            var result = await CommitModel(m);

            return result;
        }

        public async Task<IActionResult> View(long ID)
        {
            var c = await _prod.Get(ID, "brand");
            ViewData["brands"] = GetProductBrandList(c.brand_id);
            return View(c);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("View")]
        public async Task<IActionResult> ViewPost(long ID)
        {
            var r = await _prod.Get(ID, "brands");
            if (r == null)
            {
                return NotFound();
            }

            await this.TryUpdateModelAsync<Product>(
                r,
                "",
                a => a.name,
                a => a.code,
                a => a.category,
                a => a.brand_id,
                a => a.desc,
                a => a.model_name
            );

            return await CommitModel(r);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] long ID)
        {
            var c = await _prod.Get(ID);

            if (c == null)
            {
                return NotFound();
            }

            _prod.Del(c);

            ViewData["ID"] = ID;

            await _tran.Commit();

            return Ok();
        }

        // picker
        public IActionResult Picker()
        {
            ViewData["brands"] = GetProductBrandList(0);
            ViewData["categories"] = GetCategoryList();

            return PartialView("_PickerPopup");
        }

        public async Task<IActionResult> PickerQuery(QueryVM request)
        {
            var result = _prod.Find(request.search, (int)request.brand, request.category);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                id = r.ID,
                code = r.code,
                name = r.name,
                brand_name = r.brand.name,
                model_name = r.model_name,
                category = r.category.GetDisplayName()
            }, true);
        }

        public async Task<IActionResult> BrandSetting()
        {
            return View(await _prod.GetProductBrands().ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("BrandSetting")]
        [AuthorizeRole(UserPermission.Edit | UserPermission.Add)]
        public async Task<IActionResult> BrandSettingPost()
        {
            await this.TryUpdateTableModelAsync<ProductBrand, string>(
                "Brand",   // data-form-table-group name
                async t =>
                {
                    await _prod.Create(t);
                    return true;
                },
                t =>
                {
                    _prod.DelProductBrandUnsafe(t);
                    return Task.FromResult(true);
                },
                async t => await _prod.GetProductBrandAsync(t)
            );

            var result = await CommitModel(null);

            return result;
        }

    }
}
