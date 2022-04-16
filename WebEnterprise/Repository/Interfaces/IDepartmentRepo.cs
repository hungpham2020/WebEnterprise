using WebEnterprise.Models;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Repository.Interfaces
{
    public interface IDepartmentRepo
    {
        public IQueryable<Department> GetAllDepartment();

        public Department GetDepartDetail(int? id);

        public bool AddDepart(DepartDTO req);

        public bool DeleteDepart(int id);

        public bool EditDepart(Department req);
    }
}
