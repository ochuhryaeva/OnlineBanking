using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using OlnlineBanking.Controllers;
using OlnlineBanking.HtmlHelpers;
using OlnlineBanking.Infrastructure.Abstract;
using OlnlineBanking.Models;
using Assert = NUnit.Framework.Assert;

namespace OlnlineBanking.Tests
{
    [TestFixture]
    public class ClientControllerTests
    {
        private Mock<IClientRepository> _clientRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _clientRepositoryMock.Setup(m => m.Clients).Returns(new Client[] {
                new Client {Id = 1, ContractNumber = "P1", LastName = "One", FirstName = "One", DateOfBirth = new DateTime(1979,1,1), Phone = "767889"},
                new Client {Id = 2, ContractNumber = "P2", LastName = "Two", FirstName = "Two", DateOfBirth = new DateTime(1979,4,1), Phone = "789989"},
                new Client {Id = 3, ContractNumber = "P3", LastName = "Three", FirstName = "Three", DateOfBirth = new DateTime(1979,7,1), Phone = "11111"},
                new Client {Id = 4, ContractNumber = "P4", LastName = "Four", FirstName = "Four", DateOfBirth = new DateTime(1980,1,1), Phone = "45757"},
                new Client {Id = 5, ContractNumber = "P5", LastName = "Five",FirstName = "Five", DateOfBirth = new DateTime(1976,1,1), Phone = "99"}
            });
        }
        
        private ClientController GetClientController()
        {
            return new ClientController(_clientRepositoryMock.Object);
        }

        [Test]
        public void Index_ForCustomPage_CanPaginate()
        {
            // Arrange
            ClientController controller = GetClientController();
            controller.PageSize = 3;

            // Act  
            ClientListViewModel result = (ClientListViewModel)((ViewResult)controller.Index(2)). Model;
            
            // Assert
            Client[] clients= result.Clients.ToArray();
            Assert.IsTrue(clients.Length == 2);
            Assert.AreEqual(clients[0].ContractNumber, "P4");
            Assert.AreEqual(clients[1].ContractNumber, "P5");
        }

