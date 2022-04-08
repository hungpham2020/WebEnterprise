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
    public class StaffController : Controller
    {
        private readonly IStaffRepo staffRepo;
        private readonly ApplicationDbContext context;

        public StaffController(IStaffRepo _staffRepo, ApplicationDbContext _context)
        {
            staffRepo = _staffRepo;
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

            var staffs = staffRepo.GetAllStaffs();
            if(staffs != null)
            {
                if (!String.IsNullOrEmpty(keyword))
                {
                    staffs = staffs.Where(p => p.FullName.Contains(keyword));
                }

                var paging = new CommonPaging(staffs.Count(), pageIndex, pageSize);

                staffs = staffs.Skip((int)((paging.PageIndex - 1) * paging.PageSize)).Take((int)(paging.PageSize));

                staffs.ToList();

                ViewBag.Paging = paging;
            }
            ViewDepartment();
            return View(staffs);
        }

        
        [HttpPost]
        public async Task<IActionResult> AddStaff(UserAddDTO user, IFormCollection f)
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
                    var account = await staffRepo.AddStaff(user);
                    if (account.Id != null)
                    {
                        TempData["message"] = $"Successfully Add new Staff {account.FullName}";
                        return RedirectToAction("Index");
                    }
                }
            }
            TempData["message"] = $"Cannot Add staff";
            return RedirectToAction("Index");
        }

        public IActionResult EditStaff(string id)
        {
            var staff = staffRepo.GetEditStaff(id);
            if (staff == null)
            {
                return RedirectToAction("Index");
            }
            SelectedDepart(id);
            return View(staff);
        }
        
        [HttpPost]
        public IActionResult EditStaff(UserDTO res)
        {
            if (ModelState.IsValid)
            {
                var staff = staffRepo.EditStaff(res);
                if(staff != null)
                {
                    TempData["message"] = $"Successfully Edit Staff {staff.FullName}";
                }
                return RedirectToAction("Index");
            }
            SelectedDepart(res.Id);
            return View(res);
        }

        public IActionResult DeleteStaff(string id)
        {
            var staff = staffRepo.DeleteStaff(id);
            if(!staff)
            {
                return RedirectToAction("Index");
            }
            TempData["message"] = $"Successfully Delete Staff";
            return RedirectToAction("Index");
        }
    }
}
