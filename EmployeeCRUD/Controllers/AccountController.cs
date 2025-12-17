using EmployeeCRUD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCRUD.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        //public IActionResult Login()
        //{
        //    return View();
        //}

        //public IActionResult SignUp()
        //{
        //    return View();
        //}

        public IActionResult Index()
        {
            return View();
        }   
        public IActionResult Login()
        {
            return PartialView("_LoginPartial");
        }

        public IActionResult SignUp()
        {
            return PartialView("_SignUpPartial");
        }

        // New method to get Employee page content
        public async Task<IActionResult> GetEmployeePage()
        {
            var employees = await _context.Employees.ToListAsync();
            return PartialView("_EmployeePagePartial", employees);
        }
    }
}
