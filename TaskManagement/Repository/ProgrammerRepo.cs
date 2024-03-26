using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Interface;
using TaskManagement.Models;

namespace TaskManagement.Repository
{
    public class ProgrammerRepo : IProgrammer
    {
        private readonly DataContext _ctx;
        public ProgrammerRepo(DataContext ctx)
        {
            _ctx = ctx;
        }

        public bool Add(Programmer programmer)
        {
            _ctx.Add(programmer);
            return Save();
        }

        public bool Delete(Programmer programmer)
        {
            // Set ProgrammerId to null for all related Assignment records
            var assignments = _ctx.Assignments.Where(a => a.ProgrammerId == programmer.id).ToList();
            foreach (var assignment in assignments)
            {
                assignment.ProgrammerId = null; // Disassociate the assignment
            }

            _ctx.Remove(programmer);
            return Save();
        }


        public ICollection<Programmer> GetAll()
        {
            return _ctx.Programmers.Include(p => p.assignments).ToList();
        }

        public async Task<Programmer> GetByIDAsync(int id)
        {
            return await _ctx.Programmers.FirstOrDefaultAsync(i => i.id == id);
        }

        public bool ProgrammerExists(int id)
        {
            return _ctx.Programmers.Any(e => e.id == id);
        }

        public bool Save()
        {
            var saved = _ctx.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Programmer updatedProgrammer, List<int> selectedAssignmentIds)
        {
            // Fetch the current state of the programmer, including their assignments
            var currentProgrammer = _ctx.Programmers
                .Include(p => p.assignments)
                .FirstOrDefault(p => p.id == updatedProgrammer.id);

            if (currentProgrammer == null) return false;

            // Update programmer's properties
            currentProgrammer.fname = updatedProgrammer.fname;
            currentProgrammer.lname = updatedProgrammer.lname;
            currentProgrammer.email = updatedProgrammer.email;
            currentProgrammer.address = updatedProgrammer.address;
            currentProgrammer.phonenumber = updatedProgrammer.phonenumber;

            //Remove any assignments that are no longer selected
            var assignmentsToRemove = currentProgrammer.assignments
                .Where(a => !selectedAssignmentIds.Contains(a.id)).ToList();

            foreach (var assignment in assignmentsToRemove)
            {
                assignment.ProgrammerId = null; // Disassociate the assignment
                currentProgrammer.assignments.Remove(assignment);
            }

            //Add new assignments that were selected but not previously associated
            var currentAssignmentIds = currentProgrammer.assignments.Select(a => a.id).ToList();
            var assignmentsToAdd = _ctx.Assignments
                .Where(a => selectedAssignmentIds.Contains(a.id) && !currentAssignmentIds.Contains(a.id)).ToList();

            foreach (var assignment in assignmentsToAdd)
            {
                assignment.ProgrammerId = currentProgrammer.id; // Associate the assignment
                currentProgrammer.assignments.Add(assignment);
            }

            _ctx.SaveChanges();
            return true;
        }

    }
}

