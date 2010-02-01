using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Model binder for use with CSLA .NET editable business
  /// objects.
  /// </summary>
  public class CslaModelBinder : DefaultModelBinder
  {
    /// <summary>
    /// Creates an instance of the model if the controller implements
    /// IModelCreator.
    /// </summary>
    /// <param name="controllerContext">Controller context</param>
    /// <param name="bindingContext">Binding context</param>
    /// <param name="modelType">Type of model object</param>
    protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
    {
      var controller = controllerContext.Controller as IModelCreator;
      if (controller != null)
        return controller.CreateModel(modelType);
      else
        return base.CreateModel(controllerContext, bindingContext, modelType);
    }

    /// <summary>
    /// Checks the validation rules for properties
    /// after the Model has been updated.
    /// </summary>
    /// <param name="controllerContext">Controller context</param>
    /// <param name="bindingContext">Binding context</param>
    protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var obj = bindingContext.Model as Csla.Core.BusinessBase;
      if (obj != null)
      {
        var errors = from r in obj.BrokenRulesCollection
                     where r.Severity == Csla.Validation.RuleSeverity.Error
                     select r;
        foreach (var item in errors)
        {
          bindingContext.ModelState.AddModelError(item.Property, item.Description);
          //bindingContext.ModelState.SetModelValue(item.Property, bindingContext.ValueProvider.GetValue(controllerContext, item.Property));
          bindingContext.ModelState.SetModelValue(item.Property, bindingContext.ValueProvider[item.Property]);
        }
      }
      else
        base.OnModelUpdated(controllerContext, bindingContext);
    }

    /// <summary>
    /// Prevents IDataErrorInfo validation from
    /// operating against editable objects.
    /// </summary>
    /// <param name="controllerContext">Controller context</param>
    /// <param name="bindingContext">Binding context</param>
    /// <param name="propertyDescriptor">Property descriptor</param>
    /// <param name="value">Value</param>
    protected override void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, object value)
    {
      if (!(bindingContext.Model is Csla.Core.BusinessBase))
        base.OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, value);
    }
  }
}
