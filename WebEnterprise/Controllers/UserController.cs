using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.DTO;


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

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var posts = (from p in context.Posts
                         join u in context.Users on p.UserId equals u.Id
                         join c in context.Categories on p.CateId equals c.Id
                         select new PostDTO
                         {
                             Id = p.Id,
                             Title = p.Title,
                             Description = p.Description,
                             OpenDate = p.OpenDate,
                             CatId = c.Id,
                             CatName = c.Name,
                             AuthorName = u.FullName,
                             Status = context.UserLikePosts.FirstOrDefault(x => x.UserId.Equals(userId) && x.PostId == p.Id).Status,
                             Like = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == true).Count(),
                             DisLike = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == false).Count(),
                         }).ToList(); 
            
            ViewCat();
            Notifiation();
            return View(posts);
        }


        public IActionResult UserWall()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var posts = (from p in context.Posts 
                        join u in context.Users
                        on p.UserId equals u.Id
                        join c in context.Categories on p.CateId equals c.Id
                        where u.Id == id
                        select new PostDTO
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Description= p.Description,
                            OpenDate = p.OpenDate,
                            CatId = c.Id,
                            AuthorName = u.FullName,
                            Status = context.UserLikePosts.FirstOrDefault(x => x.UserId.Equals(id) && x.PostId == p.Id).Status,
                            Like = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == true).Count(),
                            DisLike = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == false).Count(),
                        }).ToList();
            ViewCat();
            Notifiation();
            return View(posts);
        }


        [HttpPost]
        public IActionResult Comment(CommentDTO c)
        {
            var comment = new Comment();
            comment.Description = c.Description;
            comment.PostId = c.PostId;
            comment.UpdateTime = DateTime.Now;
            comment.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

           if(comment.AuthorId == null||comment.PostId == 0)
            {
                TempData["message"] = $"You cannot comment, please check your account or post again!";
                return RedirectToAction("ShowComment");
            }
            else
            {
                context.Comments.Add(comment);
                context.SaveChanges();
                return RedirectToAction("ShowComment", new { id = comment.PostId });
            }
        }


        [HttpGet]
        public IActionResult ShowComment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = (from p in context.Posts
                        join u in context.Users on p.UserId equals u.Id
                        join c in context.Categories on p.CateId equals c.Id
                        where p.Id == id
                        select new PostDTO
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Description = p.Description,
                            OpenDate = p.OpenDate,
                            CatId = c.Id,
                            AuthorName = u.FullName,
                            Status = context.UserLikePosts.FirstOrDefault(x => x.UserId.Equals(userId) && x.PostId == p.Id).Status,
                            Like = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == true).Count(),
                            DisLike = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == false).Count(),
                        }).FirstOrDefault();
            ViewBag.Post = post;
            var comments = (from p in context.Posts
                            join c in context.Comments on p.Id equals c.PostId
                            join u in context.Users on c.AuthorId equals u.Id
                            where c.PostId == id
                            select new CommentDTO
                            {
                                CommentId = c.Id,
                                UpdateTime = c.UpdateTime,
                                Description = c.Description,
                                AuthorName = u.FullName,
                                PostDescription = p.Description,
                                PostId = id,
                            }).ToList();
            Notifiation();
            return View(comments);
        }

        [HttpPost]
        public IActionResult EditComment(int PostId, string Description, int CommentId)
        {
            var comment = context.Comments.Find(CommentId);
            if (comment.AuthorId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                comment.UpdateTime = DateTime.Now;
                comment.Description = Description;
                comment.PostId = PostId;
                context.Entry(comment).State = EntityState.Modified;
                context.SaveChanges();
                TempData["message"] = $"Successfully Edit Comment";
                return RedirectToAction("ShowComment", new { id = comment.PostId });
            }
            TempData["message"] = $"Cannot Edit Comment";
            return RedirectToAction("ShowComment", new { id = PostId });
        }

        public IActionResult DeleteComment(int id)
        {
            var comment = context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment.AuthorId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                context.Comments.Remove(comment);
                context.SaveChanges();
                TempData["message"] = $"Delete Comment Successfully!";
                return RedirectToAction("ShowComment", new { id = comment.PostId });
            }
            TempData["message"] = $"You not have permission to delete this comment";
            return RedirectToAction("ShowComment", new { id = comment.PostId });
        }



        //-----------------------------------------------------


        [HttpPost]
        public IActionResult AddPost(PostDTO res, IFormCollection f)
        {
            var cat = LoadCat(f["CatIds"]);
            foreach (var c in cat)
            {
                res.CatId = c.Id;
            }
              if(res.CatId != 0)
                {
                    if (!ModelState.IsValid)
                    {
                        var post = new Post();
                        post.Title = res.Title;
                        post.Description = res.Description;
                        post.CateId = res.CatId;
                        post.OpenDate = DateTime.Now;
                        post.ClosedDate = post.OpenDate.AddDays(14);
                        post.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                        var note = new Notification();
                        var user = (from u in context.Users
                                   where u.Id == post.UserId
                                   select new CustomUser
                                   {
                                       Id = u.Id,
                                       FullName = u.FullName,
                                       UserName = u.UserName
                                   }).FirstOrDefault();

                       if(user != null && post.Title != null)
                        {
                            note.description = $"{user.FullName} add new post {post.Title}";
                            note.date = DateTime.Now;
                            note.UserId = post.UserId;

                            context.Posts.Add(post);
                            context.Notifications.Add(note);
                            context.SaveChanges();

                            TempData["message"] = $"Successfully Add new Post {post.Title}";
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
            var post = context.Posts.Where(u => u.Id == id).Select(u => new PostDTO
            {
                Id = id,
                Title = u.Title,
                Description = u.Description,
                OpenDate = u.OpenDate,
                ClosedDate = u.ClosedDate,
                UserId = u.UserId,
                CatId = u.CateId
            }).FirstOrDefault();
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
            var post = context.Posts.Find(res.Id);
            if (post != null)
            {
                if (ModelState.IsValid)
                {
                    post.Id = res.Id;
                    post.Title = res.Title;
                    post.Description = res.Description;
                    post.OpenDate = res.OpenDate;
                    post.ClosedDate = res.ClosedDate;
                    post.CateId = res.CatId;

                    //string extension = Path.GetExtension(res.FileUpload.FileName);
                    //string newname = post.Title + extension;
                    //post.File = await FileControl.UploadFile(res.FileUpload, @"postFiles\", newname.ToLower());
                    context.Entry(post).State = EntityState.Modified;
                    context.SaveChanges();
                    TempData["message"] = $"Successfully Edit Post {post.Title}";
                    return RedirectToAction("Index");
                }
            }
            SelectedCat(res.Id);
            return View(res);
        }

        public IActionResult DeletePost(int id)
        {
            var post = context.Posts.FirstOrDefault(p => p.Id == id);
            var user = context.Users.FirstOrDefault(u => u.Id == post.UserId);
            var note = context.Notifications.FirstOrDefault(p => p.description.Contains($"{user.UserName} add new post {post.Title}"));
            if (post.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                context.Remove(post);
                context.Remove(note);
                context.SaveChanges();
                TempData["message"] = $"Successfully Delete Post {post.Title}";
                return RedirectToAction("UserWall");
            }
            TempData["message"] = $"Can not delete post {post.Title}, cause you are not owner!";
            return RedirectToAction("UserWall");
        }

        [HttpPost]
        public IActionResult Like([FromBody] UserLikePost req)
        {
            var userPost = context.UserLikePosts.FirstOrDefault(u => u.UserId == req.UserId && u.PostId == req.PostId);
            if (userPost == null)
            {
                var userLike = new UserLikePost
                {
                    UserId = req.UserId,
                    PostId = req.PostId,
                    Status = true,
                };
                context.Add(userLike);
                context.SaveChanges();
                return Json(userLike);
            }
            else
            {
                userPost.Status = req.Status;
                context.Update(userPost);
                context.SaveChanges();
                return Json(userPost);
            }
        }

        [HttpPost]
        public IActionResult DisLike([FromBody] UserLikePost req)
        {
            var userPost = context.UserLikePosts.FirstOrDefault(u => u.UserId == req.UserId && u.PostId == req.PostId);
            if(userPost == null)
            {
                var userLike = new UserLikePost
                {
                    UserId = req.UserId,
                    PostId = req.PostId,
                    Status = false,
                };
                context.Add(userLike);
                context.SaveChanges();
                return Json(userLike);
            }
            else
            {
                userPost.Status = req.Status;
                context.Update(userPost);
                context.SaveChanges();
                return Json(userPost);
            }
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

