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
    
    [CascadingParameter]
    public EditContext CurrentEditContext { get; set; }

    protected override void OnInitialized()
    {
      // Check that the EditContext parameter has been made available
      if (CurrentEditContext == null)
      {
        // No cascading parameter is available; we are probably not inside an EditForm component
        throw new InvalidOperationException($"{nameof(CslaValidator)} requires a cascading " +
          $"parameter of type {nameof(EditContext)}. For example, you can use {nameof(CslaValidator)} " +
          $"inside an EditForm.");
      }

      // Wire up validation to the context we have been provided
      CurrentEditContext.AddCslaValidation();
    }
  }
}
