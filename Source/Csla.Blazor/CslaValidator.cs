using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Csla.Blazor
{

  /// <summary>
  /// Class offering integration of CSLA validation into Blazor edit forms
  /// This is a component with no visible behaviour; it simply requests hook up of validation
  /// </summary>
  public class CslaValidator : ComponentBase
  {
    
    /// <summary>
    /// Gets or sets the current edit context for the component
    /// </summary>
    [CascadingParameter]
    public EditContext CurrentEditContext { get; set; }

    /// <summary>
    /// Override to do work after initialization
    /// </summary>
    protected override void OnInitialized()
    {
      // Check that the EditContext parameter has been made available
      if (CurrentEditContext == null)
      {
        // No cascading parameter is available; we are probably not inside an EditForm component
        throw new InvalidOperationException(
          string.Format(Csla.Properties.Resources.CascadingEditContextRequiredException,
          nameof(CslaValidator), nameof(EditContext)));
      }

      // Wire up validation to the context we have been provided
      CurrentEditContext.AddCslaValidation();
    }
  }
}
