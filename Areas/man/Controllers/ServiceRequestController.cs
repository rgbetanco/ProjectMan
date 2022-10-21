using repairman.Models;
using repairman.ModelOptions;
using repairman.Repositories;
using CSHelper.Authorization;
using CSHelper.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using CSHelper.Models;
using CSHelper.Extensions.File;

namespace repairman.Areas.Man.Controllers
{
    [Area("man")]
    [Authorize(AuthenticationSchemes = ManDefaults.AuthenticationScheme)]
    public class ServiceRequestController : BaseController
    {
        private readonly IServiceRequestRepository _sr;

        public ServiceRequestController(ITransaction tran, ILookupRepository lookup, IServiceRequestRepository sr) 
            : base(tran,lookup)
        {
            _sr = sr;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CatSetting()
        {
            return View();
        }

        // API: categories list
        public async Task<IActionResult> CatQuery(QueryVM request)
        {
            var result = _sr.FindCats(request.search);

            return await GetTableReplyAsync(result, request, null, r => new
            {
                ID = r.ID,
                name = r.name,
                desc = r.desc
            });
        }

        // new message
        [HttpGet]
        public IActionResult CatNew()
        {
            return View();
        }

        // delete
        public IActionResult ConfirmDeleteCat(long ID)
        {
            ViewData["ID"] = ID;
            return PartialView("_ConfirmDeleteCatPopup");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCat([FromForm] long ID)
        {
            var c = await _sr.GetCat(ID);

            if (c== null)
            {
                return NotFound();
            }

            _sr.DelCat(c);

            ViewData["ID"] = ID;

            await _tran.Commit();

            return Ok();
        }


        public async Task<IActionResult> DeptSetting()
        {
            var list = await _sr.GetDepts().ToListAsync();

            return View(list);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeptSetting")]
        public async Task<IActionResult> DeptSettingPost()
        {
            await this.TryUpdateTableModelAsync<Dept, long>("Dept",
                async t =>
                {
                    await _sr.Create(t);
                    return true;
                },
                t =>
                {
                    _sr.DelDeptUnsafe(t);
                    return Task.FromResult(true);
                },
                async t => await _sr.GetDept(t)
            );

            var result = await CommitModel(null);

            return result;
        }

        // new category - save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("CatNew")]
        public async Task<IActionResult> CatNewPost([FromServices] IWebHostEnvironment env)
        {
            var m = new ServiceRequestCat();

            await TryUpdateModelAsync<ServiceRequestCat>(
                m,
                "",
                a => a.name,
                a => a.desc
            );

            m.subcats = new List<ServiceRequestSubCat>();

            await this.TryUpdateTableModelAsync<ServiceRequestSubCat, long>("subcats",
                t =>
                {
                    m.subcats.Add(t);
                    return Task.FromResult(true);
                },
                t =>
                {
                    m.subcats.RemoveAll(x => x.ID == t); 
                    return Task.FromResult(true);
                },
                t => {
                    var r = m.subcats.FirstOrDefault(x => x.ID == t);
                    return Task.FromResult(r);
                }
            );


            m = await _sr.Create(m);

            var result = await CommitModel(m);

            return result;
        }

        // API: delete message category
        [ActionName("DelCat")]
        [HttpPost]
        [AuthorizeRole(UserPermission.Edit)]
        public async Task<IActionResult> DelCat(long ID)
        {
            var s = _sr.DelCatUnsafe(ID);

            if (!s)
            {
                return NotFound();
            }

            await _tran.Commit();

            return Ok();
        }


        // view categories
        [AuthorizeRole(UserPermission.Edit)]
        public async Task<IActionResult> CatView(long ID)
        {
            var c = await _sr.GetCat(ID, "subcats");

            if (c == null)
            {
                return NotFound();
            }

            return View(c);
        }

        // edit categories
        [HttpPost]
        [ActionName("CatView")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(UserPermission.Edit)]
        public async Task<IActionResult> CatViewPost(long ID)
        {
            var m = await _sr.GetCat(ID, "subcats");

            if (m == null)
            {
                return NotFound();
            }

            await TryUpdateModelAsync<ServiceRequestCat>(
                m,
                "",
                a => a.name,
                a => a.desc
            );

            await this.TryUpdateTableModelAsync<ServiceRequestSubCat, long>("subcats",
                t =>
                {
                    m.subcats.Add(t);
                    return Task.FromResult(true);
                },
                t =>
                {
                    m.subcats.RemoveAll(x => x.ID == t);
                    return Task.FromResult(true);
                },
                t => {
                    var r = m.subcats.FirstOrDefault(x => x.ID == t);
                    return Task.FromResult(r);
                }
            );

            var result = await CommitModel(m);

            return result;
        }



        // API: query service request list
        public async Task<IActionResult> Query(QueryVM request, ServiceRequestStatus? status)
        {
            var result = _sr.Find(request.search);
            result.Include(r => r.dept);
            result.Include(r => r.sub_cat);
            result.Include(r => r.sub_cat.cat);

            if (status != null)
            {
                result = result.Where(p => p.status == status.Value);
            }

            return await GetTableReplyAsync(result, request, null, r => new
            {
                ID = r.ID,
                date = r.date,
                moddate = r.modify_date,
                name = r.name,
                dept = r.dept.name,
                desc = (r.sub_cat != null) ? (r.sub_cat.name + " - " + r.sub_cat.cat.name) : "",
                status = r.status.GetDisplayName()
            });
        }

        // view request
        public async Task<IActionResult> View(long ID)
        {
            var m = await _sr.Get(ID, "member", "sub_cat", "sub_cat.cat", "dept", "files", "pics", "replies", "replies.files", "replies.pics", "replies.user");

            if (m == null)
            {
                return NotFound();
            }

            var l = GetCatList();
            ViewData["cat"] = l;
            ViewData["subcat"] = (l != null && l.Any()) ? GetSubCatList(m.sub_cat.cat_id) : new List<SelectListItem>();
            ViewData["dept"] = GetDeptList();
            ViewData["allow_edit"] = UserHasPermission(UserPermission.Edit);

            return View(m);
        }

        public IActionResult New()
        {
            var l= GetCatList();
            ViewData["cat"] = l;
            ViewData["subcat"] = (l!=null && l.Any())?GetSubCatList( long.Parse(l.First().Value) ):new List<SelectListItem>();
            ViewData["dept"] = GetDeptList();

            var a = new ServiceRequest();
            a.date = DateTime.UtcNow;

            return View(a);
        }

        // new message - save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("New")]
        public async Task<IActionResult> NewPost([FromServices] IWebHostEnvironment env)
        {
            var m = new ServiceRequest();

            m.date = System.DateTime.UtcNow;

            await this.TryUpdateModelListAsync(m, a => a.files, b => b.file);
            await this.TryUpdateModelListAsync(m, a => a.pics, b => b.file);
            await TryUpdateModelAsync<ServiceRequest>(
                m,
                "",
                a => a.desc,
                a => a.sub_cat_id,
                a => a.dept_id,
                a => a.name,
                a => a.email,
                a => a.phone,
                a => a.date
            );

            m.status = ServiceRequestStatus.Pending;

            List<string> paths = null;
            try
            {
                this.processUploadedFiles(m, m => m.files, new FileModelOptions()
                {
                    StoragePath = Path.Join(env.WebRootPath, "dl", "f", "p")
                }, ref paths);

                this.processUploadedFiles(m, m => m.pics, new PicFileModelOptions()
                {
                    StoragePath = Path.Join(env.ContentRootPath, "AppData", "uploads", "request"),
                    OutputThumbnailPath = Path.Join(env.WebRootPath, "dl", "img", "a"),
                    OutputFullImagePath = Path.Join(env.WebRootPath, "dl", "img", "a")
                }, ref paths);

                m = await _sr.Create(m);
            }
            catch(SaveProcessUploadError)
            {
                ModelState.AddModelError("file", "無法儲存上傳的檔案");
            }
            catch (ImageProcessUploadError )
            {
                ModelState.AddModelError("file", "無法儲存縮圖檔");
            }
            catch( MissingProcessUploadError )
            {
                ModelState.AddModelError("file", "上傳檔案必填");
            }
            catch (ProcessUploadError)
            {
                ModelState.AddModelError("file", "無法讀取上傳的圖檔");
            }

            var result = await CommitModel(m, paths);

            return result;
        }


        // edit service request
        [HttpPost]
        [ActionName("View")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long ID)
        {
            var m = await _sr.Get(ID);

            if (m == null)
            {
                return NotFound();
            }

            if(UserHasPermission(UserPermission.Edit) )
            {
                if (await TryUpdateModelAsync(
                    m,
                    "",
                    a => a.desc,
                    a => a.sub_cat_id,
                    a => a.dept_id,
                    a => a.name,
                    a => a.email,
                    a => a.phone
                ))
                {
                }

            }
            else
            {
                if (await TryUpdateModelAsync(
                    m,
                    "",
                    a => a.sub_cat_id
                ))
                {
                }

            }

            return await CommitModel(m);
        }

        public IActionResult ConfirmDel(long ID)
        {
            ViewData["ID"] = ID;
            return PartialView("_ConfirmDeletePopup");
        }

        // API: delete service request
        [ActionName("Del")]
        [HttpPost]
        [AuthorizeRole(UserPermission.Edit)]
        public async Task<IActionResult> Del(long ID)
        {
            var s = _sr.DelServiceRequest(ID);

            if (!s)
            {
                return NotFound();
            }

            await _tran.Commit();

            return Ok();
        }


        // new reply
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("NewReply")]
        public async Task<IActionResult> NewReplyPost([FromServices] IWebHostEnvironment env, long ID)
        {
            var m = new ServiceRequestReply();

            m.request_id = ID;
            m.user_id = GetCurrentUserID();
            m.modify_date = DateTime.UtcNow;
            m.date = DateTime.UtcNow;

            await this.TryUpdateModelListAsync(m, a => a.files, b => b.file);
            await this.TryUpdateModelListAsync(m, a => a.pics, b => b.file);

            await TryUpdateModelAsync<ServiceRequestReply>(
                m,
                "",
                a => a.title,
                a => a.desc,
                a => a.status,
                a => a.date
            );

            List<string> paths = null;
            try
            {
                this.processUploadedFiles(m, m => m.files, new FileModelOptions()
                {
                    StoragePath = Path.Join(env.WebRootPath, "dl", "f", "p")
                }, ref paths);

                this.processUploadedFiles(m, m => m.pics, new PicFileModelOptions()
                {
                    StoragePath = Path.Join(env.ContentRootPath, "AppData", "uploads", "reply"),
                    OutputThumbnailPath = Path.Join(env.WebRootPath, "dl", "img", "a2"),
                    OutputFullImagePath = Path.Join(env.WebRootPath, "dl", "img", "a2")
                }, ref paths);

                m = await _sr.Create(m);

                var r = await _sr.Get(ID);
                if (r != null)
                {
                    r.status = m.status;
                    r.modify_date = DateTime.UtcNow;
                }
            }
            catch (SaveProcessUploadError)
            {
                ModelState.AddModelError("file", "無法儲存上傳的檔案");
            }
            catch (ImageProcessUploadError)
            {
                ModelState.AddModelError("file", "無法儲存縮圖檔");
            }
            catch (MissingProcessUploadError)
            {
                ModelState.AddModelError("file", "上傳檔案必填");
            }
            catch (ProcessUploadError)
            {
                ModelState.AddModelError("file", "無法讀取上傳的圖檔");
            }

            var result = await CommitModel(m, paths);

            return result;
        }

        // file download
        [HttpGet]
        [ActionName("dl")]
        public async Task<IActionResult> DownloadFile([FromServices] IWebHostEnvironment env, long ?ID, long ?replyID, long fileID)
        {
            string filename_orig = null;
            string filepath_save = null;

            if (replyID != null)
            {
                var r = await _sr.GetReplyFile(fileID);

                if (r == null)
                    return NotFound();

                if (r.reply_id != replyID.Value)
                    return NotFound();

                if (r.filename == null || r.source_file == null)
                    return NotFound();

                filename_orig = r.filename;
                string filename_save = r.source_file;

                // prepare the local file path
                filepath_save = Path.Join("~", "dl", "f", "p", filename_save);
            }
            else if ( ID!=null )
            {
                var r = await _sr.GetFile(fileID);

                if (r == null)
                    return NotFound();

                if (r.request_id != ID.Value)
                    return NotFound();

                if (r.filename == null || r.source_file == null)
                    return NotFound();

                filename_orig = r.filename;
                string filename_save = r.source_file;

                // prepare the local file path
                filepath_save = Path.Join("~", "dl", "f", "p", filename_save);
            }

            if ( filename_orig==null )
                return NotFound();

            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(filepath_save, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return File(filepath_save, contentType, filename_orig );
        }


        public async Task<IActionResult> QueryCountByCat(QueryVM request)
        {
            var startDate = DateTime.Now.AddYears(-1);


            var result = _sr.Find()
                .Where(a => a.date >= startDate)
                .GroupBy(a => a.sub_cat_id, (ta, tb) => new
                {
                    sub_cat_id = ta,
                    count = tb.Count()
                }).Join(
                    _sr.GetSubCats(null), 
                    q => q.sub_cat_id,
                    a => a.ID,
                    (q,a) =>
                    new {
                        cat = a.cat.name + " - " + a.name,
                        count = q.count
                    });

            return await GetTableReplyAsync(result, request, null, r => new
            {
                cat = r.cat,
                count = r.count
            });
        }

        public async Task<IActionResult> QueryCountByDept(QueryVM request)
        {
            var startDate = DateTime.Now.AddYears(-1);

            var result = _sr.Find()
                .Where(a => a.date >= startDate)
                .GroupBy(a => a.dept_id, (ta, tb) => new
                {
                    dept_id = ta,
                    count = tb.Count()
                }).Join(
                    _sr.GetDepts(),
                    q => q.dept_id,
                    a => a.ID,
                    (q, a) =>
                    new {
                        dept = a.name,
                        count = q.count
                    });

            return await GetTableReplyAsync(result, request, null, r => new
            {
                dept = r.dept,
                count = r.count
            });
        }





        // API: list categories
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ListSubCatsForType(long? type)
        {
            return Json(GetSubCatList(type));
        }

        protected IEnumerable<SelectListItem> GetCatList()
        {
            var r = _sr.FindCats()
                .AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name });

            return result;
        }

        protected IEnumerable<SelectListItem> GetSubCatList(long? type)
        {
            var r = _sr.GetSubCats(type)
                .AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name });

            return result;
        }
        protected IEnumerable<SelectListItem> GetDeptList()
        {
            var r = _sr.GetDepts()
                .AsNoTracking();

            var result = r.Select(m => new SelectListItem { Value = m.ID.ToString(), Text = m.name });

            return result;
        }

    }
}
