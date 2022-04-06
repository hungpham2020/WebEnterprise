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
    public class AssuranceController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IAssuranceRepo assuranceRepo;
        public AssuranceController(IAssuranceRepo _assuranceRepo, ApplicationDbContext _context)
        {
            context = _context;
            assuranceRepo = _assuranceRepo;
        }

        private List<Department> LoadDepartment(string form)
        {
            if(form == null)
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

            var assurances = assuranceRepo.GetAllAssurances();

            if (!String.IsNullOrEmpty(keyword))
            {
                assurances = assurances.Where(a => a.FullName.Contains(keyword));
            }

            var paging = new CommonPaging(assurances.Count(), pageIndex, pageSize);

            assurances = assurances.Skip((int)((pageIndex - 1) * pageSize)).Take((int)(pageSize));

            assurances.ToList();
            Notifiation();
            ViewBag.Paging = paging;
            ViewDepartment();
            return View(assurances);
        }


        [HttpPost]
        public async Task<IActionResult>AddAssurance(UserAddDTO user, IFormCollection f)
        {
            var depart = LoadDepartment(f["DepartmentIds"]);
            foreach (var d in depart)
            {
                user.DepartId = d.Id;
            }
            if (depart != null)
            {
                if (ModelState.IsValid)
                {
                    var account = await assuranceRepo.AddAssurance(user);
                    if (account.Id != null)
                    {
                        TempData["message"] = $"Successfully Add new Assurance {account.FullName}";
                        return RedirectToAction("Index");
                    }
                }
            }
            TempData["message"] = $"Cannot Add Assurance, account is not suitable!";
            return RedirectToAction("Index");
        }

        public IActionResult EditAssurance(string id)
        {
            var assurance = assuranceRepo.GetEditAssurance(id);
            if (assurance == null)
            {
                return RedirectToAction("Index");
            }
            Notifiation();
            SelectedDepart(id);
            return View(assurance);
        }

        [HttpPost]
        public IActionResult EditAssurance(UserDTO res)
        {

            if (ModelState.IsValid)
            {
                var assurance = assuranceRepo.EditAssurance(res);
                if(assurance != null)
                {
                    TempData["message"] = $"Successfully Edit Assurance {assurance.FullName}";
                }
                return RedirectToAction("Index");
            }
            SelectedDepart(res.Id);
            return View(res);
        }

        public IActionResult DeleteAssurance(string id)
        {
            var assurance = assuranceRepo.DeleteAssurance(id);
            if (!assurance)
            {
                return RedirectToAction("Index");
            };
            TempData["message"] = $"Successfully Delete Assurance";
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
            notes.OrderByDescending(c => c.date).Take(5).ToList();
            ViewBag.Not = notes;
        }
    }
}
