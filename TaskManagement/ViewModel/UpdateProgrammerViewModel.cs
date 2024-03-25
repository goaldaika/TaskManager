using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagement.Models;

namespace TaskManagement.ViewModel
{
    public class UpdateProgrammerViewModel
    {
        public Programmer Programmer { get; set; }
        public List<SelectListItem>? AvailableAssignments { get; set; }
        public List<int> SelectedAssignments { get; set; } = new List<int>();
    }
}
