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
                new Client {Id = 1, ContractNumber = "P1"},
                new Client {Id = 2, ContractNumber = "P2"},
                new Client {Id = 3, ContractNumber = "P3"},
                new Client {Id = 4, ContractNumber = "P4"},
                new Client {Id = 5, ContractNumber = "P5"}
            });
        }
        
        private ClientController GetClientController()
        {
            return new ClientController(_clientRepositoryMock.Object);
        }

        [Test]
        public void Can_Paginate()
        {
            // Arrange
            ClientController controller = GetClientController();
            controller.PageSize = 3;

            // Act  
            ClientListViewModel result = (ClientListViewModel)((ViewResult)controller.Index(2)). Model;
            
            // Assert
            Client[] prodArray = result.Clients.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].ContractNumber, "P4");
            Assert.AreEqual(prodArray[1].ContractNumber, "P5");
        }

        [Test]
        public void Can_Generate_Page_Links()
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
        public void Can_Send_Pagination_View_Model()
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
        public void Can_Edit_Client()
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
        public void Can_Edit_NonExistent_Client()
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
        public void Can_AddClient()
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
        public void Can_Save_Valid_Changes()
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
            ActionResult result = controller.Edit(client);

            // Assert - check that the repository was called
            _clientRepositoryMock.Verify(m => m.SaveClient(client));
            // Assert - check the method result type
            Assert.IsNotInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Cannot_Save_Invalid_Changes()
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
            ActionResult result = controller.Delete(clientId);
            //assert
            _clientRepositoryMock.Verify(m=>m.DeleteClient(clientId));
            Assert.IsInstanceOf<RedirectToRouteResult>(result);
        }
    }
}
