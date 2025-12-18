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

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] AppUser user)
        {
            try
            {
                if (user == null)
                {
                    return Json(new { success = false, message = "Invalid user data" });
                }
                var existingUser = await _context.AppUsers
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    return Json(new { success = false, message = "Email already exists!" });
                }
                _context.AppUsers.Add(user);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Registration successful!",
                    user = new
                    {
                        id = user.Id,
                        name = user.Username,
                        email = user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    return Json(new { success = false, message = "Email and password are required" });
                }

                var user = await _context.AppUsers
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

                if (user == null)
                {
                    return Json(new { success = false, message = "Invalid email or password!" });
                }

                return Json(new
                {
                    success = true,
                    message = "Login successful!",
                    user = new
                    {
                        id = user.Id,
                        name = user.Username,
                        email = user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }


        public IActionResult CheckAuth()
        {
            return Json(new { isAuthenticated = true });
        }
    }

}