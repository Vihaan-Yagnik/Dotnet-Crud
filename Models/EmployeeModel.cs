using Microsoft.AspNetCore.Mvc;

namespace Admin3.Models
{
    public class EmployeeModel
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public decimal Phone_Number { get; set; }
        public DateTime Hire_Date { get; set; }
        public string Job_Title  { get; set; }
        public double Salary     { get; set; }

        public int DepartmentId {  get; set; }
    }
}
