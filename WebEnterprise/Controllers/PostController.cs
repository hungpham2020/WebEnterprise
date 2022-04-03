using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security.Claims;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.Common;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Controllers
{
    [Authorize(Roles = "Admin")]
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

        public IActionResult Index(string? keyword, int? pageIndex, int? pageSize)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            keyword = keyword ?? "";

            var posts = from p in context.Posts
                        join u in context.Users on p.UserId equals u.Id
                        join c in context.Categories on p.CateId equals c.Id
                        select new PostDTO
                        {
                            Id = p.Id,
                            Title = p.Title,
                            Description = p.Description,
                            CatId = c.Id,
                            CatName = c.Name,
                            AuthorName = u.UserName,
                            Like = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == true).Count(),
                            DisLike = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == false).Count(),
                        };
            if (!String.IsNullOrEmpty(keyword))
            {
                posts = posts.Where(p => p.Title.Contains(keyword));
            }

            var paging = new CommonPaging(posts.Count(), pageIndex, pageSize);

            posts = posts.Skip((int)((paging.PageIndex - 1) * paging.PageSize)).Take((int)(paging.PageSize));

            posts.ToList();

            ViewBag.Paging = paging;
            Notifiation();
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
            Notifiation();
            return View(posts);
        }

        [HttpPost]
        public IActionResult AddPost(PostDTO res, IFormCollection f)
        {
            var cat = LoadCat(f["CatIds"]);
            foreach (var c in cat)
            {
                res.CatId = c.Id;
            }
            if (res.CatId != 0)
            {
                if (ModelState.IsValid)
                {
                    var post = new Post();
                    post.Title = res.Title;
                    post.Description = res.Description;
                    post.CateId = res.CatId;
                    post.OpenDate = DateTime.Now;
                    post.ClosedDate = post.OpenDate?.AddDays(14);
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

                    if (user != null)
                    {
                        note.description = $"{user.UserName} add new post {post.Title}";
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

        public IActionResult EditPost(int id)
        {
            var post = context.Posts.Where(u => u.Id == id).Select(u => new PostDTO
            {
                Id = id,
                Title = u.Title,
                Description = u.Description,
                OpenDate = u.OpenDate,
                ClosedDate = u.ClosedDate,
                CatId = u.CateId
            }).FirstOrDefault();
            if (post == null)
            {
                return RedirectToAction("Index");
            }
            ViewCate();
            SelectedCat(id);
            Notifiation();
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

                //string extension = Path.GetExtension(res.FileUpload.FileName);
                //string newname = post.Title + extension;
                //post.File = await FileControl.UploadFile(res.FileUpload, @"postFiles\", newname.ToLower());
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
            var user = context.Users.FirstOrDefault(u => u.Id == post.UserId);
            var note = context.Notifications.FirstOrDefault(p => p.description.Contains($"{user.UserName} add new post {post.Title}"));
            if (post != null)
            {
                context.Remove(note);
                context.Remove(post);
                context.SaveChanges();
                TempData["message"] = $"Successfully Delete Post {post.Title}";
            }
            return RedirectToAction("Index");
        }

        public FileResult Export()
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
                            AuthorName = u.UserName,
                            Like = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == true).Count(),
                            DisLike = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == false).Count(),
                        }).ToList();

            FileInfo template = new FileInfo(@"wwwroot/template/Post.xlsx");

            using(ExcelPackage excel = new ExcelPackage(template))
            {
                ExcelWorksheet worksheet = excel.Workbook.Worksheets[0];
                var row = 2;
                foreach(var post in posts)
                {
                    worksheet.Cells[row, 1].Value = post.Title;
                    worksheet.Cells[row, 2].Value = post.Description;
                    worksheet.Cells[row, 3].Value = post.CatName;
                    worksheet.Cells[row, 4].Value = post.AuthorName;
                    worksheet.Cells[row, 5].Value = post.Like;
                    worksheet.Cells[row, 6].Value = post.DisLike;

                    row++;
                }
                string contentType = "";
                new FileExtensionContentTypeProvider().TryGetContentType(template.FullName, out contentType);
                return File(excel.GetAsByteArray(), contentType);
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
