using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using WebEnterprise.Data;
using WebEnterprise.Models;
using WebEnterprise.Models.DTO;
using WebEnterprise.Repository;

namespace WebEnterprise.Test
{
    [TestFixture]
    public class NUnitTest
    {
        private static AssuranceRepo AssuranRepo;
        private static StaffRepo staffRepo;
        private static CoorRepo coorRepo;
        private static Mock<UserManager<CustomUser>> userManager;
        private static ApplicationDbContext context;

        [SetUp]
        public void Initialize()
        {
            userManager = new Mock<UserManager<CustomUser>>(
                new Mock<IUserStore<CustomUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<CustomUser>>().Object,
                new IUserValidator<CustomUser>[0],
                new IPasswordValidator<CustomUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<CustomUser>>>().Object);
            context = new ApplicationDbContext();

            userManager.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new CustomUser()));
            userManager.Setup(userManager => userManager.IsInRoleAsync(It.IsAny<CustomUser>(), "Assurance"))
                .ReturnsAsync(true);
            userManager.Setup(userManager => userManager.IsInRoleAsync(It.IsAny<CustomUser>(), "Staff"))
                .ReturnsAsync(true);
            userManager.Setup(userManager => userManager.IsInRoleAsync(It.IsAny<CustomUser>(), "Coordinator"))
                .ReturnsAsync(true);
            userManager.Setup(userManager => userManager.CreateAsync(It.IsAny<CustomUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            AssuranRepo = new AssuranceRepo(userManager.Object, context);
            staffRepo = new StaffRepo(userManager.Object, context);
            coorRepo = new CoorRepo(userManager.Object, context);
        }

        #region ArrsuranceTest

        [Test]
        public void GetAllAssuranceTest()
        {
            var result = AssuranRepo.GetAllAssurances();

            Assert.IsTrue(result != null);
        }

        [Test]
        public async Task TestAddAssurance()
        {
            // var assuranceRepo = new Mock<IAssuranceRepo>();
            var account = new UserAddDTO
            {
                UserName = "TestCase1",
                FullName = "Test Case 1",
                Email = "testcase1@gmail.com",
                DepartId = 1
            };

            var result = await AssuranRepo.AddAssurance(account);

            Assert.IsTrue(result != null);
        }

        [Test]
        public async Task TestAddAssuranceFail()
        {
            var account = new UserAddDTO
            {
                UserName = "TestCase1",
                FullName = "Test Case 1",
            };

            var result = await AssuranRepo.AddAssurance(account);

            Assert.IsFalse(result != null);
        }

        [Test]
        public void TestEditAssurance()
        {
            var result = new CustomUser();
            using (var trans = context.Database.BeginTransaction())
            {
                var user = new UserDTO
                {
                    Id = "60395add-292d-4224-8d54-f9376bba5e66",
                    UserName = "Student6",
                    FullName = "Student 7",
                    Email = "student6@gmail.com",
                    DepartId = 1,
                };
                result = AssuranRepo.EditAssurance(user);

                trans.Rollback();
            }

            Assert.IsTrue(result != null);
        }

        [Test]
        public void TestEditAssuranceFail()
        {
            var user = new UserDTO
            {
                Id = "1234",
                UserName = "Student6",
                FullName = "Student 7",
                Email = "student6@gmail.com",
                DepartId = 1,
            };
            var result = AssuranRepo.EditAssurance(user);

            Assert.IsFalse(result != null);
        }

        [Test]
        public void TestDeleteAssurance()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                string id = "60395add-292d-4224-8d54-f9376bba5e66";
                result = AssuranRepo.DeleteAssurance(id);

                trans.Rollback();
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void TestDeleteAssuranceFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                string id = "1234";
                result = AssuranRepo.DeleteAssurance(id);

                trans.Rollback();
            }

