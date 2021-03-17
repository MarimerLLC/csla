using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Csla.Blazor
{
  /// <summary>
  /// Exposes metastate for a property.
  /// </summary>
  public interface IPropertyInfo
  {
    /// <summary>
    /// Indicate that all properties have changed
    /// to trigger a UI refresh of all values.
    /// </summary>
    void Refresh();
    /// <summary>
    /// Gets or sets the value of the property
    /// </summary>
    object Value { get; set; }
    /// <summary>
    /// Gets the friendly name for the property.
    /// </summary>
    string FriendlyName { get; }
    /// <summary>
    /// Gets the property name for the property.
    /// </summary>
    string PropertyName { get; }
    /// <summary>
    /// Gets the validation error messages for a
    /// property on the Model
    /// </summary>
    string ErrorText { get; }
    /// <summary>
    /// Gets the validation warning messages for a
    /// property on the Model
    /// </summary>
    string WarningText { get; }
    /// <summary>
    /// Gets the validation information messages for a
    /// property on the Model
    /// </summary>
    string InformationText { get; }
    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to read the property on the Model
    /// </summary>
    bool CanRead { get; }
    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to change the property on the Model
    /// </summary>
    bool CanWrite { get; }
    /// <summary>
    /// Gets a value indicating whether the property 
    /// on the Model is busy
    /// </summary>
    bool IsBusy { get; }
    /// <summary>
    /// Event raised when a property changes.
    /// </summary>
    event PropertyChangedEventHandler PropertyChanged;
  }
}
