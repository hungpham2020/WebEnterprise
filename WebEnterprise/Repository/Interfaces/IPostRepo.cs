using WebEnterprise.Models;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Repository.Interfaces
{
    public interface IPostRepo
    {
        public IQueryable<PostDTO> GetAllPost();

        public bool AddPost(PostDTO req, string authorId);

        public bool EditPost(PostDTO req);

        public bool DeletePost(int id);

        public Post GetPostById(int id);

        public PostDTO GetEditPost(int id);
    }
}
