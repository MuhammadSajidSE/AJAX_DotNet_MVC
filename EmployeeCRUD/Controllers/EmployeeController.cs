using EmployeeCRUD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeCRUD.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.FullName = user?.FullName;

            ViewBag.Departments = await _context.Departments.ToListAsync();
            var employees = await _context.Employees
                .Include(e => e.Department) 
                .ToListAsync();

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
            var emp = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.id == id);

            if (emp == null) return NotFound();
            return Json(new
            {
                emp.id,
                emp.name,
                emp.contact,
                departmentId = emp.departmentId,
                departmentName = emp.Department?.DeparmentName,
                gender = emp.Gender
            });
        }

        // ADD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(new { message = string.Join(" | ", errors) });
            }
            employee.Department = null;

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            var department = await _context.Departments.FindAsync(employee.departmentId);

            return Json(new
            {
                employee.id,
                employee.name,
                employee.contact,
                departmentId = employee.departmentId,
                departmentName = department?.DeparmentName,
                gender = employee.Gender
            });
        }

        // UPDATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(new { message = string.Join(" | ", errors) });
            }
            var existingEmployee = await _context.Employees.FindAsync(employee.id);
            if (existingEmployee == null) return NotFound();
            existingEmployee.name = employee.name;
            existingEmployee.contact = employee.contact;
            existingEmployee.departmentId = employee.departmentId;
            existingEmployee.Gender = employee.Gender;
            await _context.SaveChangesAsync();
            var updatedEmp = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.id == employee.id);

            return Json(new
            {
                updatedEmp.id,
                updatedEmp.name,
                updatedEmp.contact,
                departmentId = updatedEmp.departmentId,
                departmentName = updatedEmp.Department?.DeparmentName,
                gender = updatedEmp.Gender
            });
        }

        // DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteModel model)
        {
            var emp = await _context.Employees.FindAsync(model.id);
            if (emp == null) return NotFound();

            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Employee deleted successfully" });
        }

        // Helper model for delete
        public class DeleteModel
        {
            public int id { get; set; }
        }

        // SEARCH
        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return await GetAll();

            var data = await _context.Employees
                .Include(e => e.Department)
                .Where(e => e.name.Contains(q))
                .Select(e => new {
                    e.id,
                    e.name,
                    e.contact,
                    departmentId = e.departmentId,
                    departmentName = e.Department.DeparmentName,
                    gender = e.Gender
                })
                .ToListAsync();

            return Json(data);
        }

        // GET ALL (for reset)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Select(e => new {
                    e.id,
                    e.name,
                    e.contact,
                    departmentId = e.departmentId,
                    departmentName = e.Department.DeparmentName,
                    gender = e.Gender
                })
                .ToListAsync();

            return Json(employees);
        }

    }
}