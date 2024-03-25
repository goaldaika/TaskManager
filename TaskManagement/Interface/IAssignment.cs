using TaskManagement.Models;

namespace TaskManagement.Interface
{
    public interface IAssignment
    {
        ICollection<Assignment> GetAll();
        Task<Assignment> GetByIDAsync(int id);
        bool Add(Assignment assignment);
        bool Delete(Assignment assignment);
        bool Update(Assignment assignment);
        bool AssignmentExists(int id);    
        bool Save();
    }
}
