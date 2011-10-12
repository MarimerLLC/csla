using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Rolodex.Data
{
    public class User
    {
        public int UserID { get; set; }
        [StringLength(20)]
        public string UserLogin { get; set; }
        [StringLength(30)]
        public string UserPassword { get; set; }
        [StringLength(20)]
        public string UserRole { get; set; }
    }
}
