using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Model binder for use with CSLA .NET editable business
  /// objects.
  /// </summary>
  public class CslaModelBinder : Csla.Server.ObjectFactory, IModelBinder
  {
    public CslaModelBinder(Func<Type, Task<object>> instanceCreator)
    {
      _instanceCreator = instanceCreator;
    }

    public CslaModelBinder(Func<Type, Task<object>> instanceCreator, Func<IList, Type, Dictionary<string, string>, object> childCreator)
    {
      _instanceCreator = instanceCreator;
      _childCreator = childCreator;
    }

    private readonly Func<Type, Task<object>> _instanceCreator;
    private readonly Func<IList, Type, Dictionary<string,string>, object> _childCreator;

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

      if (typeof(Csla.Core.IEditableCollection).IsAssignableFrom(bindingContext.ModelType))
      {
        BindBusinessListBase(bindingContext, result);
      }
      else if (typeof(Csla.Core.IEditableBusinessObject).IsAssignableFrom(bindingContext.ModelType))
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
      var properties = Csla.Core.FieldManager.PropertyInfoManager.GetRegisteredProperties(bindingContext.ModelType);
      foreach (var item in properties)
      {
        var index = $"{bindingContext.ModelName}.{item.Name}";
        BindSingleProperty(bindingContext, result, item, index);
      }
    }

    private void BindBusinessListBase(ModelBindingContext bindingContext, object result)
    {
      var formKeys = bindingContext.ActionContext.HttpContext.Request.Form.Keys.Where(_ => _.StartsWith(bindingContext.ModelName));
      var childType = Csla.Utilities.GetChildItemType(bindingContext.ModelType);
      var properties = Csla.Core.FieldManager.PropertyInfoManager.GetRegisteredProperties(childType);
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
        if (!string.IsNullOrWhiteSpace(value))
        {
          try
          {
            if (item.Type.Equals(typeof(string)))
              Csla.Reflection.MethodCaller.CallPropertySetter(result, item.Name, value);
            else
              Csla.Reflection.MethodCaller.CallPropertySetter(result, item.Name, Csla.Utilities.CoerceValue(item.Type, value.GetType(), null, value));
          }
          catch
          {
            if (item.Type.Equals(typeof(string)))
              LoadProperty(result, item, value);
            else
              LoadProperty(result, item, Csla.Utilities.CoerceValue(item.Type, value.GetType(), null, value));
          }
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
    public CslaModelBinderProvider()
      : this(CreateInstance, CreateChild)
    { }

    public CslaModelBinderProvider(Func<Type, Task<object>> instanceCreator, Func<IList, Type, Dictionary<string, string>, object> childCreator)
    {
      _instanceCreator = instanceCreator;
      _childCreator = childCreator;
    }

    internal static Task<object> CreateInstance(Type type)
    {
      var tcs = new TaskCompletionSource<object>();
      tcs.SetResult(Csla.Reflection.MethodCaller.CreateInstance(type));
      return tcs.Task;
    }

    internal static object CreateChild(IList parent, Type type, Dictionary<string, string> values)
    {
      return Csla.Reflection.MethodCaller.CreateInstance(type);
    }

    private readonly Func<Type, Task<object>> _instanceCreator;
    private readonly Func<IList, Type, Dictionary<string, string>, object> _childCreator;

    /// <summary>
    /// Gets the CslaModelBinder provider.
    /// </summary>
    /// <param name="context">Model binder provider context.</param>
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
      if (typeof(Csla.IBusinessBase).IsAssignableFrom(context.Metadata.ModelType))
        return new CslaModelBinder(_instanceCreator);
      if (typeof(Csla.Core.IEditableCollection).IsAssignableFrom(context.Metadata.ModelType))
        return new CslaModelBinder(_instanceCreator, _childCreator);

      return null;
    }
  }
}
