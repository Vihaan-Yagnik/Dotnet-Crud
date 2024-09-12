using Admin3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace Admin3.Controllers
{
    public class EmployeeController : Controller
    {
        public static List<EmployeeModel> employeeModels = new List<EmployeeModel>()
            {
                new EmployeeModel{EmployeeId = 1, FirstName = "Krish", LastName = "Gohel", Email = "krish@gmail.com",Phone_Number=9558566847,Job_Title="Full Stack Developer",Salary=10000000,DepartmentId=1,Hire_Date=new DateTime(2024-03-17)},
                new EmployeeModel{EmployeeId = 2, FirstName = "Vihaan", LastName = "Yagnik", Email = "Vihan@gmail.com",Phone_Number=8547568541,Job_Title="Full Stack Developer",Salary=10000000,DepartmentId=4,Hire_Date=new DateTime(2023-12-18)},
                new EmployeeModel{EmployeeId = 3, FirstName = "Mehul", LastName = "Parmar", Email = "Mehul@gmail.com",Phone_Number=9857563241,Job_Title="Back End Developer",Salary=50000,DepartmentId=2,Hire_Date=new DateTime(2024-01-17)},
                new EmployeeModel{EmployeeId = 4, FirstName = "Sachin", LastName = "Patodiya", Email = "Sachin@gmail.com",Phone_Number=9583214587,Job_Title="Front End Developer",Salary=100000,DepartmentId=3,Hire_Date=new DateTime(2024-05-05)},
            };
        public IActionResult Index()
        {
            return View("EmployeeTable", employeeModels);
        }
        public IActionResult EmployeeForm()
        {
            return View();
        }

        public IActionResult SaveEmployee(EmployeeModel Emp)
        {
            if (Emp.EmployeeId == 0)
            {
                Emp.EmployeeId = employeeModels.Max(e => e.EmployeeId + 1);
                employeeModels.Add(Emp);
            }
            else
            {
                employeeModels[Emp.EmployeeId - 1].FirstName = Emp.FirstName;
                employeeModels[Emp.EmployeeId - 1].LastName = Emp.LastName;
                employeeModels[Emp.EmployeeId - 1].Phone_Number = Emp.Phone_Number;
                employeeModels[Emp.EmployeeId - 1].Email = Emp.Email;
                employeeModels[Emp.EmployeeId - 1].Job_Title = Emp.Job_Title;
                employeeModels[Emp.EmployeeId - 1].Hire_Date = Emp.Hire_Date;
                employeeModels[Emp.EmployeeId - 1].DepartmentId = Emp.DepartmentId;
                employeeModels[Emp.EmployeeId - 1].Salary = Emp.Salary;
            }
            return View("EmployeeTable", employeeModels);
        }
        public IActionResult AddEdit(int EmployeeId = 0)
        {
            EmployeeModel emp = new EmployeeModel();

            if (EmployeeId != 0)
            {
                var selectEmp = employeeModels.Find(e => e.EmployeeId == EmployeeId);

                emp.EmployeeId = selectEmp.EmployeeId;
                emp.FirstName = selectEmp.FirstName;
                emp.LastName = selectEmp.LastName;
                emp.Email = selectEmp.Email;
                emp.Salary = selectEmp.Salary;
                emp.DepartmentId = selectEmp.DepartmentId;
                emp.Hire_Date = selectEmp.Hire_Date;
                emp.Phone_Number = selectEmp.Phone_Number;
                emp.Job_Title = selectEmp.Job_Title;
            }

            return View("EmployeeForm", emp);
        }

        public IActionResult DelEmp(int EmployeeId = 0)
        {
            if (EmployeeId != 0)
            {
                employeeModels.RemoveAt(EmployeeId);
            }
            return View("EmployeeTable", employeeModels);
        }
    }
}
