using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TaskManagement.Data;
using TaskManagement.Models;
using TaskManagement.Repository;

namespace TaskManager_Test.ProgrammerTest.Repo
{
    [TestFixture]
    public class ProgrammerRepoTests
    {
        private DataContext _context;
        private ProgrammerRepo _repo;

        [SetUp]
        public void SetUp()
        {
            var dbName = $"InMemDb_{Guid.NewGuid()}";
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _context = new DataContext(options);
            _repo = new ProgrammerRepo(_context);
            _context.Programmers.Add(new Programmer { id = 1, fname = "John", lname = "Doe" });
            _context.SaveChanges();
        }

        [Test]
        public async Task GetByIDAsync_ReturnsProgrammer_WhenProgrammerExists()
        {

            // Act
            var result = await _repo.GetByIDAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.id, Is.EqualTo(1));
            Assert.That(result.fname, Is.EqualTo("John"));
            Assert.That(result.lname, Is.EqualTo("Doe"));
        }

        [Test]
        public void GetAll_ReturnsAllProgrammers()
        {

            // Act
            var results = _repo.GetAll();

            // Assert
            Assert.That(results, Is.Not.Empty);
            Assert.That(results.Count, Is.EqualTo(1)); 
        }

        [Test]
        public void Add_AddsProgrammer_WhenCalledWithValidData()
        {
            // Arrange
            var programmer = new Programmer { id = 2, fname = "Jane", lname = "Smith" };

            // Act
            var result = _repo.Add(programmer);
            _context.SaveChanges(); 

            // Assert
            Assert.IsTrue(result);
            Assert.That(_context.Programmers.Any(p => p.id == programmer.id));
        }
        [Test]
        public void Delete_DeletesProgrammer_WhenProgrammerExists()
        {
            // Arrange
            var programmer = new Programmer { id = 3, fname = "Mike", lname = "Johnson" };
            _context.Programmers.Add(programmer);
            _context.SaveChanges();

            // Act
            var result = _repo.Delete(programmer);
            _context.SaveChanges();

            // Assert
            Assert.IsTrue(result);
            Assert.That(!_context.Programmers.Any(p => p.id == programmer.id));
        }


        [Test]
        public void Save_ReturnsTrue_WhenChangesAreMade()
        {
            // Arrange - make a change
            var programmer = _context.Programmers.First();
            programmer.fname = "UpdatedName";

            // Act
            var result = _repo.Save();

            // Assert
            Assert.IsTrue(result);
        }


        [TearDown]
        public void TearDown()
        {
            // Dispose the in-memory context after each test
            _context.Dispose();
        }
    }
}