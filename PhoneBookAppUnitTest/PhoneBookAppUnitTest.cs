
using Microsoft.AspNetCore.Mvc;
using Moq;
using PhoneBookApp.Controllers;
using PhoneBookApp.DTOs;
using PhoneBookApp.Services;


namespace PhoneBookApp.Tests
{
    public class ContactsControllerTests
    {
        private readonly Mock<IContactService> _mockService;
        private readonly ContactsController _controller;

        public ContactsControllerTests()
        {
            _mockService = new Mock<IContactService>();
            _controller = new ContactsController(_mockService.Object);
        }

        [Fact]
        public async Task GetContacts_AllContactsExist_ReturnsAllContacts()
        {
            // Arrange
            var expectedContacts = new List<ContactDto>
    {
        new ContactDto { Id = 1, Name = "John Doe", PhoneNumber = "1234567890" },
        new ContactDto { Id = 2, Name = "Jane Doe", PhoneNumber = "0987654321" }
    };
            _mockService.Setup(service => service.GetAllContactsAsync())
                .ReturnsAsync(expectedContacts);

            // Act
            var result = await _controller.GetContacts();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<ContactDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var contacts = Assert.IsAssignableFrom<IEnumerable<ContactDto>>(okResult.Value);
            Assert.Equal(2, contacts.Count());
        }

        [Fact]
        public async Task GetContact_ContactExists_ReturnsContact()
        {
            // Arrange
            var expectedContact = new ContactDto { Id = 1, Name = "John Doe", PhoneNumber = "1234567890" };
            _mockService.Setup(service => service.GetContactByIdAsync(1))
                .ReturnsAsync(expectedContact);

            // Act
            var result = await _controller.GetContact(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ContactDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedContact = Assert.IsType<ContactDto>(okResult.Value);
            Assert.Equal(expectedContact.Id, returnedContact.Id);
            Assert.Equal(expectedContact.Name, returnedContact.Name);
            Assert.Equal(expectedContact.PhoneNumber, returnedContact.PhoneNumber);
        }

        [Fact]
        public async Task CreateContact_ValidContactDto_CreatesNewContact()
        {
            // Arrange
            var newContactDto = new CreateContactDto { Name = "New Contact", PhoneNumber = "1112223333" };
            var createdContact = new ContactDto { Id = 1, Name = "New Contact", PhoneNumber = "1112223333" };
            _mockService.Setup(service => service.CreateContactAsync(newContactDto))
                .ReturnsAsync(createdContact);

            // Act
            var result = await _controller.CreateContact(newContactDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ContactDto>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnedContact = Assert.IsType<ContactDto>(createdAtActionResult.Value);
            Assert.Equal(createdContact.Id, returnedContact.Id);
            Assert.Equal(createdContact.Name, returnedContact.Name);
            Assert.Equal(createdContact.PhoneNumber, returnedContact.PhoneNumber);
        }

        [Fact]
        public async Task UpdateContact_ExistingContactId_UpdatesContact()
        {
            // Arrange
            var updatedContactDto = new CreateContactDto { Name = "John Updated", PhoneNumber = "9876543210" };
            _mockService.Setup(service => service.UpdateContactAsync(1, updatedContactDto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateContact(1, updatedContactDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(service => service.UpdateContactAsync(1, updatedContactDto), Times.Once);
        }

        [Fact]
        public async Task DeleteContact_ExistingContactId_RemovesContact()
        {
            // Arrange
            _mockService.Setup(service => service.DeleteContactAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteContact(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(service => service.DeleteContactAsync(1), Times.Once);
        }
    }
}