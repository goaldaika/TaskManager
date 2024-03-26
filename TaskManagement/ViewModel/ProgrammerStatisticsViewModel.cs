namespace TaskManagement.ViewModel
{
    public class ProgrammerStatisticsViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Dictionary<string, int> HoursByState { get; set; } = new Dictionary<string, int>();
    }

}
