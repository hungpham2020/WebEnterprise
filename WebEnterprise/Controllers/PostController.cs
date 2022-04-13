using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security.Claims;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.Common;
using WebEnterprise.Models.DTO;
using WebEnterprise.Repository.Interfaces;

namespace WebEnterprise.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IPostRepo postRepo;

        public PostController(ApplicationDbContext _context, IPostRepo _postRepo)
        {
            context = _context;
            postRepo = _postRepo;
        }

        [Authorize(Roles = "Admin")]
        private void ViewCat()
        {
            ViewBag.Cat = context.Categories.ToList();
        }
        private void ViewCate()
        {
            var cats = context.Categories.Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
            }).ToList();

            ViewBag.Cat = new SelectList(cats, "Id", "Name");
        }
        [Authorize(Roles = "Admin")]
        private List<Category> LoadCat(string form)
        {
            if (form != null)
            {
                var selected = form.Split(',').Select(id => Int32.Parse(id)).ToArray();
                return context.Categories.Where(c => selected.Contains(c.Id)).ToList();
            }
            else
            {
                return context.Categories.Where(c => c.Id == 0).ToList();
            }
        }

        [Authorize(Roles = "Admin")]
        private void SelectedCat(int id)
        {
            var selectedCat = context.Categories.FirstOrDefault(c => c.Id == id);
            var cats = context.Categories.Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
            }).ToList();
            ViewBag.SelectedCat = new SelectList(cats, selectedCat);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index(string? keyword, int? pageIndex, int? pageSize)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            keyword = keyword ?? "";

            var posts = postRepo.GetAllPost();
            if (!String.IsNullOrEmpty(keyword))
            {
                posts = posts.Where(p => p.Title.Contains(keyword));
            }

            var paging = new CommonPaging(posts.Count(), pageIndex, pageSize);

            posts = posts.Skip((int)((paging.PageIndex - 1) * paging.PageSize)).Take((int)(paging.PageSize));

            posts.ToList();

            ViewBag.Paging = paging;
            Notifiation();
            ViewCat();
            return View(posts);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddPost(PostDTO res, IFormCollection f)
        {
            if (res.CatId != 0)
            {
                var cat = LoadCat(f["CatIds"]);
                foreach (var c in cat)
                {
                    res.CatId = c.Id;
                }
                if (ModelState.IsValid)
                {
                    var result = postRepo.AddPost(res, User.FindFirstValue(ClaimTypes.NameIdentifier));

                    if (result)
                    {
                        TempData["message"] = $"Successfully Add new Post {res.Title}";
                        return RedirectToAction("Index");
                    }
                }
            }
            TempData["message"] = $"Cannot Add this Post";
            return RedirectToAction("Index");

        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditPost(int id)
        {
            var post = postRepo.GetEditPost(id);
            if (post == null)
            {
                return RedirectToAction("Index");
            }
            SelectedCat(id);
            return View(post);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditPost(PostDTO res)
        {
            if (ModelState.IsValid)
            {
                var result = postRepo.EditPost(res);
                if (result)
                {
                    TempData["message"] = $"Successfully Edit Post {res.Title}";
                    return RedirectToAction("Index");
                }
            }
            SelectedCat(res.Id);
            return View(res);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeletePost(int id)
        {
            var result = postRepo.DeletePost(id);
            if(result)
            {
                TempData["message"] = $"Successfully Delete Post";
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, Assurance")]
        public FileResult Export()
        {
            var posts = postRepo.GetAllPost().ToList();

            FileInfo template = new FileInfo(@"wwwroot/template/Post.xlsx");

            using(ExcelPackage excel = new ExcelPackage(template))
            {
                ExcelWorksheet worksheet = excel.Workbook.Worksheets[0];
                var row = 2;
                foreach(var post in posts)
                {
                    worksheet.Cells[row, 1].Value = post.Title;
                    worksheet.Cells[row, 2].Value = post.Description;
                    worksheet.Cells[row, 3].Value = post.CatName;
                    worksheet.Cells[row, 4].Value = post.AuthorName;
                    worksheet.Cells[row, 5].Value = post.Like;
                    worksheet.Cells[row, 6].Value = post.DisLike;

                    row++;
                }
                string contentType = "";
                new FileExtensionContentTypeProvider().TryGetContentType(template.FullName, out contentType);
                return File(excel.GetAsByteArray(), contentType);
            }
        }
        
        private void Notifiation()
        {
            var notes = (from n in context.Notifications
                         where n.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier)
                         select new Notification
                         {
                             description = n.description,
                             date = n.date
                         }).ToList();
            notes.OrderByDescending(c => c.date).Take(5).ToList();
            ViewBag.Not = notes;
        }
    }
}
