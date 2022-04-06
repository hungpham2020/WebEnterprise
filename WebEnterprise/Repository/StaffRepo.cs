using Microsoft.AspNetCore.Identity;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.DTO;
using WebEnterprise.Repository.Interfaces;

namespace WebEnterprise.Repository
{
    public class StaffRepo : IStaffRepo
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly ApplicationDbContext context;

        public StaffRepo(UserManager<CustomUser> _userManager, ApplicationDbContext _context)
        {
            userManager = _userManager;
            context = _context;
        }
        public async Task<CustomUser> AddStaff(string userName, string fullName, string email, List<Department> departs)
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
                await userManager.AddToRoleAsync(account, "Staff");
                return account;
            }
            return null;
        }

        public bool DeleteStaff(string id)
        {
            var staff = GetStaffById(id);
            if (staff != null)
            {
                context.Remove(staff);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public CustomUser EditStaff(UserDTO res)
        {
            var staff = GetStaffById(res.Id);
            if(staff != null)
            {
                staff.Email = res.Email;
                staff.UserName = res.UserName;
                staff.FullName = res.FullName;
                staff.PhoneNumber = res.PhoneNumber;
                staff.DepartmentId = res.DepartId;
                context.SaveChanges();
                return staff;
            };
            return null;
        }

        public IQueryable<UserDTO> GetAllStaffs()
        {
            var staffs = from u in context.Users
                         join ur in context.UserRoles on u.Id equals ur.UserId
                         join r in context.Roles on ur.RoleId equals r.Id
                         where r.Name == "Staff"
                         select new UserDTO
                         {
                             Id = u.Id,
                             FullName = u.FullName,
                             UserName = u.UserName,
                             Email = u.Email,
                         };
            return staffs;
        }

        public UserDTO GetEditStaff(string id)
        {
            var staff = context.Users.Where(u => u.Id == id).Select(u => new UserDTO
            {
                UserName = u.UserName,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber
            }).FirstOrDefault();
            return staff;
        }

        public CustomUser GetStaffById(string id)
        {
            var staff = context.Users.FirstOrDefault(s => s.Id == id);
            return staff;
        }
    }
}
