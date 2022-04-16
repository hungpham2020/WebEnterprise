using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.DTO;
using WebEnterprise.Repository.Interfaces;

namespace WebEnterprise.Repository
{
    public class DepartmentRepo : IDepartmentRepo
    {
        private readonly ApplicationDbContext context;

        public DepartmentRepo(ApplicationDbContext _context)
        {
            context = _context;
        }

        public bool AddDepart(DepartDTO req)
        {
            if(!string.IsNullOrEmpty(req.DepartName) && !string.IsNullOrEmpty(req.Description))
            {
                var department = new Department
                {
                    Name = req.DepartName,
                    Description = req.Description,
                };
                context.Add(department);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteDepart(int id)
        {
            var depart = GetDepartDetail(id);
            if (depart == null)
            {
                return false;
            }
            context.Remove(depart);
            context.SaveChanges();
            return true;
        }

        public bool EditDepart(Department req)
        {
            var department = GetDepartDetail(req.Id);
            if(department == null)
            {
                return false;
            }
            department.Name = req.Name;
            department.Description = req.Description;
            context.SaveChanges();
            return true;
        }

        public IQueryable<Department> GetAllDepartment()
        {
            var depart = context.Departments.Select(x => x);
            return depart;
        }

        public Department GetDepartDetail(int? id)
        {
            var depart = context.Departments.FirstOrDefault(x => x.Id == id);
            return depart;
        }
    }
}
