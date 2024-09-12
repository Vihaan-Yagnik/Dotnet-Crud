using Admin3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Admin3.Controllers
{
    public class ProjectController : Controller
    {
        public static List<ProjectModel> projectModels = new List<ProjectModel>()
        {
            new ProjectModel
            {
                ProjectId = 1,
                ProjectName = "Project Alpha",
                Start_Date = new DateTime(2023, 1, 1),
                End_Date = new DateTime(2023, 6, 30),
                Budget = 100000
            },
            new ProjectModel
            {
                ProjectId = 2,
                ProjectName = "Project Beta",
                Start_Date = new DateTime(2023, 2, 15),
                End_Date = new DateTime(2023, 8, 15),
                Budget = 200000
            },
            new ProjectModel
            {
                ProjectId = 3,
                ProjectName = "Project Gamma",
                Start_Date = new DateTime(2023, 3, 1),
                End_Date = new DateTime(2023, 9, 30),
                Budget = 150000
            },
            new ProjectModel
            {
                ProjectId = 4,
                ProjectName = "Project Delta",
                Start_Date = new DateTime(2023, 4, 1),
                End_Date = new DateTime(2023, 10, 31),
                Budget = 120000
            },
            new ProjectModel
            {
                ProjectId = 5,
                ProjectName = "Project Epsilon",
                Start_Date = new DateTime(2023, 5, 1),
                End_Date = new DateTime(2023, 12, 31),
                Budget = 180000
            }

        };

        public IActionResult Index()
        {
            return View("ProjectTable", projectModels);
        }
        public IActionResult ProjectForm()
        {
            return View();
        }

        public IActionResult SaveProject(ProjectModel P)
        {
            if(P.ProjectId == 0)
            {
                P.ProjectId = projectModels.Max(x => x.ProjectId + 1);
                projectModels.Add(P);
            }

            return View("ProjectTable",projectModels);
        }
    }
}
