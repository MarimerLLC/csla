using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if SILVERLIGHT
namespace Csla.Silverlight
#else
namespace Csla.Wpf
#endif
{
  /// <summary>
  /// Interface defining the interaction between
  /// a CslaDataSource and an error dialog control.
  /// </summary>
  public interface IErrorDialog
  {
    /// <summary>
    /// Method called by the CslaDataProvider when the
    /// error dialog should register any events it
    /// wishes to handle from the CslaDataProvider.
    /// </summary>
    /// <param name="source">Data provider control.</param>
    void Register(object source);
  }
}
