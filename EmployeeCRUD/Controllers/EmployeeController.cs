using EmployeeCRUD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeCRUD.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeeController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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

        // GET employee for editing
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
                gender = emp.Gender,
                imagePath = emp.ImagePath // Include image path
            });
        }

        // ADD employee with image
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromForm] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                return BadRequest(new { message = string.Join(" | ", errors) });
            }

            try
            {
                // Save employee first to get ID
                employee.Department = null;
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Handle image upload if image file is provided
                if (employee.ImageFile != null && employee.ImageFile.Length > 0)
                {
                    var imagePath = await ImageHelper.SaveImageAsync(
                        employee.ImageFile,
                        _webHostEnvironment,
                        employee.id
                    );

                    // Update employee with image path
                    employee.ImagePath = imagePath;
                    await _context.SaveChangesAsync();
                }

                var department = await _context.Departments.FindAsync(employee.departmentId);

                return Json(new
                {
                    employee.id,
                    employee.name,
                    employee.contact,
                    departmentId = employee.departmentId,
                    departmentName = department?.DeparmentName,
                    gender = employee.Gender,
                    imagePath = employee.ImagePath // Return image path
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // UPDATE employee with image
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] Employee employee)
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

            try
            {
                // Handle image upload if new image is provided
                if (employee.ImageFile != null && employee.ImageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(existingEmployee.ImagePath))
                    {
                        ImageHelper.DeleteImage(existingEmployee.ImagePath, _webHostEnvironment);
                    }

                    // Save new image
                    var imagePath = await ImageHelper.SaveImageAsync(
                        employee.ImageFile,
                        _webHostEnvironment,
                        employee.id
                    );
                    existingEmployee.ImagePath = imagePath;
                }

                // Update other fields
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
                    gender = updatedEmp.Gender,
                    imagePath = updatedEmp.ImagePath
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromBody] DeleteModel model)
        {
            var emp = await _context.Employees.FindAsync(model.id);
            if (emp == null) return NotFound();

            // Delete associated image file
            if (!string.IsNullOrEmpty(emp.ImagePath))
            {
                ImageHelper.DeleteImage(emp.ImagePath, _webHostEnvironment);
            }

            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Employee deleted successfully" });
        }

        // Helper model for delete
        public class DeleteModel
        {
            public int id { get; set; }
        }

        // SEARCH employees
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
                    gender = e.Gender,
                    imagePath = e.ImagePath // Include image path
                })
                .ToListAsync();

            return Json(data);
        }

        // GET ALL employees
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
                    gender = e.Gender,
                    imagePath = e.ImagePath // Include image path
                })
                .ToListAsync();

            return Json(employees);
        }

        // GET method to serve image (if needed separately)
        [HttpGet]
        public IActionResult GetImage(string path)
        {
            if (string.IsNullOrEmpty(path))
                return NotFound();

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, path.TrimStart('/'));

            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var contentType = GetContentType(fullPath);
            return PhysicalFile(fullPath, contentType);
        }

        private string GetContentType(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }
    }
}