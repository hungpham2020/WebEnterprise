using WebEnterprise.Models;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Repository.Interfaces
{
    public interface IStaffRepo
    {
        public IQueryable<UserDTO> GetAllStaffs();

        public Task<CustomUser> AddStaff(UserAddDTO user);

        public UserDTO GetEditStaff(string id);

        public CustomUser GetStaffById(string id);

        public CustomUser EditStaff(UserDTO res);

        public bool DeleteStaff(string id);
    }
}
