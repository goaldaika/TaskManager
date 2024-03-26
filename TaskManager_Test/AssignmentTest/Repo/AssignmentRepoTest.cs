using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskManagement.Data.Enum;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Repository;

namespace TaskManager_Test.AssignmentTest.Repo
{
    [TestFixture]
    public class AssignmentRepoTests
    {
        private DataContext _context;
        private AssignmentRepo _assignmentRepo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(options);
            _assignmentRepo = new AssignmentRepo(_context);
        }

        [Test]
        public void GetAll_Assignments_ShouldReturnAllAssignments()
        {
            // Arrange
            _context.Assignments.Add(new Assignment { id = 1, name = "Test Assignment 1", description = "Test Description 1", state = State.New, startDate = DateTime.UtcNow });
            _context.Assignments.Add(new Assignment { id = 2, name = "Test Assignment 2", description = "Test Description 2", state = State.New, startDate = DateTime.UtcNow });
            _context.SaveChanges();

            // Act
            var assignments = _assignmentRepo.GetAll();

            // Assert
            Assert.IsNotNull(assignments);
            Assert.AreEqual(2, assignments.Count);
        }

        [Test]
        public void Delete_Assignment_ShouldRemoveAssignment()
        {
            // Arrange
            var assignment = new Assignment { id = 1, name = "Test Assignment", description = "Test Description", state = State.New, startDate = DateTime.UtcNow };
            _context.Assignments.Add(assignment);
            _context.SaveChanges();

            // Act
            var result = _assignmentRepo.Delete(assignment);

            // Assert
            var deletedAssignment = _context.Assignments.Find(1);
            Assert.IsTrue(result);
            Assert.IsNull(deletedAssignment);
        }

        [Test]
        public void Update_Assignment_ShouldUpdateAssignment()
        {
            // Arrange
            var assignment = new Assignment { id = 1, name = "Old Name", description = "Old Description", state = State.New, startDate = DateTime.UtcNow };
            _context.Assignments.Add(assignment);
            _context.SaveChanges();

            var updatedAssignment = new Assignment { id = 1, name = "New Name", description = "New Description", state = State.Resolved, startDate = DateTime.UtcNow };

            // Act
            var result = _assignmentRepo.Update(updatedAssignment);

            // Assert
            var actual = _context.Assignments.Find(1);
            Assert.IsTrue(result);
            Assert.IsNotNull(actual);
            Assert.AreEqual("New Name", actual.name);
            Assert.AreEqual("New Description", actual.description);
            Assert.AreEqual(State.Resolved, actual.state);
        }

        [Test]
        public void AssignmentExists_AssignmentExists_ShouldReturnTrue()
        {
            // Arrange
            var assignment = new Assignment { id = 1, name = "Existing Assignment", description = "Exists in DB", state = State.New, startDate = DateTime.UtcNow };
            _context.Assignments.Add(assignment);
            _context.SaveChanges();

            // Act
            var exists = _assignmentRepo.AssignmentExists(1);

            // Assert
            Assert.IsTrue(exists);
        }

        [Test]
        public void AssignmentExists_AssignmentDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var assignmentId = 99; 

            // Act
            var exists = _assignmentRepo.AssignmentExists(assignmentId);

            // Assert
            Assert.IsFalse(exists);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }
    }
}