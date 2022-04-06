using Microsoft.AspNetCore.Identity;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.DTO;
using WebEnterprise.Repository.Interfaces;

namespace WebEnterprise.Repository
{
    public class AssuranceRepo : IAssuranceRepo
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly ApplicationDbContext context;

        public AssuranceRepo(UserManager<CustomUser> _userManager, ApplicationDbContext _context)
        {
            userManager = _userManager;
            context = _context;
        }

        public async Task<CustomUser> AddAssurance(string userName, string fullName, string email, List<Department> departs)
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
            if(account.UserName == null || account.FullName == null || account.Email == null || account.DepartmentId == null)
            {
                return null;
            }
            var result = await userManager.CreateAsync(account, "Abc@12345");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(account, "Assurance");
                return account;
            }
            return null;
        }

        public bool DeleteAssurance(string id)
        {
            var assurance = GetAssuranceById(id);
            if (assurance != null)
            {
                context.Remove(assurance);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public CustomUser EditAssurance(UserDTO res)
        {
            var assurance = GetAssuranceById(res.Id);
            if (assurance != null)
            {
                assurance.Email = res.Email;
                assurance.FullName = res.FullName;
                assurance.UserName = res.UserName;
                assurance.PhoneNumber = res.PhoneNumber;
                assurance.DepartmentId = res.DepartId;
                context.SaveChanges();
                return assurance;
            }
            return null;
        }

        public IQueryable<UserDTO> GetAllAssurances()
        {
            var assurances = from u in context.Users
                             join ur in context.UserRoles on u.Id equals ur.UserId
                             join r in context.Roles on ur.RoleId equals r.Id
                             where r.Name == "Assurance"
                             select new UserDTO
                             {
                                 Id = u.Id,
                                 FullName = u.FullName,
                                 UserName = u.UserName,
                                 Email = u.Email
                             };
            if (assurances != null)
            {
                return assurances;
            }
            return null;
        }

        public CustomUser GetAssuranceById(string id)
        {
            var assurance = context.Users.FirstOrDefault(s => s.Id == id);
            return assurance;
        }

        public UserDTO GetEditAssurance(string id)
        {
            var assurance = context.Users.Where(u => u.Id == id).Select(u => new UserDTO
            {
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                UserName = u.UserName
            }).FirstOrDefault();
            return assurance;
        }
    }
}
