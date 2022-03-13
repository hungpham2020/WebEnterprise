using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebEnterprise.Data;
using WebEnterprise.Models;

namespace WebEnterprise.Controllers
{
    public class UserController : Controller
    {
        
            private readonly UserManager<CustomUser> userManager;
            private readonly ApplicationDbContext context;

            public UserController(UserManager<CustomUser> _userManager, ApplicationDbContext _context)
            {
                userManager = _userManager;
                context = _context;
            }
            public IActionResult Index()
            {
                var post = (from p in context.Posts
                            join u in context.Users
                            on p.UserId equals u.UserName
                            select new Post
                            {
                                Id = p.Id,
                                Title = p.Title,
                                Description = p.Description,
                                UserId = u.UserName
                            }).ToList();
                return View(post);
            }

        
    }
}
