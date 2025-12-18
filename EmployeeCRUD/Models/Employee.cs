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
        public string contact { get; set; }
        public int departmentId { get; set; }

        // Navigation property to Department
        [ForeignKey("departmentId")]
        public Department? Department { get; set; }
        [Required]
        public string Gender { get; set; }
    }
}
