﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataPortalInstrumentation.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    public Csla.Server.Dashboard.IDashboard Get()
    {
      return Csla.Server.Dashboard.DashboardFactory.GetDashboard();
    }
  }
}