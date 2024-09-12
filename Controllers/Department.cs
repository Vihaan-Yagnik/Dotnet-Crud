using Admin3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Admin3.Controllers
{
    public class Department : Controller
    {
        public static List<DepartmentModel> departmentModels = new List<DepartmentModel>()
        {
            new DepartmentModel { DepartmentId = 1, DepartmentName = "Human Resources" },
            new DepartmentModel { DepartmentId = 2, DepartmentName = "Finance" },
            new DepartmentModel { DepartmentId = 3, DepartmentName = "IT" },
            new DepartmentModel { DepartmentId = 4, DepartmentName = "Marketing" },
            new DepartmentModel { DepartmentId = 5, DepartmentName = "Sales" }
        };
        public IActionResult Index()
        {
            return View("DepartmentTable",departmentModels);
        }
        public IActionResult DepartmentForm()
        {
            return View();
        }
        public IActionResult SaveDepartment(DepartmentModel D)
        {
            if(D.DepartmentId == 0)
            {
                D.DepartmentId = departmentModels.Max(x => x.DepartmentId + 1);
                departmentModels.Add(D);
            }

            return View("DepartmentTable",departmentModels);
        }
    }
}
