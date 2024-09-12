namespace Admin3.Models
{
    public class ProjectModel
    {
        public int ProjectId    { get; set; }
        public string ProjectName { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
        public double Budget    { get; set; }
    }
}
