using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.DTO;
using WebEnterprise.Repository.Interfaces;

namespace WebEnterprise.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext context;

        public UserRepo(ApplicationDbContext _context)
        {
            context = _context;
        }

        public bool AddComment(CommentDTO c, string authorId)
        {
            var comment = new Comment
            {
                Description = c.Description,
                PostId = c.PostId,
                UpdateTime = DateTime.Now,
                AuthorId = authorId
            };
            if (comment.AuthorId != null || comment.PostId != 0)
            {
                context.Comments.Add(comment);
                context.SaveChanges();
                return true;
            }
            return false;

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

        public bool DeleteComment(int id, string authorId)
        {
            var comment = context.Comments.FirstOrDefault(c => c.Id == id);
            if (comment != null && comment.AuthorId == authorId)
            {
                context.Comments.Remove(comment);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeletePost(int id)
        {
            var post = context.Posts.FirstOrDefault(x => x.Id == id);
            if (post != null)
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
            }
            return false;
        }

        public UserLikePost Dislike(UserLikePost req)
        {
            var userPost = context.UserLikePosts.FirstOrDefault(u => u.UserId == req.UserId && u.PostId == req.PostId);
            if (userPost == null)
            {
                var userLike = new UserLikePost
                {
                    UserId = req.UserId,
                    PostId = req.PostId,
                    Status = false,
                };
                context.Add(userLike);
                context.SaveChanges();
                return userLike;
            }
            else
            {
                userPost.Status = req.Status;
                context.Update(userPost);
                context.SaveChanges();
                return userPost;
            }
        }

        public bool EditComment(CommentDTO req, string authorId)
        {
            var comment = context.Comments.Find(req.CommentId);
            if (comment != null && comment.AuthorId == authorId)
            {
                comment.UpdateTime = DateTime.Now;
                comment.Description = req.Description;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool EditPost(PostDTO req)
        {
            var post = context.Posts.FirstOrDefault(x => x.Id == req.Id);
            if(post != null)
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

        public List<CommentDTO> GetAllComment(int postId)
        {
            if(postId == 0)
            {
                return null;
            }
            var comments = (from p in context.Posts
                            join c in context.Comments on p.Id equals c.PostId
                            join u in context.Users on c.AuthorId equals u.Id
                            where c.PostId == postId
                            select new CommentDTO
                            {
                                CommentId = c.Id,
                                UpdateTime = c.UpdateTime,
                                Description = c.Description,
                                AuthorName = u.FullName,
                                AuthorId = u.Id,
                                PostDescription = p.Description,
                                PostId = postId,
                            }).ToList();
            return comments;
        }

        public IQueryable<PostDTO> GetAllPost(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var posts = from p in context.Posts
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
                                UserId = u.Id,
                                AuthorName = u.FullName,
                                Status = context.UserLikePosts.FirstOrDefault(x => x.UserId.Equals(userId) && x.PostId == p.Id).Status,
                                Like = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == true).Count(),
                                DisLike = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == false).Count(),
                            };
                if(posts != null)
                {
                    return posts;
                }
            }
            return null;
        }

        public PostDTO GetPostDetail(int id)
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
            if(post != null)
            {
                return post;
            }
            return null;
        }

        public PostDTO GetPostForComment(int postId, string userId)
        {
            if(postId != 0 && userId != null)
            {
                var post = (from p in context.Posts
                            join u in context.Users on p.UserId equals u.Id
                            join c in context.Categories on p.CateId equals c.Id
                            where p.Id == postId
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
                if(post != null)
                {
                    return post;
                }
            }
            return null;
        }

        public List<PostDTO> GetUserPost(string authorId)
        {
            if (!string.IsNullOrEmpty(authorId))
            {
                var posts = (from p in context.Posts
                             join u in context.Users
                             on p.UserId equals u.Id
                             join c in context.Categories on p.CateId equals c.Id
                             where u.Id == authorId
                             select new PostDTO
                             {
                                 Id = p.Id,
                                 Title = p.Title,
                                 Description = p.Description,
                                 OpenDate = p.OpenDate,
                                 CatId = c.Id,
                                 AuthorName = u.FullName,
                                 Status = context.UserLikePosts.FirstOrDefault(x => x.UserId.Equals(authorId) && x.PostId == p.Id).Status,
                                 Like = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == true).Count(),
                                 DisLike = context.UserLikePosts.Where(x => x.PostId == p.Id && x.Status == false).Count(),
                             }).ToList();
                if(posts.Count > 0)
                {
                    return posts;
                }
            }
            return null;
        }

        public UserLikePost Like(UserLikePost req)
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
                return userLike;
            }
            else
            {
                userPost.Status = req.Status;
                context.Update(userPost);
                context.SaveChanges();
                return userPost;
            }
        }
    }
}