        [Test]
        public void Index_ForCustomPage_ReturnCorrectPagingInfo()
        {

            // Arrange
            ClientController controller = GetClientController();
            controller.PageSize = 3;

            // Act
            ClientListViewModel result = (ClientListViewModel)((ViewResult)controller.Index(2)).Model;

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [Test]
        public void Index_ForCustomSortingAsc_ReturnCorrectSortedInfo()
        {
            // Arrange
            ClientController controller = GetClientController();
            // Act
            foreach (var property in typeof(Client).GetProperties())
            {
                if (property.Name.ToLower() != "id")
                {
                    string propertyName = property.Name;
                    ClientListViewModel result = (ClientListViewModel)((ViewResult)controller.Index(1, propertyName, Ordering.Asc)).Model;
                    // Assert
                    SortedInfo sortedInfo = result.SortedInfo;
                    Assert.AreEqual(propertyName, sortedInfo.SortedField);
                    Assert.AreEqual(Ordering.Asc, sortedInfo.SortedOrder);
    
                }
            }
        }
     
        [Test]
        public void Index_ForCustomSortingDesc_ReturnCorrectSortedInfo()
        {
            ClientController controller = GetClientController();
            // Act
            foreach (var property in typeof(Client).GetProperties())
            {
                if (property.Name.ToLower() != "id")
                {
                    string propertyName = property.Name;
                    ClientListViewModel result = (ClientListViewModel)((ViewResult)controller.Index(1, propertyName, Ordering.Desc)).Model;
                    // Assert
                    SortedInfo sortedInfo = result.SortedInfo;
                    Assert.AreEqual(propertyName, sortedInfo.SortedField);
                    Assert.AreEqual(Ordering.Desc, sortedInfo.SortedOrder);

                }
            }
        }

        [Test]
        public void Index_ForContractNumberSortDesc_ReturnCorrectClientListViewModel()
        {
            // Arrange
            ClientController controller = GetClientController();
            controller.PageSize = 3;
            // Act
            ClientListViewModel result = (ClientListViewModel)((ViewResult)controller.Index(2, "ContractNumber", Ordering.Desc)).Model;
            Client[] clients = result.Clients.ToArray();
            Assert.IsTrue(clients.Length == 2);
            Assert.AreEqual(clients[0].ContractNumber, "P2");
            Assert.AreEqual(clients[1].ContractNumber, "P1");
        }

        [Test]
        public void Index_ForContractNumberSortAsc_ReturnCorrectClientListViewModel()
        {
            // Arrange
            ClientController controller = GetClientController();
            controller.PageSize = 3;
            // Act
            ClientListViewModel result = (ClientListViewModel)((ViewResult)controller.Index(2, "ContractNumber", Ordering.Asc)).Model;
            Client[] clients = result.Clients.ToArray();
            Assert.IsTrue(clients.Length == 2);
            Assert.AreEqual(clients[0].ContractNumber, "P4");
            Assert.AreEqual(clients[1].ContractNumber, "P5");
        }

        [Test]
        public void PagingHelper_ForCustomPagingInfo_GeneratePageLinks()
        {
            // Arrange
            HtmlHelper myHelper = null;

            // Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            // Arrange - set up the delegate using a lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            // Act
            MvcHtmlString result = myHelper.Pages(pagingInfo, pageUrlDelegate);

            // Assert           
            Assert.AreEqual(@"<a href=""Page1"">1</a>"
                + @"<a class=""active"" href=""Page2"">2</a>"
                + @"<a href=""Page3"">3</a>",
                result.ToString());
   
        }
        

        [Test]
        public void Edit_ForExistingClient_ReturnEditViewWithCorrectModel()
        {

            // Arrange - create the mock repository
            ClientController controller = GetClientController();
            controller.PageSize = 3;

            // Act
            Client client1 = ((ViewResult)controller.Edit(1)).Model as Client;
            Client client2 = ((ViewResult)controller.Edit(2)).Model as Client;
            Client client3 = ((ViewResult)controller.Edit(3)).Model as Client;

            // Assert
            Assert.AreEqual(1, client1.Id);
            Assert.AreEqual(2, client2.Id);
            Assert.AreEqual(3, client3.Id);
        }

        [Test]
        public void Edit_ForNotExistingClient_ReturnNull()
        {
            // Arrange - create the mock repository
            ClientController controller = GetClientController();
            controller.PageSize = 3;

            // Act
            Client client = ((ViewResult)controller.Edit(10)).Model as Client;
            
            // Assert
            Assert.AreEqual(client, null);
        }

        [Test]
        public void Add_ForNewClient_ReturnViewWithCorrectModel()
        {
            // Arrange - create the mock repository
            ClientController controller = GetClientController();
            controller.PageSize = 3;

            // Act
            Client client = ((ViewResult)controller.Add()).Model as Client;

            // Assert
            Assert.AreEqual(client.Id, 0);
        }

        [Test]
        public void Edit_IfModelIsValid_SaveChanges()
        {

            // Arrange - create mock repository
            ClientController controller = GetClientController();
            controller.PageSize = 3;
            
            Client client = new Client()
            {
                ContractNumber = "P12",
                LastName = "Last", 
                FirstName = "First", 
                DateOfBirth = new DateTime(1976,7,7),
                Deposit = false,Phone = "4343",
                Status = ClientStatus.Classic
            };

            // Act - try to save the product
            ActionResult result = controller.Edit(client,"returnUrl");

            // Assert - check that the repository was called
            _clientRepositoryMock.Verify(m => m.SaveClient(client));
            // Assert - check the method result type
            Assert.IsNotInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Edit_IfModelIsInvalid_NotSaveChanges()
        {

            // Arrange - create mock repository
            ClientController controller = GetClientController();
            controller.PageSize = 3;

            Client client = new Client()
            {
                ContractNumber = "P12",
                LastName = "Last",
                FirstName = "First",
                DateOfBirth = new DateTime(1976, 7, 7),
                Deposit = false,
                Phone = "4343",
                Status = ClientStatus.Classic
            };

            // Arrange - add an error to the model state
            controller.ModelState.AddModelError("error", "error");

            // Act - try to save the product
            ActionResult result = controller.Edit(client);

            // Assert - check that the repository was not called
            _clientRepositoryMock.Verify(m => m.SaveClient(It.IsAny<Client>()), Times.Never());
            // Assert - check the method result type
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Delete_CallDeleteMethod_AndRedirect()
        {
            //arrange
            ClientController controller = GetClientController();
            int clientId = 1;
            //act
            ActionResult result = controller.Delete(clientId,"returnUrl");
            //assert
            _clientRepositoryMock.Verify(m=>m.DeleteClient(clientId));
            Assert.IsInstanceOf<RedirectResult>(result);
        }
    }
}
