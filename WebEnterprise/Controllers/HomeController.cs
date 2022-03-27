using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebEnterprise.Data;
using WebEnterprise.Models;

namespace WebEnterprise.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _context)
        {
            _logger = logger;
            context = _context;
        }

        public IActionResult Index()
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
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}