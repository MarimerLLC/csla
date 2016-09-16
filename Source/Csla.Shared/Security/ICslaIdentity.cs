﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Csla.Security
{
  /// <summary>
  /// Provides a base class to simplify creation of
  /// a .NET identity object for use with CslaPrincipal.
  /// </summary>
  public interface ICslaIdentity
    : IReadOnlyBase, IIdentity, ICheckRoles { }
}
