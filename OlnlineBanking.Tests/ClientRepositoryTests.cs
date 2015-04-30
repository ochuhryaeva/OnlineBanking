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
    public class ClientRepositoryTests
    {
        private Mock<IDbSet<Client>> _dbSetClientMock;
        private Mock<ClientDbContext> _clientDbContextMock;

        private IQueryable<Client> GetClients()
        {
            var clients = new List<Client> 
            { 
                new Client() { Id = 1, ContractNumber = "11111"},
                new Client() { Id = 2, ContractNumber = "22222" },
                new Client() { Id = 3, ContractNumber = "333333" },
                new Client() { Id = 4, ContractNumber = "44444"}
            }.AsQueryable();
            return clients;
        }

        [SetUp]
        public void SetUp()
        {
            var clients = GetClients();
            _dbSetClientMock = new Mock<IDbSet<Client>>();
            _dbSetClientMock.Setup(m => m.Provider).Returns(clients.Provider);
            _dbSetClientMock.Setup(m => m.Expression).Returns(clients.Expression);
            _dbSetClientMock.Setup(m => m.ElementType).Returns(clients.ElementType);
            _dbSetClientMock.Setup(m => m.GetEnumerator()).Returns(clients.GetEnumerator());

            _clientDbContextMock = new Mock<ClientDbContext>();
            _clientDbContextMock.Setup(x => x.Clients).Returns(_dbSetClientMock.Object);   
        }

        
        [Test]
        public void Delete_ForCustomClientId_CallRemoveAndSaveChanges()
        {
            //Arrange
            int clientId = 1;
            Client clientForDelete = GetClients().FirstOrDefault(c => c.Id == clientId);
            _dbSetClientMock.Setup(x => x.Find(clientId)).Returns(clientForDelete);
            var classUnderTest = new ClientRepository(_clientDbContextMock.Object);

            //Action
            classUnderTest.DeleteClient(clientId);
            _dbSetClientMock.Verify(u => u.Remove(clientForDelete));
            _clientDbContextMock.Verify(c=>c.SaveChanges());
        }

        [Test]
        public void SaveClient_ForNewClient_CallAddAndSaveChanges()
        {
            //Arrange
            var classUnderTest = new ClientRepository(_clientDbContextMock.Object);
            Client clientNew = new Client(){ContractNumber = "333333",LastName = "new client",FirstName = "new client"};

            //Action
            classUnderTest.SaveClient(clientNew);
            _dbSetClientMock.Verify(u => u.Add(clientNew));
            _clientDbContextMock.Verify(c => c.SaveChanges());
        }

        [Test]
        public void SaveClient_ForEditClient_EditAndSaveChanges()
        {
            //Arrange
            int clientId = 1;
            Client clientEdit = GetClients().FirstOrDefault(c => c.Id == clientId);
            _dbSetClientMock.Setup(x => x.Find(clientId)).Returns(clientEdit);
            var classUnderTest = new ClientRepository(_clientDbContextMock.Object);

            //Action
            int id = classUnderTest.SaveClient(clientEdit);
            _clientDbContextMock.Verify(c => c.SaveChanges());
            Assert.AreEqual(clientId,id);
        }
    }
}
