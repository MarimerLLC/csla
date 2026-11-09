//-----------------------------------------------------------------------
// <copyright file="CslaModelBinder.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Model binder for use with CSLA .NET editable business objects.</summary>
//-----------------------------------------------------------------------
#if NETSTANDARD2_0 || NET8_0_OR_GREATER 
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Model binder for use with CSLA .NET editable business
  /// objects.
  /// </summary>
  /// <remarks>
  /// The binder walks the posted form keys and binds the whole
  /// editable object graph: properties of the root object, editable
  /// child objects (<c>Address.Street</c>) and editable child lists
  /// (<c>Children[0].GrandChildren[1].Name</c>) at any depth.
  /// When the binding context already holds a model, for example
  /// when the controller calls <c>TryUpdateModelAsync</c> on an object
  /// it fetched, the binder updates that object graph in place and
  /// only sets the values that were posted. Otherwise it creates a
  /// new object graph. New list items are created using the list's
  /// declared child type.
  /// </remarks>
  [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = Core.TrimmingMessages.MvcJustification)]
  [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = Core.TrimmingMessages.MvcJustification)]
  public class CslaModelBinder : Server.ObjectFactory, IModelBinder
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public CslaModelBinder(ApplicationContext applicationContext)
      : base(applicationContext) { }

    /// <summary>
    /// Bind the form data to an IBusinessBase or editable
    /// collection object graph.
    /// </summary>
    /// <param name="bindingContext">Binding context</param>
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
      if (bindingContext == null)
      {
        throw new ArgumentNullException(nameof(bindingContext));
      }

      ApplicationContext = bindingContext.HttpContext.RequestServices.GetRequiredService<ApplicationContext>();

      bindingContext.Result = ModelBindingResult.Failed();
      var modelType = bindingContext.ModelType;
      var result = bindingContext.Model;
      var isNew = false;
      if (result == null || !modelType.IsInstanceOfType(result))
      {
        result = ApplicationContext.CreateInstanceDI(modelType);
        isNew = true;
      }
      if (result != null)
      {
        var request = bindingContext.ActionContext.HttpContext.Request;
        var form = request.HasFormContentType ? request.Form : FormCollection.Empty;
        var prefix = bindingContext.ModelName ?? string.Empty;
        if (typeof(Core.IEditableCollection).IsAssignableFrom(modelType))
        {
          BindList(form, (IList)result, prefix);
          bindingContext.Result = ModelBindingResult.Success(result);
        }
        else if (typeof(Core.IEditableBusinessObject).IsAssignableFrom(modelType))
        {
          BindObject(form, result, prefix, isNew);
          await CheckRulesAsync(result);
          bindingContext.Result = ModelBindingResult.Success(result);
        }
      }
    }

    private void BindObject(IFormCollection form, object obj, string prefix, bool isNew)
    {
      var properties = Core.FieldManager.PropertyInfoManager.GetRegisteredProperties(obj.GetType());
      foreach (var item in properties)
      {
        var key = string.IsNullOrEmpty(prefix) ? item.Name : $"{prefix}.{item.Name}";
        if (typeof(Core.IEditableCollection).IsAssignableFrom(item.Type))
        {
          if (HasKeysWithPrefix(form, $"{key}["))
          {
            var childList = ReadProperty(obj, item) ?? CreateChild(obj, item);
            BindList(form, (IList)childList, key);
          }
        }
        else if (typeof(Core.IEditableBusinessObject).IsAssignableFrom(item.Type))
        {
          if (HasKeysWithPrefix(form, $"{key}."))
          {
            var child = ReadProperty(obj, item);
            var childIsNew = child == null;
            child ??= CreateChild(obj, item);
            BindObject(form, child, key, childIsNew);
            CheckRules(child);
          }
        }
        else if (typeof(Core.IBusinessObject).IsAssignableFrom(item.Type))
        {
          // read-only or other non-editable child objects aren't bound
        }
        else if (isNew || form.ContainsKey(key))
        {
          BindSingleProperty(form, obj, item, key);
        }
      }
    }

    private void BindList(IFormCollection form, IList list, string prefix)
    {
      var childType = Utilities.GetChildItemType(list.GetType()) ?? throw new InvalidOperationException();
      foreach (var index in GetIndexes(form, prefix))
      {
        var key = $"{prefix}[{index}]";
        if (index < list.Count && list[index] is { } existing)
        {
          BindObject(form, existing, key, false);
          CheckRules(existing);
        }
        else
        {
          var child = ApplicationContext.CreateInstanceDI(childType) ?? throw new InvalidOperationException($"Could not create instance of child type {childType}");
          MarkAsChild(child);
          BindObject(form, child, key, true);
          CheckRules(child);
          try
          {
            list.Add(child);
          }
          catch (NotSupportedException)
          {
            // the list doesn't allow new items (AllowNew is false),
            // so items posted beyond the existing ones are ignored
          }
        }
      }
    }

    private object CreateChild(object parent, Core.IPropertyInfo item)
    {
      var child = ApplicationContext.CreateInstanceDI(item.Type) ?? throw new InvalidOperationException($"Could not create instance of child type {item.Type}");
      MarkAsChild(child);
      LoadProperty(parent, item, child);
      return child;
    }

    private static bool HasKeysWithPrefix(IFormCollection form, string prefix)
      => form.Keys.Any(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

    private static SortedSet<int> GetIndexes(IFormCollection form, string prefix)
    {
      var pattern = new Regex($"^{Regex.Escape(prefix)}\\[(\\d+)\\]", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
      var result = new SortedSet<int>();
      foreach (var key in form.Keys)
      {
        var match = pattern.Match(key);
        if (match.Success && int.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var index))
          result.Add(index);
      }
      return result;
    }

    private void BindSingleProperty(IFormCollection form, object result, Core.IPropertyInfo item, string index)
    {
      try
      {
        var value = form[index].FirstOrDefault();
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
}
#elif !NETSTANDARD
using System.Web.Mvc;
using System.ComponentModel;
using System.Collections;
using Csla.Core;

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
      if (typeof(IEditableCollection).IsAssignableFrom((bindingContext.ModelType)))
        return BindCslaCollection(controllerContext, bindingContext);

      if (bindingContext.Model is ICheckRules suppress)
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

      var collection = (IList)bindingContext.Model!;
      for (int currIdx = 0; currIdx < collection.Count; currIdx++)
      {
        string subIndexKey = CreateSubIndexName(bindingContext.ModelName, currIdx);
        if (!bindingContext.ValueProvider.ContainsPrefix(subIndexKey))
          continue;      //no value to update skip
        var elementModel = collection[currIdx];
        if (elementModel is ICheckRules suppress)
          suppress.SuppressRuleChecking();
        var elementContext = new ModelBindingContext
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

      return bindingContext.Model!;
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
      if (controllerContext.Controller is IModelCreator controller)
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
      if (bindingContext.Model is BusinessBase obj)
      {
        if (_checkRulesOnModelUpdated)
        {
          if (obj is ICheckRules suppress)
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
          string mskey = CreateSubPropertyName(bindingContext.ModelName, item.Property ?? string.Empty);
          if (bindingContext.ModelState.TryGetValue(mskey, out var state))
          {
            if (state.Errors.Any(e => e.ErrorMessage == item.Description))
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
    protected override void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
    {
      if (!(bindingContext.Model is BusinessBase))
        base.OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, value);
    }
  }
}
#endif
