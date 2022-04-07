using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Controllers
{
    [Authorize(Roles = "Admin, Coordinator, Assurance")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext context;

        public DashboardController(ApplicationDbContext _context)
        {
            context = _context;
        }

        private void ViewLastest()
        {
            var lastest = (from p in context.Posts
                           join u in context.Users on p.UserId equals u.Id
                           join c in context.Categories on p.CateId equals c.Id
                           orderby p.OpenDate descending
                           select new PostDTO
                           {
                               Title = p.Title,
                               CatName = c.Name,
                               AuthorName = u.FullName,
                               OpenDate = p.OpenDate
                           }).Take(5).ToList();
            ViewBag.Lastest = lastest;
        }

        private void ViewMostLike()
        {
            var liked = (from p in context.Posts
                         join u in context.Users on p.UserId equals u.Id
                         join c in context.Categories on p.CateId equals c.Id
                         select new PostDTO
                         {
                             Title = p.Title,
                             CatName = c.Name,
                             AuthorName = u.FullName,
                             Like = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == true).Count()
                         }).OrderByDescending(x => x.Like).Take(5).ToList();
            ViewBag.Liked = liked;
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
        public IActionResult Index()
        {
            Notifiation();
            ViewLastest();
            ViewMostLike();
            return View();
        }
    }
}
