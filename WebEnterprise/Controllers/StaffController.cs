﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.Common;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Controllers
{
    public class StaffController : Controller
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly ApplicationDbContext context;

        public StaffController(UserManager<CustomUser> _userManager, ApplicationDbContext _context)
        {
            userManager = _userManager;
            context = _context;
        }
        public IActionResult Index(string? keyword, int? pageIndex, int? pageSize)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            keyword = keyword ?? "";

            var staffs = from u in context.Users
                        join ur in context.UserRoles on u.Id equals ur.UserId
                        join r in context.Roles on ur.RoleId equals r.Id
                        where r.Name == "Staff"
                        select new UserDTO
                        {
                            Id = u.Id,
                            FullName = u.FullName,
                            UserName = u.UserName,
                            Email = u.Email,
                        };
            if (!String.IsNullOrEmpty(keyword))
            {
                staffs = staffs.Where(p => p.FullName.Contains(keyword));
            }

            var paging = new CommonPaging(staffs.Count(), pageIndex, pageSize);

            staffs = staffs.Skip((int)((paging.PageIndex - 1) * paging.PageSize)).Take((int)(paging.PageSize));

            staffs.ToList();

            ViewBag.Paging = paging;
            Notifiation();
            return View(staffs);
        }

        
        [HttpPost]
        public async Task<IActionResult> AddStaff(string fullname, string username, string email)
        {
            if (ModelState.IsValid)
            {
                var account = new CustomUser
                {
                    UserName = username,
                    FullName = fullname,
                    Email = email
                };
                var result = await userManager.CreateAsync(account, "Abc@12345");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(account, "Staff");
                    TempData["message"] = $"Successfully Add new Staff {account.FullName}";
                    return RedirectToAction("Index");
                }
            }
            return Content("Cannot Add");
        }

        public IActionResult EditStaff(string id)
        {
            var staff = context.Users.Where(u => u.Id == id).Select(u => new UserDTO
            {
                UserName=u.UserName,
                FullName=u.FullName,
                Email=u.Email,
                PhoneNumber=u.PhoneNumber
            }).FirstOrDefault();
            if (staff == null)
            {
                return RedirectToAction("Index");
            }
            Notifiation();
            return View(staff);
        }

        [HttpPost]
        public IActionResult EditStaff(UserDTO res)
        {
            if (ModelState.IsValid)
            {
                var staff = context.Users.FirstOrDefault(context => context.Id == res.Id);
                {
                    staff.Email = res.Email;
                    staff.UserName = res.UserName;
                    staff.FullName = res.FullName;
                    staff.PhoneNumber = res.PhoneNumber;

                };
                context.SaveChanges();
                TempData["message"] = $"Successfully Edit Staff {staff.FullName}";
                return RedirectToAction("Index");
            }
            return View(res);
        }

        public IActionResult DeleteStaff(string id)
        {
            var staff = context.Users.FirstOrDefault(s => s.Id == id);
            if(staff == null)
            {
                return RedirectToAction("Index");
            }
            context.Remove(staff);
            context.SaveChanges();
            TempData["message"] = $"Successfully Delete Staff";
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
