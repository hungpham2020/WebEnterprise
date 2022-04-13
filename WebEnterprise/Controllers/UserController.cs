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
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IUserRepo userRepo;

        public UserController(ApplicationDbContext _context, IUserRepo _userRepo)
        {
            context = _context;
            userRepo = _userRepo;
        }

        //-----------------------------------------
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

        //-----------------------------------

        public IActionResult Index(int? pageIndex, int? pageSize, int? filter1)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            filter1 = filter1 ?? 0;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var posts = userRepo.GetAllPost(userId);

            var paging = new CommonPaging(posts.Count(), pageIndex, pageSize);
            posts = posts.Skip((int)((pageIndex - 1) * pageSize)).Take((int)(pageSize));
                
            switch (filter1)
            {
                case 1:
                    posts = posts.OrderByDescending(x => x.OpenDate);
                    posts.ToList();
                    break;
                case 2:
                    posts = posts.OrderByDescending(x => x.Like - x.DisLike);
                    posts.ToList();
                    break;
                default:
                    posts.ToList();
                    break;
            }

            ViewBag.Paging = paging;
            ViewCat();
            Notifiation();
            return View(posts);
        }


        public IActionResult UserWall()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var posts = userRepo.GetUserPost(id);
            ViewCat();
            Notifiation();
            return View(posts);
        }


        [HttpPost]
        public IActionResult Comment(CommentDTO c)
        {
            var result = userRepo.AddComment(c, User.FindFirstValue(ClaimTypes.NameIdentifier));
           if(!result)
            {
                TempData["message"] = $"You cannot comment, please check your account or post again!";
                return RedirectToAction("ShowComment");
            }
            else
            {
                return RedirectToAction("ShowComment", new { id = c.PostId });
            }
        }


        [HttpGet]
        public IActionResult ShowComment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = userRepo.GetPostForComment(id, userId);
            ViewBag.Post = post;
            var comments = userRepo.GetAllComment(id);
            Notifiation();
            return View(comments);
        }

        [HttpPost]
        public IActionResult EditComment(CommentDTO req)
        {
            var result = userRepo.EditComment(req, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (result)
            {
                TempData["message"] = $"Successfully Edit Comment";
                return RedirectToAction("ShowComment", new { id = req.PostId });
            }
            TempData["message"] = $"Cannot Edit Comment";
            return RedirectToAction("ShowComment", new { id = req.PostId });
        }

        public IActionResult DeleteComment(int id)
        {
            var result = userRepo.DeleteComment(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (result)
            {
                TempData["message"] = $"Delete Comment Successfully!";
                return RedirectToAction("ShowComment", new { id = id});
            }
            TempData["message"] = $"You not have permission to delete this comment";
            return RedirectToAction("ShowComment", new { id = id });
        }



        //-----------------------------------------------------


        [HttpPost]
        public IActionResult AddPost(PostDTO res, IFormCollection f)
        {
            if (res.CatId != 0)
            {
                var cat = LoadCat(f["CatIds"]);
                foreach (var c in cat)
                {
                    res.CatId = c.Id;
                }
                if (ModelState.IsValid)
                {
                    var result = userRepo.AddPost(res, User.FindFirstValue(ClaimTypes.NameIdentifier));

                    if (result)
                    {
                        TempData["message"] = $"Successfully Add new Post {res.Title}";
                        return RedirectToAction("Index");
                    }
                }
            }
            TempData["message"] = $"Cannot Add this Post";
            return RedirectToAction("Index");

        }
        
        [HttpGet]
        public IActionResult EditPost(int id)
        {
            var post = userRepo.GetPostDetail(id);
            if (post.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {

                ViewCate();
                Notifiation();
                SelectedCat(id);
                return View(post);
            }
            TempData["message"] = $"You can not edit post {post.Title}, cause you noy owner! ";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(PostDTO res)
        {
            if (ModelState.IsValid)
            {
                var result = userRepo.EditPost(res);
                if (result)
                {
                    TempData["message"] = $"Successfully Edit Post {res.Title}";
                    return RedirectToAction("Index");
                }
            }
            SelectedCat(res.Id);
            return View(res);
        }

        public IActionResult DeletePost(int id)
        {
            var result = userRepo.DeletePost(id);
            if (result)
            {
                TempData["message"] = $"Successfully Delete Post";
                return RedirectToAction("UserWall");
            }
            return RedirectToAction("UserWall");
        }

        [HttpPost]
        public IActionResult Like([FromBody] UserLikePost req)
        {
            var userPost = userRepo.Like(req);
            return Json(userPost);
        }

        [HttpPost]
        public IActionResult DisLike([FromBody] UserLikePost req)
        {
            var userPost = userRepo.Dislike(req);
            return Json(userPost);
        }
        public IActionResult AssuranceIndex()
        {
            Notifiation();
            return View();
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

