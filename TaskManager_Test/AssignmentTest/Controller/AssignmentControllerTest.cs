using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using TaskManagement.Controllers;
using TaskManagement.Data.Enum;
using TaskManagement.Interface;
using TaskManagement.Models;

namespace TaskManager_Test.AssignmentTest.Controller
{
    [TestFixture]
    public class AssignmentsControllerTests
    {
        private Mock<IAssignment> _mockRepo;
        private AssignmentsController _controller;

        [SetUp]
        public void Setup()
        {
            // Mock the IAssignment repository
            _mockRepo = new Mock<IAssignment>();

            // Instantiate the controller with the mocked repository
            _controller = new AssignmentsController(null, _mockRepo.Object);
        }

        [Test]
        public async Task Details_WhenCalled_ReturnsViewResultWithAssignment()
        {
            // Arrange
            var assignmentId = 1;
            var mockAssignment = new Assignment
            {
                id = assignmentId,
                name = "Test Assignment",
                description = "Test Description",
                state = State.New,
                estimateHours = 10,
            };

            _mockRepo.Setup(repo => repo.GetByIDAsync(assignmentId)).ReturnsAsync(mockAssignment);

            // Act
            var result = await _controller.Details(assignmentId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as Assignment;
            Assert.AreEqual(assignmentId, model.id);
        }

        [Test]
        public async Task Details_IdIsNull_ReturnsNotFound()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.Details(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_AssignmentDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIDAsync(It.IsAny<int>())).ReturnsAsync((Assignment)null);

            // Act
            var result = await _controller.Details(1);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        [Test]
        public async Task Create_Post_ValidAssignment_CreatesNewAssignmentAndRedirectsToIndex()
        {
            // Arrange
            var newAssignment = new Assignment
            {
                name = "New Assignment",
                description = "New Description",
                state = State.New,
                estimateHours = 10,
                startDate = DateTime.Now
            };

            _mockRepo.Setup(repo => repo.Add(newAssignment)).Returns(true);

            // Act
            var result = _controller.Create(newAssignment);

            // Assert
            _mockRepo.Verify(repo => repo.Add(It.IsAny<Assignment>()), Times.Once);
            Assert.IsInstanceOf<RedirectToActionResult>(result);

            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [Test]
        public async Task Delete_Confirmed_DeletesAssignmentAndRedirectsToIndex()
        {
            // Arrange
            var assignmentId = 1;
            var mockAssignment = new Assignment
            {
                id = assignmentId,
                name = "Test Name",
                description = "Description",
                estimateHours = 123
            };

            _mockRepo.Setup(repo => repo.GetByIDAsync(assignmentId)).ReturnsAsync(mockAssignment);
            _mockRepo.Setup(repo => repo.Delete(It.Is<Assignment>(a => a.id == assignmentId))).Returns(true);

            // Act
            var result = await _controller.DeleteAssignment(assignmentId);

            // Assert
            _mockRepo.Verify(repo => repo.Delete(It.Is<Assignment>(a => a.id == assignmentId)), Times.Once);
            Assert.IsInstanceOf<RedirectToActionResult>(result);

            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }


        [TearDown]
        public void TearDown()
        {
            _mockRepo = null;
            _controller = null;
        }
    }
}
