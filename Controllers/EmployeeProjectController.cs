using Admin3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Admin3.Controllers
{
    public class EmployeeProjectController : Controller
    {
        public static List<EmployeeProjectModel> employeeProjectModels = new List<EmployeeProjectModel>(){
            new EmployeeProjectModel { EmployeeProjectId = 1, EmployeeId = 101, ProjectId = 201 },
            new EmployeeProjectModel { EmployeeProjectId = 2, EmployeeId = 102, ProjectId = 202 },
            new EmployeeProjectModel { EmployeeProjectId = 3, EmployeeId = 103, ProjectId = 203 },
            new EmployeeProjectModel { EmployeeProjectId = 4, EmployeeId = 104, ProjectId = 204 },
            new EmployeeProjectModel { EmployeeProjectId = 5, EmployeeId = 105, ProjectId = 205 }
        };
        public IActionResult Index()
        {
            return View("EmployeeProject",employeeProjectModels);
        }
        public IActionResult EmployeeProjectForm() {
            return View();
        }
        public IActionResult SaveEmployeeProject(EmployeeProjectModel ep)
        {
            if(ep.EmployeeProjectId == 0)
            {
                ep.EmployeeProjectId = employeeProjectModels.Max(x => x.EmployeeProjectId + 1);
                employeeProjectModels.Add(ep);
            }
            return View("EmployeeProject",employeeProjectModels);
        }
    }
}
