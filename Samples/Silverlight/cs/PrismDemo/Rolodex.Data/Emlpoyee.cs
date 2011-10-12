using System.ComponentModel.DataAnnotations;

namespace Rolodex.Data
{
    public class Emlpoyee
    {
        public int EmlpoyeeID { get; set; }
        public int CompanyID { get; set; }
        public int EmployeeStatusID { get; set; }
        [StringLength(30)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }

        [ForeignKey("CompanyID")]
        public Company Company { get; set; }

        [ForeignKey("EmployeeStatusID")]
        public EmployeeStatus EmployeeStatus { get; set; }
    }
}
