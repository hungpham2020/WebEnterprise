using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.DTO;
using WebEnterprise.Repository.Interfaces;

namespace WebEnterprise.Repository
{
    public class PostRepo : IPostRepo
    {
        private readonly ApplicationDbContext context;

        public PostRepo(ApplicationDbContext _context)
        {
            context = _context;
        }

        public bool AddPost(PostDTO req, string authorId)
        {
            var post = new Post
            {
                Title = req.Title,
                Description = req.Description,
                CateId = req.CatId,
                OpenDate = DateTime.Now,
                UserId = authorId,
            };
            post.ClosedDate = post.OpenDate?.AddDays(14);

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
                return true;
            }
            return false;
        }

        public bool DeletePost(int id)
        {
            var post = GetPostById(id);
            if(post != null)
            {
                var user = context.Users.FirstOrDefault(u => u.Id == post.UserId);
                var note = context.Notifications.FirstOrDefault(p => p.description.Contains($"{user.UserName} add new post {post.Title}"));
                if (post != null && note != null && user != null)
                {
                    context.Remove(note);
                    context.Remove(post);
                    context.SaveChanges();
                    return true;
                }
                else if(post != null && user != null)
                {
                    context.Remove(post);
                    context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool EditPost(PostDTO req)
        {
            var post = GetPostById(req.Id);
            if (post != null)
            {
                post.Title = req.Title;
                post.Description = req.Description;
                post.OpenDate = req.OpenDate;
                post.ClosedDate = req.ClosedDate;
                post.CateId = req.CatId;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public IQueryable<PostDTO> GetAllPost()
        {
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
            return posts;
        }

        public PostDTO GetEditPost(int id)
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
            if(post != null)
            {
                return post;
            }
            return null;
        }

        public Post GetPostById(int id)
        {
            var post = context.Posts.FirstOrDefault(p => p.Id == id);
            if(post == null)
            {
                return null;
            }
            return post;
        }
    }
}
