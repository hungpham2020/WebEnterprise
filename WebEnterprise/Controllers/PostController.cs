using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebEnterprise.Data;
using WebEnterprise.Models;
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

        private void ViewCat()
        {
            ViewBag.Cat = context.Categories.ToList();
        }
        private void ViewCate()
        {
            var cats = context.Categories.Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
            }).ToList();

            ViewBag.Cat = new SelectList(cats, "Id", "Name");
        }
        private List<Category> LoadCat(string form)
        {
            if (form != null)
            {
                var selected = form.Split(',').Select(id => Int32.Parse(id)).ToArray();
                return context.Categories.Where(c => selected.Contains(c.Id)).ToList();
            }
            else
            {
                return context.Categories.Where(c => c.Id == 0).ToList();
            }
        }

        private void SelectedCat(int id)
        {
            var selectedCat = context.Categories.FirstOrDefault(c => c.Id == id);
            var cats = context.Categories.Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
            }).ToList();
            ViewBag.SelectedCat = new SelectList(cats, selectedCat);
        }

        public IActionResult Index()
        {
            var posts = (from p in context.Posts
                         join u in context.Users on p.UserId equals u.Id
                         join c in context.Categories on p.CateId equals c.Id
                         select new PostDTO
                         {
                             Id = p.Id,
                             Title = p.Title,
                             Description = p.Description,
                             CatId = c.Id,
                             CatName = c.Name,
                             AuthorName =u.UserName,
                         }).ToList();
            ViewCat();
            return View(posts);
        }

        public IActionResult GetUserPost()
        {
            var posts = (from p in context.Posts
                         where p.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)
                         select new Post
                         {
                             Id = p.Id,
                             Title = p.Title,
                             Description= p.Description,
                             CateId = p.CateId,
                             OpenDate = p.OpenDate,
                             ClosedDate = p.ClosedDate,
                         }).ToList();
            return View(posts);
        }

        [HttpPost]
        public IActionResult AddPost(PostDTO res, IFormCollection f)
        {
            var cat = LoadCat(f["CatIds"]);
            foreach(var c in cat)
            {
                res.CatId = c.Id;
            }
            if (!ModelState.IsValid)
            {
                var post = new Post();
                post.Title = res.Title;
                post.Description = res.Description;
                post.CateId = res.CatId;
                post.OpenDate = DateTime.Now;
                post.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                context.Posts.Add(post);
                context.SaveChanges();
                TempData["message"] = $"Successfully Add new Post {post.Title}";
                return RedirectToAction("Index");
            }
            return BadRequest();
          
        }

        public IActionResult Like(int id)
        {
            int count = 0;
            var user = context.UserLikePosts.Where(x => x.Id == id).Select(x => new UserLikePost
            {
                UserId= x.UserId,
                PostId= x.PostId,
                Status = x.Status
            }).ToList();
            foreach(var l in user)
            {
                if(l.Status == true)
                {
                    count++;
                }
                else
                {
                    count--;
                }
            }
            return View(count);
        }


        public IActionResult EditPost(int id)
        {
            var post = context.Posts.Where(u => u.Id == id).Select(u => new PostDTO
            {
                Id = id,
                Title = u.Title,
                Description = u.Description,
                OpenDate = u.OpenDate,
                CatId = u.CateId
            }).FirstOrDefault();
            if (post == null)
            {
                return RedirectToAction("Index");
            }
            ViewCate();
            SelectedCat(id);
            return View(post);
        }

        [HttpPost]
        public IActionResult EditPost(PostDTO res)
        {
            var post = context.Posts.Find(res.Id);
            if (post != null)
            {
                post.Id = res.Id;
                post.Title = res.Title;
                post.Description = res.Description;
                post.OpenDate = res.OpenDate;
                post.ClosedDate = res.ClosedDate;
                post.CateId = res.CatId;
                context.Entry(post).State = EntityState.Modified;
                context.SaveChanges();
                TempData["message"] = $"Successfully Edit Post {post.Title}";
                return RedirectToAction("Index");
            }
            SelectedCat(res.Id);
            return View(res);
        }

        public IActionResult DeletePost(int id)
        {
            var post = context.Posts.FirstOrDefault(p => p.Id == id);
            if(post != null)
            {
                context.Remove(post);
                context.SaveChanges();
                TempData["message"] = $"Successfully Delete Post {post.Title}";
            }
            return RedirectToAction("Index");
        }
    }
}
