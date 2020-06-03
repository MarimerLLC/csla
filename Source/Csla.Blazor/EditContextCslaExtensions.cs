using System;
using System.Collections.Generic;
using System.Text;
using Csla.Core;
using Csla.Rules;
using Microsoft.AspNetCore.Components.Forms;

namespace Csla.Blazor
{
  /// <summary>
  /// Implements extension methods for edit context.
  /// </summary>
  public static class EditContextCslaExtensions
  {
    /// <summary>
    /// Adds validation support to the <see cref="EditContext"/> for objects implementing ICheckRules.
    /// </summary>
    /// <param name="editContext">The <see cref="EditContext"/>.</param>
    public static EditContext AddCslaValidation(this EditContext editContext)
    {
      if (editContext == null)
      {
        throw new ArgumentNullException(nameof(editContext));
      }

      var messages = new ValidationMessageStore(editContext);

      // Perform object-level validation on request
      editContext.OnValidationRequested +=
          (sender, eventArgs) => ValidateModel((EditContext)sender, messages);

      // Perform per-field validation on each field edit
      editContext.OnFieldChanged +=
          (sender, eventArgs) => ValidateField(editContext, messages, eventArgs.FieldIdentifier);

      return editContext;
    }

    /// <summary>
    /// Method to perform validation on the model as a whole
    /// Applies changes to the validation store provided as a parameter
    /// </summary>
    /// <param name="editContext">The EditContext provided by the form doing the editing</param>
    /// <param name="messages">The validation message store to be updated during validation</param>
    private static void ValidateModel(EditContext editContext, ValidationMessageStore messages)
    {
      ICheckRules model;

      // Get access to the model via the required interface
      model = editContext.Model as ICheckRules;

      // Check if the model was provided, and correctly cast
      if (editContext.Model == null)
      {
        throw new ArgumentNullException(nameof(editContext.Model));
      }
      if (model == null)
      {
        throw new ArgumentException(
          string.Format(Csla.Properties.Resources.InterfaceNotImplementedException, 
          nameof(editContext.Model), nameof(ICheckRules)));
      }

      // Transfer broken rules of severity Error to the ValidationMessageStore
      messages.Clear();
      foreach (var brokenRuleNode in BusinessRules.GetAllBrokenRules(model))
      {
        foreach (var brokenRule in brokenRuleNode.BrokenRules)
        if (brokenRule.Severity == RuleSeverity.Error)
        {
          // Add a new message for each broken rule
          messages.Add(new FieldIdentifier(brokenRuleNode.Object, brokenRule.Property), brokenRule.Description);
        }
      }

      // Inform consumers that the state may have changed
      editContext.NotifyValidationStateChanged();
    }

    /// <summary>
    /// Method to perform validation on a single property of the model being edit
    /// Applies changes to the validation store provided as a parameter
    /// </summary>
    /// <param name="editContext">The EditContext provided by the form doing the editing</param>
    /// <param name="messages">The validation message store to be updated during validation</param>
    /// <param name="fieldIdentifier">Identifier that indicates the field being validated</param>
    private static void ValidateField(EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier)
    {
      ICheckRules model;

      // Get access to the model via the required interface
      model = fieldIdentifier.Model as ICheckRules;

      // Check if the model was provided, and correctly cast
      if (model == null)
      {
        throw new ArgumentException(
          string.Format(Csla.Properties.Resources.InterfaceNotImplementedException,
          nameof(fieldIdentifier.Model), nameof(ICheckRules)));
      }

      // Transfer any broken rules of severity Error for the required property to the store
      messages.Clear(fieldIdentifier);
      foreach (BrokenRule brokenRule in model.GetBrokenRules())
      {
        if (brokenRule.Severity == RuleSeverity.Error)
        {
          if (fieldIdentifier.FieldName.Equals(brokenRule.Property))
          {
            // Add a message for each broken rule on the property under validation
            messages.Add(fieldIdentifier, brokenRule.Description);
          }
        }
      }

      // We have to notify even if there were no messages before and are still no messages now,
      // because the "state" that changed might be the completion of some async validation task
      editContext.NotifyValidationStateChanged();
    }
  }
}
