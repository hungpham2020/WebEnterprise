using Microsoft.AspNetCore.Mvc;
using WebEnterprise.Data;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext context;

        public PostController(ApplicationDbContext _context)
        {
            context = _context;
        }
        public IActionResult Index()
        {
            var posts = (from p in context.Posts
                         join c in context.Categories on p.CateId equals c.Id
                         join u in context.Users on p.UserId equals u.Id
                         select new PostDTO
                         {
                             Id = p.Id,
                             Title = p.Title,
                             Description = p.Description,
                             CatName = c.Name,
                             AuthorName = u.FullName,
                         }).ToList();
            return View(posts);
        }

        [HttpPost]
        public IActionResult AddPost(PostDTO post)
        {
            if (ModelState.IsValid)
            {

            }
        }
    }
}
