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

namespace WebEnterprise.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AssuranceController : Controller
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly ApplicationDbContext context;

        public AssuranceController(UserManager<CustomUser> _userManager, ApplicationDbContext _context)
        {
            userManager = _userManager;
            context = _context;
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

            var assurances = from u in context.Users
                             join ur in context.UserRoles on u.Id equals ur.UserId
                             join r in context.Roles on ur.RoleId equals r.Id
                             where r.Name == "Assurance"
                             select new UserDTO
                             {
                                 Id = u.Id,
                                 FullName = u.FullName,
                                 UserName = u.UserName,
                                 Email = u.Email
                             };

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
        public async Task<IActionResult>AddAssurance(string userName, string fullName, string email, IFormCollection f)
        {
            var depart = LoadDepartment(f["DepartmentIds"]);
            if(depart != null)
            {
                if (ModelState.IsValid)
                {
                    var account = new CustomUser
                    {
                        UserName = userName,
                        FullName = fullName,
                        Email = email,
                    };
                    foreach (var d in depart)
                    {
                        account.DepartmentId = d.Id;
                    }
                    var result = await userManager.CreateAsync(account, "Abc@12345");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(account, "Assurance");
                        TempData["message"] = $"Successfully Add new Assurance {account.FullName}";
                        return RedirectToAction("Index");
                    }
                }
            }
            TempData["message"] = $"Cannot Add Assurance";
            return RedirectToAction("Index");
        }

        public IActionResult EditAssurance(string id)
        {
            var assurance = context.Users.Where(u => u.Id == id).Select(u => new UserDTO
            {
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                UserName = u.UserName
            }).FirstOrDefault();
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
                var assurance = context.Users.Find(res.Id);
                if(assurance != null)
                {
                    assurance.Email = res.Email;
                    assurance.FullName = res.FullName;
                    assurance.UserName = res.UserName;
                    assurance.PhoneNumber = res.PhoneNumber;
                    context.SaveChanges();
                    TempData["message"] = $"Successfully Edit Assurance {assurance.FullName}";
                }
                return RedirectToAction("Index");
            }
            SelectedDepart(res.Id);
            return View(res);
        }

        public IActionResult DeleteAssurance(string id)
        {
            var assurance = context.Users.FirstOrDefault(s => s.Id == id);
            if (assurance == null)
            {
                return RedirectToAction("Index");
            }
            context.Remove(assurance);
            context.SaveChanges();
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
