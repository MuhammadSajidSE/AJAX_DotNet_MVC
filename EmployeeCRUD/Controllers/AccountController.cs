using EmployeeCRUD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeCRUD.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return Json(new { success = false, message = result.Errors.First().Description });

            await _signInManager.SignInAsync(user, isPersistent: false);

            return Json(new
            {
                success = true,
                redirectUrl = "/Employee"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                false,
                true
            );

            if (!result.Succeeded)
                return Json(new { success = false, message = "Invalid email or password" });

            return Json(new
            {
                success = true,
                redirectUrl = "/Employee"
            });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Account");
        }
        public IActionResult CheckAuth()
        {
            return Json(new { isAuthenticated = true });
        }
    }

}