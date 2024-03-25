using TaskManagement.Models;

namespace TaskManagement.Interface
{
    public interface IProgrammer
    {
        ICollection<Programmer> GetAll();
        Task<Programmer> GetByIDAsync(int id);
        bool Add(Programmer programmer);
        bool Update(Programmer programmer, List<int> selectedAssignmentIds);
        bool Delete(Programmer programmer);
        bool ProgrammerExists(int id);
        bool Save();
    }
}
