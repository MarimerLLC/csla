using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CslaMVC.Web.Models
{
    public class LogOnModel
    {
        public string UserName { get; set; }
        public string Roles { get; set; }
        public bool RememberMe { get; set; }
    }
}