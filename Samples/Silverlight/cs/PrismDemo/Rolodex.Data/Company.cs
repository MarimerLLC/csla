using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace Rolodex.Data
{
    public class Company
    {
        public int CompanyID { get; set; }
        [StringLength(30)]
        public string CompanyName { get; set; }
        public string Notes { get; set; }

        public virtual ICollection<Emlpoyee> Emlpoyees { get; set; }
    }
}