            Assert.IsFalse(result);
        }

        #endregion

        #region StaffTest

        [Test]
        public void GetAllStaffTest()
        {
            var result = staffRepo.GetAllStaffs();

            Assert.IsTrue(result != null);
        }

        [Test]
        public async Task TestAddStaff()
        {
            var account = new UserAddDTO
            {
                UserName = "TestCase2",
                FullName = "Test Case 2",
                Email = "testcase1@gmail.com",
                DepartId = 1
            };

            var result = await staffRepo.AddStaff(account);

            Assert.IsTrue(result != null);
        }

        [Test]
        public async Task TestAddStaffFail()
        {
            var account = new UserAddDTO
            {
                UserName = "TestCase2",
                FullName = "Test Case 2",
                DepartId = 1
            };

            var result = await staffRepo.AddStaff(account);

            Assert.IsFalse(result != null);
        }

        [Test]
        public void TestEditStaff()
        {
            var result = new CustomUser();
            using (var trans = context.Database.BeginTransaction())
            {
                var user = new UserDTO
                {
                    Id = "60395add-292d-4224-8d54-f9376bba5e66",
                    UserName = "Student6",
                    FullName = "Student 7",
                    Email = "student6@gmail.com",
                    DepartId = 1,
                };
                result = staffRepo.EditStaff(user);

                trans.Rollback();
            }

            Assert.IsTrue(result != null);
        }

        [Test]
        public void TestEditStaffFail()
        {
            var user = new UserDTO
            {
                Id = "1234",
                UserName = "Student6",
                FullName = "Student 7",
                Email = "student6@gmail.com",
                DepartId = 1,
            };
            var result = staffRepo.EditStaff(user);

            Assert.IsFalse(result != null);
        }

        [Test]
        public void TestDeleteStaff()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                string id = "60395add-292d-4224-8d54-f9376bba5e66";
                result = staffRepo.DeleteStaff(id);

                trans.Rollback();
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void TestDeleteStaffFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                string id = "1234";
                result = staffRepo.DeleteStaff(id);

                trans.Rollback();
            }

            Assert.IsFalse(result);
        }

        #endregion

        #region CoordinatorTest

        [Test]
        public void GetAllCoorTest()
        {
            var result = coorRepo.GetAllCoor();

            Assert.IsTrue(result != null);
        }

        [Test]
        public async Task TestAddCoor()
        {
            var account = new UserAddDTO
            {
                UserName = "TestCase2",
                FullName = "Test Case 2",
                Email = "testcase1@gmail.com",
                DepartId = 1
            };

            var result = await coorRepo.AddCoor(account);

            Assert.IsTrue(result != null);
        }

        [Test]
        public async Task TestAddCoorFail()
        {
            var account = new UserAddDTO
            {
                UserName = "TestCase2",
                FullName = "Test Case 2",
            };

            var deparments = new List<Department>() { new Department() { Id = 1 } };

            var result = await coorRepo.AddCoor(account);

            Assert.IsFalse(result != null);
        }

        [Test]
        public void TestEditCoor()
        {
            var result = new CustomUser();
            using (var trans = context.Database.BeginTransaction())
            {
                var user = new UserDTO
                {
                    Id = "60395add-292d-4224-8d54-f9376bba5e66",
                    UserName = "Student6",
                    FullName = "Student 7",
                    Email = "student6@gmail.com",
                    DepartId = 1,
                };
                result = coorRepo.EditCoor(user);

                trans.Rollback();
            }

            Assert.IsTrue(result != null);
        }

        [Test]
        public void TestEditCoorFail()
        {
            var user = new UserDTO
            {
                Id = "1234",
                UserName = "Student6",
                FullName = "Student 7",
                Email = "student6@gmail.com",
                DepartId = 1,
            };
            var result = coorRepo.EditCoor(user);

            Assert.IsFalse(result != null);
        }

        [Test]
        public void TestDeleteCoor()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                string id = "60395add-292d-4224-8d54-f9376bba5e66";
                result = coorRepo.DeleteCoor(id);

                trans.Rollback();
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void TestDeleteCoorFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                string id = "1234";
                result = coorRepo.DeleteCoor(id);

                trans.Rollback();
            }

            Assert.IsFalse(result);
        }

        #endregion

        #region PostTest
        #endregion

        #region CategoryTest
        #endregion

        #region DepartmentTest
        #endregion
    }
}
