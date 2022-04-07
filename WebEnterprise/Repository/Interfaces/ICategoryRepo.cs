using WebEnterprise.Models;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Repository.Interfaces
{
    public interface ICategoryRepo
    {
        public IQueryable<Category> GetAllCategory();

        public Category GetCatDetail(int? id);

        public bool AddCat(CatDTO req);

        public bool DeleteCat(int id);

        public bool EditCat(CatDTO req);
    }
}
