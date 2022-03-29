using Microsoft.AspNetCore.Mvc;
using WebEnterprise.Data;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Controllers
{
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

        public IActionResult Index()
        {
            ViewLastest();
            ViewMostLike();
            return View();
        }
    }
}
