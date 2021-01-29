//-----------------------------------------------------------------------
// <copyright file="CslaModelBinder.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Model binder for use with CSLA .NET editable business objects.</summary>
//-----------------------------------------------------------------------
#if NETSTANDARD2_0 || NET5_0 || NETCORE3_1
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Model binder for use with CSLA .NET editable business
  /// objects.
  /// </summary>
  public class CslaModelBinder : Server.ObjectFactory, IModelBinder
  {
    /// <summary>
    /// Creates a model binder with an instance creator for root objects.
    /// </summary>
    /// <param name="instanceCreator">Instance creator for root objects.</param>
    public CslaModelBinder(Func<Type, Task<object>> instanceCreator)
    {
      _instanceCreator = instanceCreator;
    }

    /// <summary>
    /// Creates a model binder with instance creators for root and child objects.
    /// </summary>
    /// <param name="instanceCreator">Instance creator for root objects.</param>
    /// <param name="childCreator">Instance creator for child objects.</param>
    public CslaModelBinder(Func<Type, Task<object>> instanceCreator, Func<IList, Type, Dictionary<string, string>, object> childCreator)
    {
      _instanceCreator = instanceCreator;
      _childCreator = childCreator;
    }

    private readonly Func<Type, Task<object>> _instanceCreator;
    private readonly Func<IList, Type, Dictionary<string, string>, object> _childCreator;

    /// <summary>
    /// Bind the form data to a new instance of an IBusinessBase object.
    /// </summary>
    /// <param name="bindingContext">Binding context</param>
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
      if (bindingContext == null)
      {
        throw new ArgumentNullException(nameof(bindingContext));
      }

      var result = await _instanceCreator(bindingContext.ModelType);
      if (result == null)
        return;

      if (typeof(Core.IEditableCollection).IsAssignableFrom(bindingContext.ModelType))
      {
        BindBusinessListBase(bindingContext, result);
      }
      else if (typeof(Core.IEditableBusinessObject).IsAssignableFrom(bindingContext.ModelType))
      {
        BindBusinessBase(bindingContext, result);
      }
      else
      {
        return;
      }

      bindingContext.Result = ModelBindingResult.Success(result);
      return;
    }

    private void BindBusinessBase(ModelBindingContext bindingContext, object result)
    {
      var properties = Core.FieldManager.PropertyInfoManager.GetRegisteredProperties(bindingContext.ModelType);
      foreach (var item in properties)
      {
        string index;
        if (string.IsNullOrEmpty(bindingContext.ModelName))
          index = $"{item.Name}";
        else
          index = $"{bindingContext.ModelName}.{item.Name}";
        BindSingleProperty(bindingContext, result, item, index);
      }
      CheckRules(result);
    }

    private void BindBusinessListBase(ModelBindingContext bindingContext, object result)
    {
      var formKeys = bindingContext.ActionContext.HttpContext.Request.Form.Keys.Where(_ => _.StartsWith(bindingContext.ModelName));
      var childType = Utilities.GetChildItemType(bindingContext.ModelType);
      var properties = Core.FieldManager.PropertyInfoManager.GetRegisteredProperties(childType);
      var list = (IList)result;

      var itemCount = formKeys.Count() / properties.Count();
      for (int i = 0; i < itemCount; i++)
      {
        var child = _childCreator(
          list, childType,
          GetFormValuesForObject(bindingContext.ActionContext.HttpContext.Request.Form, bindingContext.ModelName, i, properties));
        MarkAsChild(child);
        if (child == null)
          throw new InvalidOperationException($"Could not create instance of child type {childType}");
        foreach (var item in properties)
        {
          var index = $"{bindingContext.ModelName}[{i}].{item.Name}";
          BindSingleProperty(bindingContext, child, item, index);
        }
        CheckRules(child);
        if (!list.Contains(child))
          list.Add(child);
      }
    }

    private Dictionary<string, string> GetFormValuesForObject(
              Microsoft.AspNetCore.Http.IFormCollection formData,
              string modelName,
              int index,
              Core.FieldManager.PropertyInfoList properties)
    {
      var result = new Dictionary<string, string>();
      foreach (var item in properties)
      {
        var key = $"{modelName}[{index}].{item.Name}";
        result.Add(item.Name, formData[key]);
      }
      return result;
    }

    private void BindSingleProperty(ModelBindingContext bindingContext, object result, Core.IPropertyInfo item, string index)
    {
      try
      {
        var value = bindingContext.ActionContext.HttpContext.Request.Form[index].FirstOrDefault();
        try
        {
          if (item.Type.Equals(typeof(string)))
            Reflection.MethodCaller.CallPropertySetter(result, item.Name, value);
          else if (value != null)
            Reflection.MethodCaller.CallPropertySetter(result, item.Name, Utilities.CoerceValue(item.Type, value.GetType(), null, value));
          else
            Reflection.MethodCaller.CallPropertySetter(result, item.Name, null);
        }
        catch
        {
          if (item.Type.Equals(typeof(string)))
            LoadProperty(result, item, value);
          else if (value != null)
            LoadProperty(result, item, Utilities.CoerceValue(item.Type, value.GetType(), null, value));
          else
            LoadProperty(result, item, null);
        }
      }
      catch (Exception ex)
      {
        throw new Exception($"Could not map {index} to model", ex);
      }
    }
  }

  /// <summary>
  /// Model binder provider that will use the CslaModelBinder for
  /// any type that implements the IBusinessBase interface.
  /// </summary>
  public class CslaModelBinderProvider : IModelBinderProvider
  {
    /// <summary>
    /// Creates a model binder provider that uses the default
    /// instance and child creators.
    /// </summary>
    public CslaModelBinderProvider()
      : this(CreateInstance, CreateChild)
    { }

    /// <summary>
    /// Creates a model binder provider that use custom
    /// instance and child creators.
    /// </summary>
    /// <param name="instanceCreator">Instance creator for root objects.</param>
    /// <param name="childCreator">Instance creator for child objects.</param>
    public CslaModelBinderProvider(Func<Type, Task<object>> instanceCreator, Func<IList, Type, Dictionary<string, string>, object> childCreator)
    {
      _instanceCreator = instanceCreator;
      _childCreator = childCreator;
    }

    internal static Task<object> CreateInstance(Type type)
    {
      var tcs = new TaskCompletionSource<object>();
      tcs.SetResult(Reflection.MethodCaller.CreateInstance(type));
      return tcs.Task;
    }

    internal static object CreateChild(IList parent, Type type, Dictionary<string, string> values)
    {
      return Reflection.MethodCaller.CreateInstance(type);
    }

    private readonly Func<Type, Task<object>> _instanceCreator;
    private readonly Func<IList, Type, Dictionary<string, string>, object> _childCreator;

    /// <summary>
    /// Gets the CslaModelBinder provider.
    /// </summary>
    /// <param name="context">Model binder provider context.</param>
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
      if (typeof(Core.IEditableCollection).IsAssignableFrom(context.Metadata.ModelType))
        return new CslaModelBinder(_instanceCreator, _childCreator);
      if (typeof(IBusinessBase).IsAssignableFrom(context.Metadata.ModelType))
        return new CslaModelBinder(_instanceCreator);
      return null;
    }
  }
}
#elif !NETSTANDARD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.ComponentModel;
using System.Collections;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Model binder for use with CSLA .NET editable business
  /// objects.
  /// </summary>
  public class CslaModelBinder : DefaultModelBinder
  {
    private bool _checkRulesOnModelUpdated;

    /// <summary>
    /// Creates an instance of the model binder.
    /// </summary>
    /// <param name="CheckRulesOnModelUpdated">Value indicating if business rules will be checked after the model is updated.</param>
    public CslaModelBinder(bool CheckRulesOnModelUpdated = true)
    {
      _checkRulesOnModelUpdated = CheckRulesOnModelUpdated;
    }

    /// <summary>
    /// Binds the model by using the specified controller context and binding context.
    /// </summary>
    /// <param name="controllerContext">Controller Context</param>
    /// <param name="bindingContext">Binding Context</param>
    /// <returns>Bound object</returns>
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      if (typeof(Core.IEditableCollection).IsAssignableFrom((bindingContext.ModelType)))
        return BindCslaCollection(controllerContext, bindingContext);

      var suppress = bindingContext.Model as Core.ICheckRules;
      if (suppress != null)
        suppress.SuppressRuleChecking();
      var result = base.BindModel(controllerContext, bindingContext);
      return result;
    }

    /// <summary>
    /// Bind CSLA Collection object using specified controller context and binding context
    /// </summary>
    /// <param name="controllerContext">Controller Context</param>
    /// <param name="bindingContext">Binding Context</param>
    /// <returns>Bound CSLA collection object</returns>
    private object BindCslaCollection(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      if (bindingContext.Model == null)
        bindingContext.ModelMetadata.Model = CreateModel(controllerContext, bindingContext, bindingContext.ModelType);

      var collection = (IList)bindingContext.Model;
      for (int currIdx = 0; currIdx < collection.Count; currIdx++)
      {
        string subIndexKey = CreateSubIndexName(bindingContext.ModelName, currIdx);
        if (!bindingContext.ValueProvider.ContainsPrefix(subIndexKey))
          continue;      //no value to update skip
        var elementModel = collection[currIdx];
        var suppress = elementModel as Core.ICheckRules;
        if (suppress != null)
          suppress.SuppressRuleChecking();
        var elementContext = new ModelBindingContext()
        {
          ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => elementModel, elementModel.GetType()),
          ModelName = subIndexKey,
          ModelState = bindingContext.ModelState,
          PropertyFilter = bindingContext.PropertyFilter,
          ValueProvider = bindingContext.ValueProvider
        };

        if (OnModelUpdating(controllerContext, elementContext))
        {
          //update element's properties
          foreach (PropertyDescriptor property in GetFilteredModelProperties(controllerContext, elementContext))
          {
            BindProperty(controllerContext, elementContext, property);
          }
          OnModelUpdated(controllerContext, elementContext);
        }
      }

      return bindingContext.Model;
    }

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
      var obj = bindingContext.Model as Core.BusinessBase;
      if (obj != null)
      {
        if (this._checkRulesOnModelUpdated)
        {
          var suppress = obj as Core.ICheckRules;
          if (suppress != null)
          {
            suppress.ResumeRuleChecking();
            suppress.CheckRules();
          }
        }
        var errors = from r in obj.BrokenRulesCollection
                     where r.Severity == Rules.RuleSeverity.Error
                     select r;
        foreach (var item in errors)
        {
          ModelState state;
          string mskey = CreateSubPropertyName(bindingContext.ModelName, item.Property ?? string.Empty);
          if (bindingContext.ModelState.TryGetValue(mskey, out state))
          {
            if (state.Errors.Where(e => e.ErrorMessage == item.Description).Any())
              continue;
            else
              bindingContext.ModelState.AddModelError(mskey, item.Description);
          }
          else if (mskey == string.Empty)
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, item.Description);
        }
      }
      else
        if (!(bindingContext.Model is IViewModel))
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
      if (!(bindingContext.Model is Core.BusinessBase))
        base.OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, value);
    }
  }
}
#endif
