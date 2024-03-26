using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskManagement.Controllers;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Repository;
using TaskManagement.ViewModel;

namespace TaskManager_Test.ProgrammerTest.Controller
{
    [TestFixture]
    public class HomeControllerTests
    {
        private HomeController _controller;
        private DataContext _context;

        [SetUp]
        public void SetUp()
        {
            // Setup the in-memory database
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);

            // Setup the HomeController with the in-memory DataContext
            var programmerService = new ProgrammerRepo(_context);
            _controller = new HomeController(_context, programmerService);
        }

        [Test]
        public void Create_PostAddsProgrammerAndRedirects_WithValidModel()
        {
            // Arrange
            var newProgrammer = new Programmer { fname = "Jane", lname = "Doe" };

            // Act
            var result = _controller.Create(newProgrammer, null);

            // Assert
            Assert.That(_context.Programmers.Any(p => p.fname == "Jane" && p.lname == "Doe"));
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
        [Test]
        public void Index_ReturnsViewWithAllProgrammers()
        {
            // Arrange
            _context.Programmers.Add(new Programmer { id = 1, fname = "John", lname = "Doe" });
            _context.Programmers.Add(new Programmer { id = 2, fname = "Jane", lname = "Smith" });
            _context.SaveChanges();

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            var model = result.Model as IEnumerable<Programmer>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count());
        }

        [Test]
        public void Create_InvalidModel_DoesNotAddProgrammerAndReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Model is invalid");
            var newProgrammer = new Programmer { fname = "Invalid", lname = "Model" };

            // Act
            var result = _controller.Create(newProgrammer, null) as ViewResult;

            // Assert
            Assert.IsFalse(_context.Programmers.Any(p => p.fname == "Invalid" && p.lname == "Model"));
            Assert.IsNotNull(result);
        }
        [Test]
        public async Task Edit_Get_ReturnsCorrectViewModel_ForExistingProgrammer()
        {
            // Arrange
            var programmer = new Programmer { id = 5, fname = "Bruce", lname = "Wayne" };
            _context.Programmers.Add(programmer);

           
            var assignment = new Assignment { id = 1, name = "Assignment 1", description = "Sample description", ProgrammerId = 5 };

            _context.Assignments.Add(assignment);
            _context.SaveChanges();

            // Act
            var result = await _controller.Edit(programmer.id) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var viewModel = result.Model as UpdateProgrammerViewModel;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(programmer.fname, viewModel.Programmer.fname);
            Assert.AreEqual(programmer.lname, viewModel.Programmer.lname);
            Assert.IsTrue(viewModel.AvailableAssignments.Any());
            Assert.IsTrue(viewModel.SelectedAssignments.Any());
        }

        [Test]
        public async Task Delete_ValidProgrammerId_DeletesProgrammerAndRedirects()
        {
            // Arrange
            var programmer = new Programmer { id = 3, fname = "Mike", lname = "Johnson" };
            _context.Programmers.Add(programmer);
            _context.SaveChanges();

            // Act
            var result = await _controller.DeleteProgrammer(programmer.id) as RedirectToActionResult;

            // Assert
            Assert.IsFalse(_context.Programmers.Any(p => p.id == programmer.id));
            Assert.AreEqual("Index", result.ActionName);
        }




        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}