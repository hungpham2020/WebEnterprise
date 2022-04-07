using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.Common;
using WebEnterprise.Models.DTO;
using WebEnterprise.Repository.Interfaces;

namespace WebEnterprise.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepo departmentRepo;

        public DepartmentController(IDepartmentRepo _departmentRepo)
        {
            departmentRepo = _departmentRepo;
        }

        public IActionResult Index(string? keyword, int? pageIndex, int? pageSize)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            keyword = keyword ?? "";

            var departments = departmentRepo.GetAllDepartment();
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
        public IActionResult AddDepart(DepartDTO depart)
        {
            if (ModelState.IsValid)
            {
                var result = departmentRepo.AddDepart(depart);
                if(result)
                {
                    TempData["message"] = $"Successfully Add new Department {depart.DepartName}";
                    return RedirectToAction("Index");
                }
            }
            TempData["message"] = $"Cannot Add Department";
            return RedirectToAction("Index");
        }

        public IActionResult EditDepartment(int id)
        {
            var depart = departmentRepo.GetDepartDetail(id);
            if (depart == null)
            {
                return RedirectToAction("Index");
            }
            return View(depart);
        }

        [HttpPost]
        public IActionResult EditDepartment(DepartDTO req)
        {
            if (ModelState.IsValid)
            {
                var result = departmentRepo.EditDepart(req);
                if (!result)
                {
                    TempData["message"] = $"Department not Found";
                    return View(req);
                }
                TempData["message"] = $"Successfully edit department with name {req.DepartName}";
                return RedirectToAction("Index");
            }
            return View(req);
        }

        public IActionResult DeleteDepartment(int id)
        {
            var result = departmentRepo.DeleteDepart(id);
            if (!result)
            {
                TempData["message"] = $"Cannot Delete Department";
                return RedirectToAction("Index");
            }
            TempData["message"] = $"Successfully delete department";
            return RedirectToAction("Index");
        }
    }
}
