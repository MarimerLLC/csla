using System.ComponentModel.DataAnnotations;

namespace Rolodex.Data
{
    public class EmployeeStatus
    {
        public int EmployeeStatusID { get; set; }
        [StringLength(30)]
        public string StatusName { get; set; }
    }
}
