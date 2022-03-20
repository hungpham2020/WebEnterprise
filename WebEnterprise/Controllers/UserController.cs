﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                             AuthorName = u.UserName,
                         }).ToList();
            return View(posts);

        }


        [HttpPost]
        public IActionResult Comment(Comment c, IFormCollection f)
        {
            var p = Convert.ToInt32(f["PostId"]);
            Post post = context.Posts.Where(u => u.Id == p).FirstOrDefault();
            CustomUser user = context.Users.Where(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier)).FirstOrDefault();

            var comment = new Comment();
            comment.Description = c.Description;
            comment.Post = post;
            comment.UpdateTime = DateTime.Now;
            comment.CustomUser = user;

            context.Comments.Add(comment);
            context.SaveChanges();
            return RedirectToAction("ShowComment");
        }

        public IActionResult ShowComment(int id)
        {
            var post = context.Posts.Where(u => u.Id == id).Select(u => new PostDTO
            {
                Id = id,
                Title = u.Title,
                Description = u.Description
            }).FirstOrDefault();
            var comments = (from p in context.Posts
                            join c in context.Comments
                            on post.Id equals c.Post.Id
                            select new Comment
                            {
                                Id = c.Id,
                                UpdateTime = c.UpdateTime,
                                CustomUser = c.CustomUser,
                                Description = c.Description,
                                Post = c.Post,
                            }).ToList();
            return View(comments);
        }

    }
}

