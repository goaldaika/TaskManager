using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class Programmer
    {
        public int id { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        public string fname { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string lname { get; set; }
        public string? email { get; set; }
        public string? address { get; set; } //homeless ?
        public long? phonenumber { get; set; }
        public ICollection<Assignment> assignments { get; set; }

        public Programmer()
        {
            assignments = new List<Assignment>();
        }
    }
}
