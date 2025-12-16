using Microsoft.AspNetCore.Identity;

namespace EmployeeCRUD.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

