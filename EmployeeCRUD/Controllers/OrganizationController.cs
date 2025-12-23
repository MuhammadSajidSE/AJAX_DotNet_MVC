using Microsoft.AspNetCore.Mvc;

namespace EmployeeCRUD.Controllers
{
    public class OrganizationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
