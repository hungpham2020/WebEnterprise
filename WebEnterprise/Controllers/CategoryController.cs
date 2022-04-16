using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.Common;
using WebEnterprise.Models.DTO;
using WebEnterprise.Repository.Interfaces;

namespace WebEnterprise.Controllers
{
    [Authorize(Roles = "Admin, Assurance, Coordinator")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepo categoryRepo;

        public CategoryController(ICategoryRepo _categoryRepo)
        {
            categoryRepo = _categoryRepo;
        }
        public IActionResult Index(string? keyword, int? pageIndex, int? pageSize)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            keyword = keyword ?? "";

            var cats = categoryRepo.GetAllCategory();
            if (!String.IsNullOrEmpty(keyword))
            {
                cats = cats.Where(c => c.Name.Contains(keyword));
            }

            var paging = new CommonPaging(cats.Count(), pageIndex, pageSize);

            cats = cats.Skip((int)((paging.PageIndex - 1) * paging.PageSize)).Take((int)(paging.PageSize));

            cats.ToList();
            ViewBag.Paging = paging;
            return View(cats);
        }

   
        [HttpPost]
        public IActionResult AddCat(CatDTO cat)
        {
            if (ModelState.IsValid)
            {
                var result = categoryRepo.AddCat(cat);
                if (result)
                {
                    TempData["message"] = $"Successfully Add new Category {cat.CatName}";
                    return RedirectToAction("Index");
                }
            }
            TempData["message"] = $"Cannot Add Category";
            return RedirectToAction("Index");
        }

        public IActionResult EditCat(int id)
        {
            var cat = categoryRepo.GetCatDetail(id);
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
                var result = categoryRepo.EditCat(res);
                if (result)
                {
                    TempData["message"] = $"Successfully Edit Category {res.Name}";
                    return RedirectToAction("Index");
                }
            }
            return View(res);
        }

        public IActionResult DeleteCat(int id)
        {
            var result = categoryRepo.DeleteCat(id);
            if (result)
            {
                TempData["message"] = $"Successfully Delete Category";
            }
            return RedirectToAction("Index");
        }
    }
}
