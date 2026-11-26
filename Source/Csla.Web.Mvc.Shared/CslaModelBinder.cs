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
using System.Reflection;
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
  /// Only properties with a public, non-<c>init</c> setter are bound, and
  /// values are set through that setter, so authorization rules and setter
  /// logic run. When a posted value can't be converted to the property's
  /// type, the user isn't allowed to change the property, or the setter
  /// throws, the value isn't stored (a value written before the setter
  /// failed is restored) and a ModelState error is added under that field's
  /// key. A read-only property posted back with its current value isn't an
  /// error, provided the user may read the property; otherwise any posted
  /// value is reported, so the response doesn't reveal the current value. Fields listed in <c>__Invariant</c> by the tag helpers, such as
  /// <c>&lt;input type="number"&gt;</c>, are parsed with the invariant culture.
  /// A posted value that converts to null for a not-null property other than
  /// <see cref="string"/> (<see cref="Core.INotNullPropertyInfo"/>) is rejected
  /// by the setter, so it isn't stored and a ModelState error is added. A
  /// not-null property that isn't posted on a new object stays unloaded, without
  /// an error. A null string is stored as <see cref="string.Empty"/>.
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
        var scope = new BindingScope(
          request.HasFormContentType ? request.Form : FormCollection.Empty,
          bindingContext.HttpContext.RequestServices.GetService<IModelMetadataProvider>(),
          bindingContext.ModelState);
        var prefix = bindingContext.ModelName ?? string.Empty;
        if (typeof(Core.IEditableCollection).IsAssignableFrom(modelType))
        {
          BindList(scope, (IList)result, prefix);
          await WaitForIdle(result);
          bindingContext.Result = ModelBindingResult.Success(result);
        }
        else if (typeof(Core.IEditableBusinessObject).IsAssignableFrom(modelType))
        {
          // the root honors the binding context's property filter, which
          // comes from [Bind] or the include list of TryUpdateModelAsync
          var metadata = bindingContext.ModelMetadata.ModelType == result.GetType()
            ? bindingContext.ModelMetadata
            : scope.GetMetadata(result.GetType());
          BindObject(scope, result, prefix, isNew, metadata, bindingContext.PropertyFilter);
          await CheckRulesAsync(result);
          await WaitForIdle(result);
          bindingContext.Result = ModelBindingResult.Success(result);
        }
      }
    }

    private void BindObject(BindingScope scope, object obj, string prefix, bool isNew)
    {
      // nested objects honor the property filter of their own type ([Bind] on the class)
      var metadata = scope.GetMetadata(obj.GetType());
      BindObject(scope, obj, prefix, isNew, metadata, metadata?.PropertyFilterProvider?.PropertyFilter);
    }

    private void BindObject(BindingScope scope, object obj, string prefix, bool isNew, ModelMetadata? metadata, Func<ModelMetadata, bool>? propertyFilter)
    {
      var form = scope.Form;
      var properties = Core.FieldManager.PropertyInfoManager.GetRegisteredProperties(obj.GetType());
      foreach (var item in properties)
      {
        if (!IsBindingAllowed(metadata, propertyFilter, item.Name))
          continue;
        var key = string.IsNullOrEmpty(prefix) ? item.Name : $"{prefix}.{item.Name}";
        if (typeof(Core.IEditableCollection).IsAssignableFrom(item.Type))
        {
          if (HasKeysWithPrefix(form, $"{key}["))
          {
            var childList = ReadProperty(obj, item) ?? CreateChild(obj, item);
            BindList(scope, (IList)childList, key);
          }
        }
        else if (typeof(Core.IEditableBusinessObject).IsAssignableFrom(item.Type))
        {
          if (HasKeysWithPrefix(form, $"{key}."))
          {
            var child = ReadProperty(obj, item);
            var childIsNew = child == null;
            child ??= CreateChild(obj, item);
            BindObject(scope, child, key, childIsNew);
            CheckRules(child);
          }
        }
        else if (typeof(Core.IBusinessObject).IsAssignableFrom(item.Type))
        {
          // read-only or other non-editable child objects aren't bound
        }
        else
        {
          var posted = form.ContainsKey(key);
          if (isNew || posted)
            BindSingleProperty(scope, obj, item, key, posted);
        }
      }
    }

    private void BindList(BindingScope scope, IList list, string prefix)
    {
      var childType = Utilities.GetChildItemType(list.GetType()) ?? throw new InvalidOperationException();
      var allowNew = AllowsNewItems(list);
      foreach (var index in GetIndexes(scope.Form, prefix))
      {
        var key = $"{prefix}[{index}]";
        if (index < list.Count && list[index] is { } existing)
        {
          BindObject(scope, existing, key, false);
          CheckRules(existing);
        }
        else if (allowNew)
        {
          var child = ApplicationContext.CreateInstanceDI(childType) ?? throw new InvalidOperationException($"Could not create instance of child type {childType}");
          MarkAsChild(child);
          BindObject(scope, child, key, true);
          CheckRules(child);
          list.Add(child);
        }
        // otherwise the list doesn't allow new items (AllowNew is false),
        // so items posted beyond the existing ones are ignored
      }
    }

    private static bool AllowsNewItems(IList list) => list switch
    {
      Core.ISupportsAllowNew l => l.AllowNew,
      System.ComponentModel.IBindingList l => l.AllowNew,
      _ => true
    };

    private static bool IsBindingAllowed(ModelMetadata? metadata, Func<ModelMetadata, bool>? propertyFilter, string propertyName)
    {
      var propertyMetadata = metadata?.Properties[propertyName];
      if (propertyMetadata == null)
        return true;
      return propertyMetadata.IsBindingAllowed && (propertyFilter == null || propertyFilter(propertyMetadata));
    }

    private object CreateChild(object parent, Core.IPropertyInfo item)
    {
      var child = ApplicationContext.CreateInstanceDI(item.Type) ?? throw new InvalidOperationException($"Could not create instance of child type {item.Type}");
      MarkAsChild(child);
      LoadProperty(parent, item, child);
      return child;
    }

    private sealed class BindingScope
    {
      public BindingScope(IFormCollection form, IModelMetadataProvider? metadataProvider, ModelStateDictionary modelState)
      {
        Form = form;
        MetadataProvider = metadataProvider;
        ModelState = modelState;
        // the tag helpers list fields rendered with the invariant culture
        // (such as <input type="number">) in __Invariant, as MVC's FormValueProvider expects
        _invariantKeys = new HashSet<string>(
          form.TryGetValue("__Invariant", out var keys) ? keys.Where(k => k is not null).Select(k => k!) : [],
          StringComparer.OrdinalIgnoreCase);
      }

      private readonly HashSet<string> _invariantKeys;

      public bool IsInvariant(string key) => _invariantKeys.Contains(key);

      public IFormCollection Form { get; }
      public ModelStateDictionary ModelState { get; }
      public IModelMetadataProvider? MetadataProvider { get; }

      public ModelMetadata? GetMetadata(Type type) => MetadataProvider?.GetMetadataForType(type);

      private readonly Dictionary<(Type, string), bool> _publicSetters = [];

      [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = Core.TrimmingMessages.MvcJustification)]
      public bool HasPublicSetter(object obj, string propertyName)
      {
        var type = obj.GetType();
        if (!_publicSetters.TryGetValue((type, propertyName), out var result))
        {
          var setter = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)?.SetMethod;
          // an init accessor only sets the value while the object is created
          result = setter is { IsPublic: true } && !IsInitOnly(setter);
          _publicSetters.Add((type, propertyName), result);
        }
        return result;
      }

      private static bool IsInitOnly(System.Reflection.MethodInfo setter)
        => setter.ReturnParameter.GetRequiredCustomModifiers().Any(m => m.FullName == "System.Runtime.CompilerServices.IsExternalInit");
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

    private void BindSingleProperty(BindingScope scope, object obj, Core.IPropertyInfo item, string key, bool posted)
    {
      // a property without a public setter is read-only to the UI
      if (!scope.HasPublicSetter(obj, item.Name))
        return;

      var value = scope.Form[key].FirstOrDefault();
      // an unposted value type on a new object keeps its default
      if (!posted && value is null && item.Type.IsValueType && Nullable.GetUnderlyingType(item.Type) is null)
        return;

      object? newValue;
      try
      {
        if (item.Type.Equals(typeof(string)) || value is null)
          newValue = value;
        else
          newValue = CoerceValue(item.Type, value, scope.IsInvariant(key));
      }
      catch
      {
        // only a posted (non-null) value is converted, so this is user input
        // that doesn't fit the property's type
        AddError(scope, key, GetValueNotAcceptedMessage(item));
        return;
      }

      if (obj is Csla.Security.IAuthorizeReadWrite auth && !IsAllowed(() => auth.CanWriteProperty(item)))
      {
        // a read-only field posted back unchanged isn't an error; the value is
        // only compared when the user may read it, so a guess can't be confirmed
        if (posted)
        {
          var unchanged = IsAllowed(() => auth.CanReadProperty(item))
            && TryReadCurrentValue(obj, item, out var currentValue)
            && IsUnchanged(item, value, newValue, currentValue);
          if (!unchanged)
            AddError(scope, key, Csla.Properties.Resources.PropertySetNotAllowed);
        }
        return;
      }

      var hasOldValue = TryReadCurrentValue(obj, item, out var oldValue);
      try
      {
        Reflection.MethodCaller.CallPropertySetterCore(obj, item.Name, newValue);
      }
      catch (Exception ex)
      {
        // a failure after the field was written (for example in a
        // PropertyChanged handler) leaves the new value, so restore it
        // (a not-null property can't hold the null default of a field that wasn't loaded)
        if (hasOldValue && TryReadCurrentValue(obj, item, out var failedValue) && !Equals(oldValue, failedValue)
          && !(oldValue is null && Core.NotNullGuard.RejectsNull(item)))
          LoadProperty(obj, item, oldValue);

        var cause = GetCause(ex);
        if (posted)
        {
          // the setter rejected the posted value, so it isn't stored
          AddError(scope, key, GetSetFailedMessage(item, cause));
        }
        else if (!IsSecurityException(cause) && cause is not ArgumentException)
        {
          // nothing was posted, so a setter rejecting null leaves the
          // property alone; any other failure is a real error
          throw;
        }
      }
    }

    private static object? CoerceValue(Type type, string value, bool invariant)
    {
      if (!invariant)
        return Utilities.CoerceValue(type, typeof(string), null, value);

      // the field was rendered with the invariant culture (for example
      // <input type="number">), so parse it that way
      var culture = CultureInfo.CurrentCulture;
      CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
      try
      {
        return Utilities.CoerceValue(type, typeof(string), null, value);
      }
      finally
      {
        CultureInfo.CurrentCulture = culture;
      }
    }

    private static bool IsAllowed(Func<bool> authorizationCheck)
    {
      try
      {
        return authorizationCheck();
      }
      catch
      {
        // the check couldn't complete (for example an async rule that's
        // still running), so treat the action as not allowed
        return false;
      }
    }

    /// <summary>
    /// Reads the property's current value without loading its field. A
    /// lazy-loaded property that isn't loaded can't be read. Another
    /// managed field that isn't loaded reads as the property's default
    /// value, which <c>ReadProperty</c> would also store in the field (for
    /// a not-null property without a default, that would store null).
    /// </summary>
    private bool TryReadCurrentValue(object obj, Core.IPropertyInfo item, out object? value)
    {
      if ((item.RelationshipType & RelationshipTypes.PrivateField) != RelationshipTypes.PrivateField && !FieldExists(obj, item))
      {
        if ((item.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad)
        {
          value = null;
          return false;
        }
        value = item.DefaultValue;
        return true;
      }
      value = ReadProperty(obj, item);
      return true;
    }

    /// <summary>
    /// Gets whether a posted value matches the current value, allowing for
    /// what's lost when a value is rendered and posted back.
    /// </summary>
    private static bool IsUnchanged(Core.IPropertyInfo item, string? postedText, object? newValue, object? currentValue)
    {
      if (Equals(newValue, currentValue))
        return true;
      if (item.Type.Equals(typeof(string)))
        return string.IsNullOrEmpty((string?)newValue) && string.IsNullOrEmpty((string?)currentValue);
      if (newValue is DateTime postedDate && currentValue is DateTime currentDate)
        return MatchesAtPostedPrecision(postedDate, currentDate);
      if (newValue is DateTimeOffset postedOffset && currentValue is DateTimeOffset currentOffset)
        return postedOffset.Offset == currentOffset.Offset && MatchesAtPostedPrecision(postedOffset.DateTime, currentOffset.DateTime);
      // the current value may have been rendered in another culture
      return postedText is not null && currentValue is not null
        && (postedText == Convert.ToString(currentValue, CultureInfo.InvariantCulture)
          || postedText == Convert.ToString(currentValue, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Compares a posted date with the current one at the precision the
    /// posted text has: a date input drops the time, a datetime-local
    /// input drops the seconds or milliseconds.
    /// </summary>
    private static bool MatchesAtPostedPrecision(DateTime posted, DateTime current)
    {
      long unit;
      if (posted.Ticks % TimeSpan.TicksPerDay == 0)
        unit = TimeSpan.TicksPerDay;
      else if (posted.Ticks % TimeSpan.TicksPerMinute == 0)
        unit = TimeSpan.TicksPerMinute;
      else if (posted.Ticks % TimeSpan.TicksPerSecond == 0)
        unit = TimeSpan.TicksPerSecond;
      else if (posted.Ticks % TimeSpan.TicksPerMillisecond == 0)
        unit = TimeSpan.TicksPerMillisecond;
      else
        unit = 1;
      return posted.Ticks == current.Ticks - current.Ticks % unit;
    }

    private static void AddError(BindingScope scope, string key, string message)
    {
      // keep what was posted so the form shows it again next to the error
      scope.ModelState.SetModelValue(key, new ValueProviderResult(scope.Form[key], CultureInfo.CurrentCulture));
      scope.ModelState.TryAddModelError(key, message);
    }

    private static Exception GetCause(Exception ex)
    {
      while (ex is TargetInvocationException or PropertyLoadException && ex.InnerException is { } inner)
        ex = inner;
      return ex;
    }

    private static bool IsSecurityException(Exception ex)
      => ex is Csla.Security.SecurityException or System.Security.SecurityException;

    private static string GetSetFailedMessage(Core.IPropertyInfo item, Exception cause)
    {
      if (IsSecurityException(cause))
        return Csla.Properties.Resources.PropertySetNotAllowed;
      // argument and validation exceptions carry messages meant for the user;
      // other exceptions can hold internal details, so they get a generic message
      if (cause is ArgumentException or System.ComponentModel.DataAnnotations.ValidationException)
        return string.Format(CultureInfo.CurrentCulture, Csla.Properties.Resources.ModelBinderPropertySetFailed, item.FriendlyName, cause.Message);
      return GetValueNotAcceptedMessage(item);
    }

    private static string GetValueNotAcceptedMessage(Core.IPropertyInfo item)
      => string.Format(CultureInfo.CurrentCulture, Csla.Properties.Resources.ModelBinderPropertyValueNotAccepted, item.FriendlyName);
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
