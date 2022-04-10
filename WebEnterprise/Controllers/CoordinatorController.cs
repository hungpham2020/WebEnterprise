using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.Common;
using WebEnterprise.Models.DTO;
using WebEnterprise.Repository.Interfaces;

namespace WebEnterprise.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CoordinatorController : Controller
    {
        private readonly ICoorRepo coorRepo;
        private readonly ApplicationDbContext context;

        public CoordinatorController(ICoorRepo _coorRepo, ApplicationDbContext _context)
        {
            coorRepo = _coorRepo;
            context = _context;
        }

        private List<Department> LoadDepartment(string form)
        {
            if (form == null)
            {
                return null;
            }
            else
            {
                var selected = form.Split(',').Select(id => Int32.Parse(id)).ToArray();
                return context.Departments.Where(c => selected.Contains(c.Id)).ToList();
            }
        }

        private void ViewDepartment()
        {
            ViewBag.Department = context.Departments.ToList();
        }

        private void ViewDepart()
        {
            var departments = context.Departments.Select(x => x).ToList();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
        }

        private void SelectedDepart(string id)
        {
            var selected = context.Users.Where(x => x.Id == id).Select(x => x.DepartmentId).FirstOrDefault();
            var departments = context.Departments.Select(x => x).ToList();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", selected);
        }

        public IActionResult Index(string? keyword, int? pageIndex, int? pageSize)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            keyword = keyword ?? "";

            var coordinators = coorRepo.GetAllCoor();
            if (!String.IsNullOrEmpty(keyword))
            {
                coordinators = coordinators.Where(c => c.FullName.Contains(keyword));
            }

            var paging = new CommonPaging(coordinators.Count(), pageIndex, pageSize);

            coordinators = coordinators.Skip((int)((paging.PageIndex - 1) * paging.PageSize)).Take((int)(paging.PageSize));

            coordinators.ToList();

            ViewBag.Paging = paging;
            ViewDepartment();
            return View(coordinators);
        }

        [HttpPost]
        public async Task<IActionResult> AddCoordinator(UserAddDTO user, IFormCollection f)
        {
            var depart = LoadDepartment(f["DepartmentIds"]);
            if (depart != null)
            {
                foreach (var d in depart)
                {
                    user.DepartId = d.Id;
                }
                if (ModelState.IsValid)
                {
                    var account = await coorRepo.AddCoor(user);
                    if (account != null)
                    {
                        TempData["message"] = $"Successfully Add new Coordinator {account.FullName}";
                        return RedirectToAction("Index");
                    }
                }
            }
            TempData["message"] = $"Cannot Add Coordinator";
            return RedirectToAction("Index");
        }

        public IActionResult EditCoordinator(string id)
        {
            var coordinator = coorRepo.GetEditCoor(id);

            if (coordinator == null)
            {
                return RedirectToAction("Index");
            }
            SelectedDepart(id);
            return View(coordinator);
        }

        [HttpPost]
        public IActionResult EditCoordinator(UserDTO res)
        {
            if (ModelState.IsValid)
            {
                var coordinator = coorRepo.EditCoor(res);
                if(coordinator != null) 
                {
                    TempData["message"] = $"Successfully Edit Coordinator {coordinator.FullName}";
                }
                return RedirectToAction("Index");
            }
            SelectedDepart(res.Id);
            return View(res);
        }

        public IActionResult DeleteCoordinator(string id)
        {
            var coordinator = coorRepo.DeleteCoor(id);
            if (!coordinator)
            {
                return RedirectToAction("Index");
            }
            TempData["message"] = $"Successfully Delete Coordinator";
            return RedirectToAction("Index");
        }
    }
}
