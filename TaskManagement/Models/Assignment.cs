using TaskManagement.Data.Enum;

namespace TaskManagement.Models
{
    public class Assignment
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public State state { get; set; }
        public int estimateHours { get; set; }
        public DateTime? closingDate { get; set; }
        public DateTime startDate { get; set; }
        public int? ParentId { get; set; }
        public Assignment? Parent { get; set; }
        public ICollection<Assignment>? Child { get; set; }

        // Navigation property to the assigned Programmer
        public int? ProgrammerId { get; set; } // Foreign key to the Programmer
        public Programmer? AssignedProgrammer { get; set; } // Navigation property

        public Assignment()
        {
            Child = new HashSet<Assignment>();
        }
    }
}
