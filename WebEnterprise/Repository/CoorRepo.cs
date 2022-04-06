using Microsoft.AspNetCore.Identity;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.DTO;
using WebEnterprise.Repository.Interfaces;

namespace WebEnterprise.Repository
{
    public class CoorRepo : ICoorRepo
    {

        private readonly UserManager<CustomUser> userManager;
        private readonly ApplicationDbContext context;

        public CoorRepo(UserManager<CustomUser> _userManager, ApplicationDbContext _context)
        {
            userManager = _userManager;
            context = _context;
        }

        public async Task<CustomUser> AddCoor(string userName, string fullName, string email, List<Department> departs)
        {
            var account = new CustomUser
            {
                UserName = userName,
                FullName = fullName,
                Email = email,
            };
            foreach (var d in departs)
            {
                account.DepartmentId = d.Id;
            }
            if (account.UserName == null || account.FullName == null || account.Email == null || account.DepartmentId == null)
            {
                return null;
            }
            var result = await userManager.CreateAsync(account, "Abc@12345");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(account, "Coordinator"); 
                return account;
            }
            return null;
        }

        public bool DeleteCoor(string id)
        {
            var coor = GetCoorById(id);
            if (coor != null)
            {
                context.Remove(coor);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public CustomUser EditCoor(UserDTO res)
        {
            var coor = GetCoorById(res.Id);
            if (coor != null)
            {
                coor.Email = res.Email;
                coor.FullName = res.FullName;
                coor.UserName = res.UserName;
                coor.PhoneNumber = res.PhoneNumber;
                coor.DepartmentId = res.DepartId;
                context.SaveChanges();
                return coor;
            }
            return null;
        }

        public IQueryable<UserDTO> GetAllCoor()
        {
            var coors = from u in context.Users
                             join ur in context.UserRoles on u.Id equals ur.UserId
                             join r in context.Roles on ur.RoleId equals r.Id
                             where r.Name == "Coordinator"
                             select new UserDTO
                             {
                                 Id = u.Id,
                                 FullName = u.FullName,
                                 UserName = u.UserName,
                                 Email = u.Email
                             };
            if (coors != null)
            {
                return coors;
            }
            return null;
        }

        public CustomUser GetCoorById(string id)
        {
            var coor = context.Users.FirstOrDefault(s => s.Id == id);
            return coor;
        }

        public UserDTO GetEditCoor(string id)
        {
            var coor = context.Users.Where(u => u.Id == id).Select(u => new UserDTO
            {
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                UserName = u.UserName
            }).FirstOrDefault();
            return coor;
        }
    }
}
