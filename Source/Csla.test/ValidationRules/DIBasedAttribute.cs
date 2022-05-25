using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csla.Test.ValidationRules
{

  /// <summary>
  /// Test validation attribute that makes use of DI to create an instance of something
  /// </summary>
  internal class DIBasedTestAttribute : ValidationAttribute
  {
    public DIBasedTestAttribute() : base("Unknown validation error") { }

    public override bool RequiresValidationContext => true;

    /// <summary>
    /// Override for the IsValid method, used to perform the appropriate validation work
    /// </summary>
    /// <param name="value">The value that is to be validated</param>
    /// <param name="validationContext">The context within which we are to perform validation</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Validation context is not provided</exception>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      ApplicationContext applicationContext;

      if (value is null) return ValidationResult.Success;
      if (validationContext is null) throw new ArgumentNullException(nameof(validationContext));

      applicationContext = (ApplicationContext)validationContext.GetService(typeof(ApplicationContext));

      if (applicationContext is null)
      {
        return new ValidationResult("Service provider failed to create the appropriate class!");
      }

      return ValidationResult.Success;
    }
  }
}
