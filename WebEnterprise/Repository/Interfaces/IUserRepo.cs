using WebEnterprise.Models;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Repository.Interfaces
{
    public interface IUserRepo
    {
        public IQueryable<PostDTO> GetAllPost(string userId);

        public List<PostDTO> GetUserPost(string authorId);

        public PostDTO GetPostDetail(int postId);

        public PostDTO GetPostForComment(int postId, string userId);

        public bool AddPost(PostDTO req, string authorId);

        public bool EditPost(PostDTO req);

        public bool DeletePost(int id);

        public UserLikePost Like(UserLikePost req);

        public UserLikePost Dislike(UserLikePost req);

        public bool AddComment(CommentDTO comment, string authorId);

        public List<CommentDTO> GetAllComment(int postId);

        public bool EditComment(CommentDTO comment, string authorId);

        public bool DeleteComment(int id, string authorId);
    }
}
