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
    public class CoordinatorController : Controller
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly ApplicationDbContext context;

        public CoordinatorController(UserManager<CustomUser> _userManager, ApplicationDbContext _context)
        {
            userManager = _userManager;
            context = _context;
        }
        public IActionResult Index(string? keyword, int? pageIndex, int? pageSize)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            keyword = keyword ?? "";

            var coordinators = from u in context.Users
                                join ur in context.UserRoles on u.Id equals ur.UserId
                                join r in context.Roles on ur.RoleId equals r.Id
                                where r.Name == "Coordinator"
                                select new UserDTO
                                {
                                    Id = u.Id,
                                    FullName = u.FullName,
                                    UserName = u.UserName,
                                    Email = u.Email,
                                };
            if (!String.IsNullOrEmpty(keyword))
            {
                coordinators = coordinators.Where(c => c.FullName.Contains(keyword));
            }

            var paging = new CommonPaging(coordinators.Count(), pageIndex, pageSize);

            coordinators = coordinators.Skip((int)((paging.PageIndex - 1) * paging.PageSize)).Take((int)(paging.PageSize));

            coordinators.ToList();

            ViewBag.Paging = paging;
            Notifiation();
            return View(coordinators);
        }

        [HttpPost]
        public async Task<IActionResult> AddCoordinator(string fullname, string username, string email)
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
                    await userManager.AddToRoleAsync(account, "Coordinator");
                    TempData["message"] = $"Successfully Add new Coordinator {account.FullName}";
                    return RedirectToAction("Index");
                }
            }
            return Content("Cannot Add");
        }

        public IActionResult EditCoordinator(string id)
        {
            var coordinator = context.Users.Where(u => u.Id == id).Select(u => new UserDTO
            {
                UserName=u.UserName,
                FullName=u.FullName,
                Email=u.Email,
                PhoneNumber = u.PhoneNumber
            }).FirstOrDefault();

            if (coordinator == null)
            {
                return RedirectToAction("Index");
            }
            Notifiation();
            return View(coordinator);
        }

        [HttpPost]
        public IActionResult EditCoordinator(UserDTO res)
        {
            if (ModelState.IsValid)
            {
                var coordinator = context.Users.Find(res.Id);
                if(coordinator != null) 
                {
                    coordinator.FullName = res.FullName;
                    coordinator.UserName = res.UserName;
                    coordinator.Email = res.Email;
                    coordinator.PhoneNumber = res.PhoneNumber;
                    context.SaveChanges();
                    TempData["message"] = $"Successfully Edit Coordinator {coordinator.FullName}";
                }
                return RedirectToAction("Index");
            }
            return BadRequest();
        }

        public IActionResult DeleteCoordinator(string id)
        {
            var coordinator = context.Users.FirstOrDefault(s => s.Id == id);
            if (coordinator == null)
            {
                return RedirectToAction("Index");
            }
            context.Remove(coordinator);
            context.SaveChanges();
            TempData["message"] = $"Successfully Delete Coordinator";
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
