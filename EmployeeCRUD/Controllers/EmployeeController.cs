using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeCRUD.Models;
using System.Threading.Tasks;
using System.Linq;

namespace EmployeeCRUD.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees.ToListAsync();
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_EmployeePagePartial", employees);
            }
            return View(employees);
        }

        // GET FOR EDIT
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return NotFound();
            return Json(emp);
        }

        // ADD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(string.Join(" | ", errors));
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return Json(employee);
        }

        // UPDATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(string.Join(" | ", errors));
            }

            var existingEmployee = await _context.Employees.FindAsync(employee.id);
            if (existingEmployee == null) return NotFound();
            existingEmployee.name = employee.name;
            existingEmployee.contact = employee.contact;
            existingEmployee.department = employee.department;
            existingEmployee.Gender = employee.Gender;
            await _context.SaveChangesAsync();
            return Json(existingEmployee);
        }

        // DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return NotFound();

            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // SEARCH
        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(await _context.Employees.ToListAsync());

            var data = await _context.Employees
                .Where(e => e.name.Contains(q))
                .ToListAsync();

            return Json(data);
        }

        // GET ALL (for reset)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(await _context.Employees.ToListAsync());
        }
    }
}