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
        private static CatRepo catRepo;
        private static DepartmentRepo departmentRepo;
        private static PostRepo postRepo;
        private static UserRepo userRepo;
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
            catRepo = new CatRepo(context);
            departmentRepo = new DepartmentRepo(context);
            postRepo = new PostRepo(context);
            userRepo = new UserRepo(context);
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
                    Id = "b5562f73-74ea-4596-9856-fde7f75ae009",
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
                string id = "b5562f73-74ea-4596-9856-fde7f75ae009";
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
                    Id = "b5562f73-74ea-4596-9856-fde7f75ae009",
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
                string id = "b5562f73-74ea-4596-9856-fde7f75ae009";
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
                    Id = "b5562f73-74ea-4596-9856-fde7f75ae009",
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
                string id = "b5562f73-74ea-4596-9856-fde7f75ae009";
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

        [Test]
        public void GetAllPostTest()
        {
            var result = postRepo.GetAllPost();

            Assert.IsTrue(result != null);
        }

        [Test]
        public void AddPostTest()
        {
            bool result;
            using(var trans = context.Database.BeginTransaction())
            {
                var test = new PostDTO
                {
                    Title = "title123",
                    Description = "des12345",
                    CatId = 1,
                };
                result = postRepo.AddPost(test, "1");

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void AddPostTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var test = new PostDTO
                {
                    Title = "title123",
                    Description = "des12345",
                    CatId = 1,
                };
                var authorId = "";
                result = postRepo.AddPost(test, authorId);

                trans.Rollback();
            }
            Assert.IsFalse(result);
        }

        [Test]
        public void EditPostTest()
        {
            bool result;
            using(var trans = context.Database.BeginTransaction())
            {
                var test = new PostDTO
                {
                    Id = 4,
                    Title = "Test12345",
                    Description = "test12345",
                    CatId = 1,
                    OpenDate = DateTime.Now,
                    ClosedDate = DateTime.Now.AddDays(3),
                };
                result = postRepo.EditPost(test);

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void EditPostTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var test = new PostDTO
                {
                    Id = 0,
                    Title = "Test12345",
                    Description = "test12345",
                    CatId = 1,
                    OpenDate = DateTime.Now,
                    ClosedDate = DateTime.Now.AddDays(3),
                };
                result = postRepo.EditPost(test);

                trans.Rollback();
            }
            Assert.IsFalse(result);
        }

        [Test]
        public void DeletePostTest()
        {
            bool result;
            using(var trans = context.Database.BeginTransaction())
            {
                int id = 4;
                result = postRepo.DeletePost(id);
                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void DeletePostTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                int id = 0;
                result = postRepo.DeletePost(id);
                trans.Rollback();
            }
            Assert.IsFalse(result);
        }

        #endregion

        #region CategoryTest

        [Test]
        public void GetAllCatTest()
        {
            var result = catRepo.GetAllCategory();
            Assert.IsTrue(result != null);
        }

        [Test]
        public void AddCatTest()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var cat = new CatDTO
                {
                    CatName = "TestCat1",
                    Description = "TestDes1"
                };
                result = catRepo.AddCat(cat);

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void AddCatTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var cat = new CatDTO
                {
                    CatName = "TestCat1",
                };
                result = catRepo.AddCat(cat);

                trans.Rollback();
            }
            Assert.IsFalse(result);
        }

        [Test]
        public void EditCatTest()
        {
            bool result;
            using(var trans = context.Database.BeginTransaction())
            {
                var test = new Category
                {
                    Id = 2,
                    Name = "Coding",
                    Description = "Coding"
                };
                result = catRepo.EditCat(test);

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void EditCatTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var test = new Category
                {
                    Id = 0,
                    Name = "Coding",
                    Description = "Coding"
                };
                result = catRepo.EditCat(test);

                trans.Rollback();
            }
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteCatTest()
        {
            bool result;
            using(var trans = context.Database.BeginTransaction())
            {
                int id = 2;
                result = catRepo.DeleteCat(id);

                trans.Rollback();
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteCatTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                int id = 0;
                result = catRepo.DeleteCat(id);

                trans.Rollback();
            }

            Assert.IsFalse(result);
        }

        #endregion

        #region DepartmentTest

        [Test]
        public void GetAllDepartTest()
        {
            var result = departmentRepo.GetAllDepartment();
            Assert.IsTrue(result != null);
        }

        [Test]
        public void AddDepartTest()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var depart = new DepartDTO
                {
                    DepartName = "TestCat1",
                    Description = "TestDes1"
                };
                result = departmentRepo.AddDepart(depart);

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void AddDepartTesFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var depart = new DepartDTO
                {
                    Description = "TestDes1"
                };
                result = departmentRepo.AddDepart(depart);

                trans.Rollback();
            }
            Assert.IsFalse(result);
        }

        [Test]
        public void EditDepartTest()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var test = new Department
                {
                    Id = 2,
                    Name = "IT",
                    Description = "Name"
                };
                result = departmentRepo.EditDepart(test);

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void EditDepartTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var test = new Department
                {
                    Id = 0,
                    Name = "IT",
                    Description = "Coding"
                };
                result = departmentRepo.EditDepart(test);

                trans.Rollback();
            }
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteDepartTest()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                int id = 2;
                result = departmentRepo.DeleteDepart(id);

                trans.Rollback();
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteDepartTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                int id = 0;
                result = departmentRepo.DeleteDepart(id);

                trans.Rollback();
            }

            Assert.IsFalse(result);
        }

        #endregion

        #region UserTest

        [Test]
        public void GetAllUserPostTest()
        {
            var result = userRepo.GetAllPost("1");
            Assert.IsTrue(result != null);
        }

        [Test]
        public void GetUserPostTest()
        {
            var result = userRepo.GetUserPost("1");
            Assert.IsTrue(result != null);
        }

        [Test]
        public void GetPostDetail()
        {
            var result = userRepo.GetPostDetail(4);
            Assert.IsTrue(result != null);
        }

        [Test]
        public void GetPostDetailFail()
        {
            var result = userRepo.GetPostDetail(0);
            Assert.IsFalse(result != null);
        }

        [Test]
        public void GetPostForComment()
        {
            var result = userRepo.GetPostForComment(4, "1");
            Assert.IsTrue(result != null);
        }

        [Test]
        public void AddPostUserTest()
        {
            bool result;
            using(var trans = context.Database.BeginTransaction())
            {
                var test = new PostDTO
                {
                    Title = "Test123",
                    Description = "Test123",
                    CatId = 1,
                };
                result = userRepo.AddPost(test, "1");

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void AddPostUserTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var test = new PostDTO
                {
                    Title = "Test123",
                    CatId = 1,
                };
                result = userRepo.AddPost(test, "1");

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void EditPostUserTest()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var test = new PostDTO
                {
                    Id = 4,
                    Title = "Test123",
                    Description = "Test123",
                    OpenDate = DateTime.Now,
                    ClosedDate = DateTime.Now.AddDays(3),
                    CatId = 1,
                };
                result = userRepo.EditPost(test);

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void EditPostUserTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var test = new PostDTO
                {
                    Id = 0,
                    Title = "Test123",
                    Description = "Test123",
                    OpenDate = DateTime.Now,
                    ClosedDate = DateTime.Now.AddDays(3),
                    CatId = 1,
                };
                result = userRepo.EditPost(test);

                trans.Rollback();
            }
            Assert.IsFalse(result);
        }

        [Test]
        public void DeletePostUserTest()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                int id = 4;
                result = userRepo.DeletePost(id);

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }
        
        [Test]
        public void DeletePostUserTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                int id = 0;
                result = userRepo.DeletePost(id);

                trans.Rollback();
            }
            Assert.IsFalse(result);
        }

        [Test]
        public void LikeTest()
        {
            UserLikePost result;
            using (var trans = context.Database.BeginTransaction())
            {
                var like = new UserLikePost
                {
                    UserId = "1",
                    PostId = 1,
                };

                result = userRepo.Like(like);
                trans.Rollback();
            }
            Assert.IsTrue(result.Status);
        }

        [Test]
        public void DisLikeTest()
        {
            UserLikePost result;
            using (var trans = context.Database.BeginTransaction())
            {
                var like = new UserLikePost
                {
                    UserId = "1",
                    PostId = 1,
                };

                result = userRepo.Dislike(like);
                trans.Rollback();
            }
            Assert.IsTrue(!result.Status);
        }

        [Test]
        public void AddCommentTest()
        {
            bool result;
            using(var trans = context.Database.BeginTransaction())
            {
                var comment = new CommentDTO
                {
                    PostId = 1,
                    Description = "test123"
                };
                result = userRepo.AddComment(comment, "1");

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void GetAllCommentTest()
        {
            var result = userRepo.GetAllComment(1);
            Assert.IsTrue(result != null);
        }

        [Test]
        public void GetAllCommentTestFail()
        {
            var result = userRepo.GetAllComment(0);
            Assert.IsFalse(result != null);
        }

        [Test]
        public void EditCommentTest()
        {
            bool result;
            using(var trans = context.Database.BeginTransaction())
            {
                var test = new CommentDTO
                {
                    CommentId = 10,
                    Description = "Test123"
                };
                result = userRepo.EditComment(test, "1");

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void EditCommentTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                var test = new CommentDTO
                {
                    CommentId = 0,
                    Description = "Test123"
                };
                result = userRepo.EditComment(test, "1");

                trans.Rollback();
            }
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteCommnetTest()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                int id = 10;
                result = userRepo.DeleteComment(id, "1");

                trans.Rollback();
            }
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteCommnetTestFail()
        {
            bool result;
            using (var trans = context.Database.BeginTransaction())
            {
                int id = 0;
                result = userRepo.DeleteComment(id, "1");

                trans.Rollback();
            }
            Assert.IsFalse(result);
        }

        #endregion
    }
}
