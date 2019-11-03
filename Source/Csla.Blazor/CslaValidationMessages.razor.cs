using Csla.Core;
using Csla.Rules;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Blazor
{
	public class CslaValidationMessageBase : ComponentBase, IDisposable
	{

		protected bool _validationInitiated = false;
		private EditContext _previousEditContext;
		private EventHandler<FieldChangedEventArgs> _fieldChangedHandler;
		private EventHandler<ValidationStateChangedEventArgs> _validationStateChangedHandler;

		[Parameter] public string PropertyName { get; set; }
    [Parameter] public string WrapperId { get; set; } = "wrapper";
    [Parameter] public string WrapperClass { get; set; } = "validation-messages";
		[Parameter] public string ErrorClass { get; set; } = "text-danger";
		[Parameter] public string WarningClass { get; set; } = "text-warning";
		[Parameter] public string InfoClass { get; set; } = "text-info";
    [Parameter] public string ErrorWrapperClass { get; set; } = "error-messages";
    [Parameter] public string WarningWrapperClass { get; set; } = "warning-messages";
    [Parameter] public string InformationWrapperClass { get; set; } = "information-messages";

    [CascadingParameter] protected EditContext CurrentEditContext { get; set; }

		#region Event Handlers
		
		protected override void OnInitialized()
		{
			// Initialise event handler delegates for use in capturing state changes
			_fieldChangedHandler = (sender, eventArgs) => OnFieldChanged(sender, eventArgs);
			_validationStateChangedHandler = (sender, eventArgs) => OnValidationStateChanged(sender, eventArgs);
		}

		protected override void OnParametersSet()
		{
			// Check that the required parameters have been provided
			if (CurrentEditContext == null)
			{
				throw new InvalidOperationException(
          string.Format(Csla.Properties.Resources.CascadingEditContextRequiredException,
          nameof(CslaValidationMessages), nameof(EditContext)));

      }

      if (string.IsNullOrWhiteSpace(PropertyName))
			{
				throw new InvalidOperationException(
          string.Format(Csla.Properties.Resources.ParameterRequiredException, 
          nameof(CslaValidationMessages), nameof(PropertyName)));
			}

			// Wire up handling of the state change events we need event
			if (CurrentEditContext != _previousEditContext)
			{
				DetachPreviousEventHandlers();
				CurrentEditContext.OnFieldChanged += _fieldChangedHandler;
				CurrentEditContext.OnValidationStateChanged += _validationStateChangedHandler;
				_previousEditContext = CurrentEditContext;
			}
		}

		protected void OnFieldChanged(object sender, FieldChangedEventArgs eventArgs)
		{
			if (eventArgs.FieldIdentifier.FieldName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase))
			{
				_validationInitiated = true;
				StateHasChanged();
			}
		}

		protected void OnValidationStateChanged(object sender, ValidationStateChangedEventArgs eventArgs)
		{
			IEnumerable<string> messages;

			// Retrieve the messages for the property in which we are interested
			messages = CurrentEditContext.GetValidationMessages(CurrentEditContext.Field(PropertyName));

			if (messages.Any())
			{
				// If any messages are present then validation has been run
				// This is a cheat used to identify that the form has been submitted with invalid data
				_validationInitiated = true;
			}

			StateHasChanged();
		}

		protected void DetachPreviousEventHandlers()
		{
			if (_previousEditContext != null)
			{
				// Unhook any event handler made to a different edit context
				_previousEditContext.OnFieldChanged -= _fieldChangedHandler;
				_previousEditContext.OnValidationStateChanged -= _validationStateChangedHandler;
			}
		}

		#endregion

		#region Message Retrieval

		protected IEnumerable<string> GetErrorMessages()
		{
			return GetBrokenRuleMessages(PropertyName, RuleSeverity.Error);
		}

		protected IEnumerable<string> GetWarningMessages()
		{
			return GetBrokenRuleMessages(PropertyName, RuleSeverity.Warning);
		}

		protected IEnumerable<string> GetInfoMessages()
		{
			return GetBrokenRuleMessages(PropertyName, RuleSeverity.Information);
		}

		private IEnumerable<string> GetBrokenRuleMessages(string propertyName, RuleSeverity severity)
		{
			IList<string> messages = new List<string>();
			ICheckRules objectUnderTest;

			// Attempt to gain access to the underlying CSLA object
			objectUnderTest = CurrentEditContext.Model as ICheckRules;
			if (objectUnderTest == null)
			{
        throw new ArgumentException("Model");
      }

			// Iterate through the broken rules to find the subset we want
			foreach (BrokenRule rule in objectUnderTest.GetBrokenRules())
			{
				// Exclude any broken rules that are not for the property we are interested in
				if (rule.Property.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase))
				{
					// Exclude any of a severity other than that we want
					if (rule.Severity == severity)
					{
						// Rule meets our criteria, so add its text to the list we are to return
						messages.Add(rule.Description);
					}
				}
			}

			// Return the list of messages that matched the criteria
			return messages;
		}

		#endregion

		#region IDisposable Interface

		void IDisposable.Dispose()
		{
			DetachPreviousEventHandlers();
			Dispose(disposing: true);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		#endregion

	}
}
