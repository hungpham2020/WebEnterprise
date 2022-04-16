using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.DTO;
using WebEnterprise.Repository.Interfaces;

namespace WebEnterprise.Repository
{
    public class CatRepo : ICategoryRepo
    {
        private readonly ApplicationDbContext context;

        public CatRepo(ApplicationDbContext _context)
        {
            context = _context;
        }

        public bool AddCat(CatDTO req)
        {
            var cate = new Category();
            if(!string.IsNullOrEmpty(req.CatName) && !string.IsNullOrEmpty(req.Description))
            {
                cate.Name = req.CatName;
                cate.Description = req.Description;
                context.Categories.Add(cate);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteCat(int id)
        {
            var cat = GetCatDetail(id);
            if (cat != null)
            {
                context.Remove(cat);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool EditCat(Category req)
        {
            var cat = GetCatDetail(req.Id);
            if(cat == null)
            {
                return false;
            }
            if(!string.IsNullOrEmpty(req.Name) && !string.IsNullOrEmpty(req.Description))
            {
                cat.Name = req.Name;
                cat.Description = req.Description;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public IQueryable<Category> GetAllCategory()
        {
            var cats = from c in context.Categories
                       select new Category
                       {
                           Id = c.Id,
                           Name = c.Name,
                           Description = c.Description,
                       };
            return cats;
        }

        public Category GetCatDetail(int? id)
        {
            var cat = context.Categories.FirstOrDefault(x => x.Id == id);
            if(cat == null)
            {
                return null;
            }
            return cat;
        }
    }
}
