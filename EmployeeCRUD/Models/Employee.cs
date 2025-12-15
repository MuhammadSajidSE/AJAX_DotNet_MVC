using System.ComponentModel.DataAnnotations;

namespace EmployeeCRUD.Models
{
    public class Employee
    {
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string contact { get; set; }
        [Required]
        public string department { get; set; }
        [Required]
        public string Gender { get; set; }
    }
}
