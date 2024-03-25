using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Data.Enum;
using TaskManagement.Interface;
using TaskManagement.Models;

namespace TaskManagement.Repository
{
    public class AssignmentRepo : IAssignment
    {
        private readonly DataContext _ctx;
        public AssignmentRepo(DataContext ctx)
        {
            _ctx = ctx;
        }
        public bool Add(Assignment assignment)
        {
            _ctx.Add(assignment);
            return Save();
        }

        public bool AssignmentExists(int id)
        {
            return _ctx.Assignments.Any(e => e.id == id);
        }

        public bool Delete(Assignment assignment)
        {
            // Check if the assignment is a parent and unparent children
            if (_ctx.Assignments.Any(a => a.ParentId == assignment.id))
            {
                var childAssignments = _ctx.Assignments.Where(a => a.ParentId == assignment.id).ToList();
                foreach (var child in childAssignments)
                {
                    child.ParentId = null; // Unparent the child assignment
                }
            }

            // Unassign from any programmers
            var assignedProgrammers = _ctx.Programmers.Where(p => p.assignments.Any(a => a.id == assignment.id)).ToList();
            foreach (var programmer in assignedProgrammers)
            {
                var assignmentToRemove = programmer.assignments.FirstOrDefault(a => a.id == assignment.id);
                if (assignmentToRemove != null)
                {
                    programmer.assignments.Remove(assignmentToRemove);
                }
            }

            _ctx.Assignments.Remove(assignment);
            return Save();
        }


        public ICollection<Assignment> GetAll()
        {
            return _ctx.Assignments.Include(a => a.AssignedProgrammer).ToList();
        }

        public async Task<Assignment> GetByIDAsync(int id)
        {
            return await _ctx.Assignments.FirstOrDefaultAsync(i => i.id == id);
        }

        public bool Save()
        {
            var saved = _ctx.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Assignment assignment)
        {
            var existingAssignment = _ctx.Assignments.FirstOrDefault(a => a.id == assignment.id);
            if (existingAssignment != null)
            {
                // Copy properties you want to update
                existingAssignment.name = assignment.name;
                existingAssignment.description = assignment.description;
                existingAssignment.state = assignment.state;
                existingAssignment.estimateHours = assignment.estimateHours;
                existingAssignment.ParentId = assignment.ParentId;
                existingAssignment.ProgrammerId = assignment.ProgrammerId;

                // Special logic for dates based on state
                if (assignment.state == State.Closed)
                {
                    existingAssignment.closingDate = DateTime.Now;
                }
                else if (assignment.state == State.InProgress)
                {
                    existingAssignment.startDate = DateTime.Now;
                }

                _ctx.Update(existingAssignment);
                return Save();
            }

            return false;
        }
    }
}
