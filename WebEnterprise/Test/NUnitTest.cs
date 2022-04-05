using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebEnterprise.Data;
using WebEnterprise.Models;

namespace WebEnterprise.Test
{
    [TestClass]
    public class NUnitTest
    {
        private readonly UserManager<CustomUser> userManager;
        private readonly ApplicationDbContext context;

        public NUnitTest(UserManager<CustomUser> _userManager, ApplicationDbContext _context)
        {
            userManager = _userManager;
            context = _context;
        }


        [TestMethod]
        public async void TestAddUser()
        {
            var result = new IdentityResult();

            using (var trans = context.Database.BeginTransaction())
            {
                var account = new CustomUser
                {
                    UserName = "test case 1",
                    FullName = "test case 1",
                    Email = "testcase1@gmailcom",
                };

                result = await userManager.CreateAsync(account, "Abc@12345");

                trans.Rollback();
            }

            Assert.IsTrue(result.Succeeded);
        }
    }
}
