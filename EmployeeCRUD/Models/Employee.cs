using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeCRUD.Models
{
    public class Employee
    {
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        [RegularExpression(@"^\+92\d{10}$", ErrorMessage = "Phone number must start with +92 and be 12 digits long")]
        public string contact { get; set; }
        public int departmentId { get; set; }
        [ForeignKey("departmentId")]
        public Department? Department { get; set; }
        [Required]
        public string Gender { get; set; }
    }
}
