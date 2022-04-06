using WebEnterprise.Models;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Repository.Interfaces
{
    public interface ICoorRepo
    {
        public IQueryable<UserDTO> GetAllCoor();

        public Task<CustomUser> AddCoor(string userName, string fullName, string email, List<Department> departs);

        public UserDTO GetEditCoor(string id);

        public CustomUser GetCoorById(string id);

        public CustomUser EditCoor(UserDTO res);

        public bool DeleteCoor(string id);
    }
}
