using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.Common;

namespace WebEnterprise.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext context;

        public CategoryController(ApplicationDbContext _context)
        {
            context = _context;
        }
        public IActionResult Index(string? keyword, int? pageIndex, int? pageSize)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            keyword = keyword ?? "";

            var cats = from c in context.Categories
                       select new Category
                       {
                           Id = c.Id,
                           Name = c.Name,
                           Description = c.Description,
                       };
            if (!String.IsNullOrEmpty(keyword))
            {
                cats = cats.Where(c => c.Name.Contains(keyword));
            }

            var paging = new CommonPaging(cats.Count(), pageIndex, pageSize);

            cats = cats.Skip((int)((paging.PageIndex - 1) * paging.PageSize)).Take((int)(paging.PageSize));
            
            cats.ToList();
            Notifiation();
            ViewBag.Paging = paging;
            return View(cats);
        }

   
        [HttpPost]
        public IActionResult AddCat(string name, string description)
        {
          
            if (ModelState.IsValid)
            {
                var cate = new Category();
                cate.Name = name;
                cate.Description = description;
                context.Categories.Add(cate);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return Content("Cannot Add");
        }

        public IActionResult EditCat(int id)
        {
            var cat = context.Categories.FirstOrDefault(c => c.Id == id);
            if(cat == null)
            {
                return RedirectToAction("Index");
            }
            Notifiation();
            return View(cat);
        }

        [HttpPost]
        public IActionResult EditCat(Category res)
        {
            if (ModelState.IsValid)
            {
                context.Entry(res).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(res);
        }

        public IActionResult DeleteCat(int id)
        {
            var cat = context.Categories.FirstOrDefault(c => c.Id == id);
            if(cat != null)
            {
                context.Remove(cat);
                context.SaveChanges();
            }
            return RedirectToAction("Index");
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
            ViewBag.Not = notes;
        }
    }
}
