using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;
using NUnit.Framework;
using OlnlineBanking.Infrastructure.Concrete;
using OlnlineBanking.Models;

namespace OlnlineBanking.Tests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private Mock<IDbSet<User>> _dbSetUserMock;
        private Mock<UserDbContext> _userDbContextMock;

        private IQueryable<User> GetUsers()
        {
            var users = new List<User> 
            { 
                new User() { Id = 1, Login = "login1"},
                new User() { Id = 2, Login = "login2"}
            }.AsQueryable();
            return users;
        }

        [SetUp]
        public void SetUp()
        {
            var users = GetUsers();
            _dbSetUserMock = new Mock<IDbSet<User>>();
            _dbSetUserMock.Setup(m => m.Provider).Returns(users.Provider);
            _dbSetUserMock.Setup(m => m.Expression).Returns(users.Expression);
            _dbSetUserMock.Setup(m => m.ElementType).Returns(users.ElementType);
            _dbSetUserMock.Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _userDbContextMock = new Mock<UserDbContext>();
            _userDbContextMock.Setup(x => x.Users).Returns(_dbSetUserMock.Object);
        }

        [Test]
        public void SaveUser_ForNewClient_CallAddAndSaveChanges()
        {
            //Arrange
            var classUnderTest = new UserRepository(_userDbContextMock.Object);
            User userNew = new User() { Login = "new", Password = "new" };

            //Action
            classUnderTest.SaveUser(userNew);
            _dbSetUserMock.Verify(u => u.Add(userNew));
            _userDbContextMock.Verify(c => c.SaveChanges());
        }

        [Test]
        public void SaveUser_ForEditUser_EditAndSaveChanges()
        {
            //Arrange
            int userId = 1;
            User userEdit = GetUsers().FirstOrDefault(c => c.Id == userId);
            _dbSetUserMock.Setup(x => x.Find(userId)).Returns(userEdit);
            var classUnderTest = new UserRepository(_userDbContextMock.Object);

            //Action
            classUnderTest.SaveUser(userEdit);
            _userDbContextMock.Verify(c => c.SaveChanges());
        }

    }
}
