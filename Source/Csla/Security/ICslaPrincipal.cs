using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Csla.Security
{
  /// <summary>
  /// Defines the base requirements for the interface of any
  /// CSLA principal object.
  /// </summary>
  public interface ICslaPrincipal : IPrincipal, Csla.Serialization.Mobile.IMobileObject
  {
  }
}
