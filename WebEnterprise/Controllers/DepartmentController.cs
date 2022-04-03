using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.Common;

namespace WebEnterprise.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext context;

        public DepartmentController(ApplicationDbContext _context)
        {
            context = _context;
        }

        public IActionResult Index(string? keyword, int? pageIndex, int? pageSize)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            keyword = keyword ?? "";

            var departments = context.Departments.Select(x => x);
            if (!String.IsNullOrEmpty(keyword))
            {
                departments = departments.Where(c => c.Name.Contains(keyword));
            }

            var paging = new CommonPaging(departments.Count(), pageIndex, pageSize);

            departments = departments.Skip((int)((paging.PageIndex - 1) * paging.PageSize)).Take((int)(paging.PageSize));

            departments.ToList();

            ViewBag.Paging = paging;
            return View(departments);
        }

        [HttpPost]
        public IActionResult AddDepart(string name, string description)
        {
            if (ModelState.IsValid)
            {
                if(name != null && description != null)
                {
                    var department = new Department
                    {
                        Name = name,
                        Description = description
                    };
                    context.Add(department);
                    context.SaveChanges();
                    TempData["message"] = $"Successfully Add new Department {department.Name}";
                    return RedirectToAction("Index");
                }
            }
            TempData["message"] = $"Cannot Add Department";
            return RedirectToAction("Index");
        }

        public IActionResult EditDepartment(int id)
        {
            var depart = context.Departments.Where(x => x.Id == id).FirstOrDefault();
            if (depart == null)
            {
                return RedirectToAction("Index");
            }
            return View(depart);
        }

        [HttpPost]
        public IActionResult EditDepartment(Department req)
        {
            if (ModelState.IsValid)
            {
                var department = context.Departments.Where(x => x.Id == req.Id).FirstOrDefault();
                if (department == null)
                {
                    TempData["message"] = $"Department not Found";
                    return View(req);
                }
                department.Name = req.Name;
                department.Description = req.Description;
                context.SaveChanges();
                TempData["message"] = $"Successfully edit department with name {department.Name}";
                return RedirectToAction("Index");
            }
            return View(req);
        }

        public IActionResult DeleteDepartment(int id)
        {
            var department = context.Departments.FirstOrDefault(x => x.Id == id);
            if (department == null)
            {
                TempData["message"] = $"Department not Found";
                return RedirectToAction("Index");
            }
            context.Remove(department);
            context.SaveChanges();
            TempData["message"] = $"Successfully delete department";
            return RedirectToAction("Index");
        }
    }
}
