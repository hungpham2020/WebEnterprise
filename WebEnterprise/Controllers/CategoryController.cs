using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEnterprise.Data;
using WebEnterprise.Models;

namespace WebEnterprise.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext context;

        public CategoryController(ApplicationDbContext _context)
        {
            context = _context;
        }
        public IActionResult Index()
        {
            var cats = (from c in context.Categories
                        select new Category
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description,
                        }).ToList();
            return View(cats);
        }

        public IActionResult AddCat()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCat(Category res)
        {
            if (ModelState.IsValid)
            {
                context.Categories.Add(res);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(res);
        }

        public IActionResult EditCat(int id)
        {
            var cat = context.Categories.FirstOrDefault(c => c.Id == id);
            if(cat == null)
            {
                return RedirectToAction("Index");
            }
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
    }
}
