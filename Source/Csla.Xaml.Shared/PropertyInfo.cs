﻿//-----------------------------------------------------------------------
// <copyright file="PropertyInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Expose metastate information about a property.</summary>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.ComponentModel;
using Csla.Core;
using Csla.Reflection;
using Csla.Rules;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#elif XAMARIN
using Xamarin.Forms;
#elif MAUI
#else
using System.Windows;
using System.Windows.Data;
#endif

namespace Csla.Xaml
{
  /// <summary>
  /// Expose metastate information about a property.
  /// </summary>
#if MAUI
  public class PropertyInfo : ContentView, INotifyPropertyChanged
  {
#elif XAMARIN
  public class PropertyInfo : View, INotifyPropertyChanged
  {

#else
  public class PropertyInfo : FrameworkElement, INotifyPropertyChanged
  {
    private const string _dependencyPropertySuffix = "Property";
#endif
    private bool _loading = true;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public PropertyInfo()
    {
      SetValue(BrokenRulesProperty, new ObservableCollection<BrokenRule>());
#if XAMARIN || MAUI
      _loading = false;
      BindingContextChanged += (o, e) => SetSource();
      UpdateState();
#else
      Visibility = Visibility.Collapsed;
      Height = 20;
      Width = 20;
      Loaded += (_, _) =>
      {
        _loading = false;
        UpdateState();
      };
#endif
    }

    /// <summary>
    /// Creates an instance of the object for testing.
    /// </summary>
    /// <param name="testing">Test mode parameter.</param>
    public PropertyInfo(bool testing)
      : this()
    {
      _ = testing;
      _loading = false;
      UpdateState();
    }

    #region BrokenRules property

#if XAMARIN || MAUI
    /// <summary>
    /// Gets the broken rules collection from the
    /// business object.
    /// </summary>
    public static readonly BindableProperty BrokenRulesProperty =
      BindableProperty.Create(nameof(BrokenRules), typeof(ObservableCollection<BrokenRule>), typeof(PropertyInfo), new ObservableCollection<BrokenRule>());
    /// <summary>
    /// Gets the broken rules collection from the
    /// business object.
    /// </summary>
    /// <exception cref="ArgumentNullException"><see cref="BrokenRules"/> is <see langword="null"/>.</exception>
    public ObservableCollection<BrokenRule> BrokenRules
    {
      get => (ObservableCollection<BrokenRule>)this.GetValue(BrokenRulesProperty);
      set => SetValue(BrokenRulesProperty, value ?? throw new ArgumentNullException(nameof(BrokenRules)));
    }
#else
    /// <summary>
    /// Gets the broken rules collection from the
    /// business object.
    /// </summary>
    public static readonly DependencyProperty BrokenRulesProperty = DependencyProperty.Register(
      nameof(BrokenRules),
      typeof(ObservableCollection<BrokenRule>),
      typeof(PropertyInfo),
      null);

    /// <summary>
    /// Gets the broken rules collection from the
    /// business object.
    /// </summary>
    [Category("Property Status")]
    public ObservableCollection<BrokenRule> BrokenRules
    {
      get { return (ObservableCollection<BrokenRule>)GetValue(BrokenRulesProperty); }
    }
#endif

    #endregion

    #region MyDataContext Property

#if XAMARIN || MAUI
#else
    /// <summary>
    /// Used to monitor for changes in the binding path.
    /// </summary>
    public static readonly DependencyProperty MyDataContextProperty =
    DependencyProperty.Register("MyDataContext",
                                typeof(Object),
                                typeof(PropertyInfo),
#if NETFX_CORE
                                new PropertyMetadata(null, MyDataContextPropertyChanged));
#else
                                new PropertyMetadata(MyDataContextPropertyChanged));
#endif

    private static void MyDataContextPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      ((PropertyInfo)sender).SetSource(true);
    }
#endif

    #endregion

    #region RelativeBinding Property

#if XAMARIN || MAUI
#else

    /// <summary>
    /// Used to monitor for changes in the binding path.
    /// </summary>
    public static readonly DependencyProperty RelativeBindingProperty =
    DependencyProperty.Register("RelativeBinding",
                                typeof(Object),
                                typeof(PropertyInfo),
#if NETFX_CORE
                                new PropertyMetadata(null, RelativeBindingPropertyChanged));
#else
                                new PropertyMetadata(RelativeBindingPropertyChanged));
#endif

    private static void RelativeBindingPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      ((PropertyInfo)sender).SetSource(true);
    }
#endif

    #endregion

    #region Source property

    /// <summary>
    /// Gets or sets the Source.
    /// </summary>
    /// <value>The source.</value>
    protected object? Source { get; set; }

    /// <summary>
    /// Gets or sets the binding path.
    /// </summary>
    /// <value>The binding path.</value>
    protected string BindingPath { get; set; } = string.Empty;

#if XAMARIN || MAUI
    private string? _bindingPath;
    /// <summary>
    /// Gets or sets the binding path used to bind this
    /// control to the business object property relative
    /// to the BindingContext.
    /// </summary>
    public string? Path
    {
      get => _bindingPath;
      set
      {
        if (_bindingPath != value)
        {
          _bindingPath = value;
          OnPropertyChanged();
          SetSource();
        }
      }
    }

    private class SourceReference
    {
      public SourceReference(PropertyInfo parent, object source, string propertyName)
      {
        Parent = parent;
        Source = source;
        PropertyName = propertyName;
        var p = Source as INotifyPropertyChanged;
        if (p != null)
          p.PropertyChanged += P_PropertyChanged;
      }

      private void P_PropertyChanged(object? sender, PropertyChangedEventArgs e)
      {
        if (e.PropertyName == PropertyName)
          Parent.SetSource();
      }

      public void DetachHandlers()
      {
        var p = Source as INotifyPropertyChanged;
        if (p != null)
          p.PropertyChanged -= P_PropertyChanged;
      }

      public PropertyInfo Parent { get; }
      public object Source { get; }
      public string PropertyName { get; }
    }

    private readonly List<SourceReference> _sources = new();

    private void SetSource()
    {
      foreach (var item in _sources)
        item.DetachHandlers();
      _sources.Clear();
      var oldSource = Source;
      Source = BindingContext;
      BindingPath = Path ?? "";
      if (Source != null && !string.IsNullOrWhiteSpace(BindingPath))
      {
        try
        {
          while (BindingPath.Contains(".") && Source != null)
          {
            var dotIndex = BindingPath.IndexOf('.');
            var refName = BindingPath.Substring(0, dotIndex);
            _sources.Add(new SourceReference(this, Source, refName));
            BindingPath = BindingPath.Substring(dotIndex + 1);
            Source = MethodCaller.CallPropertyGetter(Source, refName);
          }
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException(
            string.Format("SetSource: BindingContext:{0}, Path={1}", BindingPath.GetType().Name, Path), ex);
        }
      }
      HandleSourceEvents(oldSource, Source);
      UpdateState();
      OnPropertyChanged(nameof(Value));
    }
#else
    /// <summary>
    /// Gets or sets the source business
    /// property to which this control is bound.
    /// </summary>
    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
      nameof(Property),
      typeof(object),
      typeof(PropertyInfo),
      new PropertyMetadata(new object(), (o, e) =>
      {
        bool changed = true;
        if (e.NewValue == null)
        {
          if (e.OldValue == null)
            changed = false;
        }
        else if (e.NewValue.Equals(e.OldValue))
        {
          changed = false;
        }
        ((PropertyInfo)o).SetSource(changed);
      }));

    /// <summary>
    /// Gets or sets the source business
    /// property to which this control is bound.
    /// </summary>
    [Category("Common")]
    public object? Property
    {
      get { return GetValue(PropertyProperty); }
      set
      {
        SetValue(PropertyProperty, value);
        SetSource(false);
      }
    }

    /// <summary>
    /// Checks a binding expression to see if it is a relative source binding used in a control template.
    /// </summary>
    /// <param name="sourceBinding">The binding expression to parse.</param>
    /// <returns>If the source binding is a relative source binding, this method 
    /// finds the proper dependency property on the parent control and returns
    /// the binding expression for that property.</returns>
    protected virtual BindingExpression? ParseRelativeBinding(BindingExpression? sourceBinding)
    {
      if (sourceBinding != null
        && sourceBinding.ParentBinding.RelativeSource != null
        && sourceBinding.ParentBinding.RelativeSource.Mode == RelativeSourceMode.TemplatedParent
        && sourceBinding.DataItem is FrameworkElement control)
      {
        var path = sourceBinding.ParentBinding.Path.Path;

        var type = control.GetType();
        while (type != null)
        {
          var name = $"{path}{_dependencyPropertySuffix}";
#if NETFX_CORE
          var fi = type.GetField(name, BindingFlags.Instance | BindingFlags.Public);
#else
          var fi = type.GetField(name);
#endif

          if (fi != null)
          {
            var mappedDP = (DependencyProperty?)fi.GetValue(control.GetType());
            return control.GetBindingExpression(mappedDP);
          }
          else
          {
#if NETFX_CORE
            type = type.GetTypeInfo().BaseType;
#else
            type = type.BaseType;
#endif
          }
        }

        return null;
      }

      return sourceBinding;
    }

    /// <summary>
    /// Sets the source binding and updates status.
    /// </summary>
    protected virtual void SetSource(bool propertyValueChanged)
    {
      var binding = GetBindingExpression(PropertyProperty);
      if (binding != null)
      {
        SetSource(binding.DataItem);
      }
    }

    /// <summary>
    /// Sets the source binding and updates status.
    /// </summary>
    protected virtual void SetSource(object? dataItem)
    {
      bool isDataLoaded = true;

      SetBindingValues(GetBindingExpression(PropertyProperty));
      var newSource = GetRealSource(dataItem, BindingPath);

      // Check to see if PropertyInfo is inside a control template
      ClearValue(MyDataContextProperty);
      if (newSource is FrameworkElement element)
      {
        var data = element.DataContext;
        SetBindingValues(ParseRelativeBinding(GetBindingExpression(PropertyProperty)));

        if (data != null && GetBindingExpression(RelativeBindingProperty) == null)
        {
          var relativeBinding = ParseRelativeBinding(GetBindingExpression(PropertyProperty));
          if (relativeBinding != null)
            SetBinding(RelativeBindingProperty, relativeBinding.ParentBinding);
        }

        newSource = GetRealSource(data, BindingPath);

        if (newSource != null)
        {
          Binding b = new Binding();
          b.Source = newSource;
          if (BindingPath.IndexOf('.') > 0)
          {
            var path = GetRelativePath(newSource, BindingPath);
            if (path != null)
              b.Path = path;
          }
          if (b.Path != null
              && !string.IsNullOrEmpty(b.Path.Path)
              && b.Path.Path != BindingPath.Substring(BindingPath.LastIndexOf('.') + 1))
          {
            SetBinding(MyDataContextProperty, b);
            isDataLoaded = false;
          }
        }
      }

      if (BindingPath.Contains('.'))
        BindingPath = BindingPath.Substring(BindingPath.LastIndexOf('.') + 1);

      if (isDataLoaded)
      {
        if (!ReferenceEquals(Source, newSource))
        {
          var old = Source;
          Source = newSource;

          HandleSourceEvents(old, Source);
        }

        UpdateState();
      }
    }

    /// <summary>
    /// Sets the binding values for this instance.
    /// </summary>
    private void SetBindingValues(BindingExpression? binding)
    {
      var bindingPath = string.Empty;

      if (binding != null)
      {
        if (binding.ParentBinding != null && binding.ParentBinding.Path != null)
          bindingPath = binding.ParentBinding.Path.Path;
        else
          bindingPath = string.Empty;
      }

      BindingPath = bindingPath;
    }

    /// <summary>
    /// Gets the real source helper method.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="bindingPath">The binding path.</param>
    /// <exception cref="ArgumentNullException"><paramref name="bindingPath"/> is <see langword="null"/>.</exception>
    protected object? GetRealSource(object? source, string bindingPath)
    {
      if (bindingPath is null)
        throw new ArgumentNullException(nameof(bindingPath));

      if (source is ICollectionView icv)
        source = icv.CurrentItem;
      var dotIndex = bindingPath.IndexOf('.');
      if (source != null && dotIndex > 0)
      {
        var firstProperty = bindingPath.Substring(0, dotIndex);
        var p = MethodCaller.GetProperty(source.GetType(), firstProperty);
        if (p != null)
        {
          source = GetRealSource(
           MethodCaller.GetPropertyValue(source, p),
           bindingPath.Substring(dotIndex + 1));
        }
      }

      return source;
    }

    /// <summary>
    /// Gets the part of the binding path relevant to the given source.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="bindingPath">The binding path.</param>
    /// <exception cref="ArgumentNullException"><paramref name="bindingPath"/> is <see langword="null"/>.</exception>
    protected PropertyPath? GetRelativePath(object? source, string bindingPath)
    {
      if (source != null)
      {
        if (bindingPath is null)
          throw new ArgumentNullException(nameof(bindingPath));

        var dotIndex = bindingPath.IndexOf('.');
        if (dotIndex > 0)
        {
          var firstProperty = bindingPath.Substring(0, dotIndex);
          var p = MethodCaller.GetProperty(source.GetType(), firstProperty);

          if (p != null)
            return new PropertyPath(firstProperty);
          else
            return GetRelativePath(source, bindingPath.Substring(dotIndex + 1));
        }
        else
          return new PropertyPath(bindingPath);
      }

      return null;
    }
#endif

    private void HandleSourceEvents(object? old, object? source)
    {
      if (!ReferenceEquals(old, source))
      {
        DetachSource(old);
        AttachSource(source);
        if (Source is BusinessBase bb && !string.IsNullOrWhiteSpace(BindingPath))
        {
          IsBusy = bb.IsPropertyBusy(BindingPath);
        }
      }
    }

    private void DetachSource(object? source)
    {
      if (source is INotifyPropertyChanged p)
        p.PropertyChanged -= source_PropertyChanged;
      if (source is INotifyBusy busy)
        busy.BusyChanged -= source_BusyChanged;
    }

    private void AttachSource(object? source)
    {
      if (source is INotifyPropertyChanged p)
        p.PropertyChanged += source_PropertyChanged;
      if (source is INotifyBusy busy)
        busy.BusyChanged += source_BusyChanged;
    }

    void source_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == BindingPath || string.IsNullOrEmpty(e.PropertyName))
      {
        OnPropertyChanged(nameof(Value));
        UpdateState();
      }
    }

    void source_BusyChanged(object? sender, BusyChangedEventArgs e)
    {
      if (e.PropertyName == BindingPath || string.IsNullOrEmpty(e.PropertyName))
      {
        bool busy = e.Busy;
        if (Source is BusinessBase bb)
          busy = bb.IsPropertyBusy(BindingPath);

        if (busy != IsBusy)
        {
          IsBusy = busy;
          UpdateState();
        }
      }
    }

    #endregion

    #region Error/Warn/Info Text

    /// <summary>
    /// Gets the validation error messages for a
    /// property on the Model
    /// </summary>
    public string ErrorText
    {
      get
      {
        var result = string.Empty;
        if (Source is BusinessBase obj && !string.IsNullOrEmpty(BindingPath))
          result = obj.BrokenRulesCollection.ToString(",", RuleSeverity.Error, BindingPath);
        return result;
      }
    }

    /// <summary>
    /// Gets the validation warning messages for a
    /// property on the Model
    /// </summary>
    public string WarningText
    {
      get
      {
        var result = string.Empty;
        if (Source is BusinessBase obj && !string.IsNullOrEmpty(BindingPath))
          result = obj.BrokenRulesCollection.ToString(",", RuleSeverity.Warning, BindingPath);
        return result;
      }
    }

    /// <summary>
    /// Gets the validation information messages for a
    /// property on the Model
    /// </summary>
    public string InformationText
    {
      get
      {
        var result = string.Empty;
        if (Source is BusinessBase obj && !string.IsNullOrEmpty(BindingPath))
          result = obj.BrokenRulesCollection.ToString(",", RuleSeverity.Information, BindingPath);
        return result;
      }
    }

    #endregion

    #region State properties

    /// <summary>
    /// Gets and sets the value of the property 
    /// on the business object.
    /// </summary>
    [Category("Property Status")]
    public object? Value
    {
      get
      {
        object? result = null;
        if (Source != null && !string.IsNullOrWhiteSpace(BindingPath))
          result = MethodCaller.CallPropertyGetter(Source, BindingPath);
        return result;
      }
      set
      {
        if (Source != null && !string.IsNullOrWhiteSpace(BindingPath))
          MethodCaller.CallPropertySetter(Source, BindingPath, value);
      }
    }


    private bool _canRead = true;
    /// <summary>
    /// Gets a value indicating whether the user
    /// is authorized to read the property.
    /// </summary>
    [Category("Property Status")]
    public bool CanRead
    {
      get => _canRead;
      protected set
      {
        if (value != _canRead)
        {
          _canRead = value;
          OnPropertyChanged(nameof(CanRead));
        }
      }
    }

    private bool _canWrite = true;
    /// <summary>
    /// Gets a value indicating whether the user
    /// is authorized to write the property.
    /// </summary>
    [Category("Property Status")]
    public bool CanWrite
    {
      get => _canWrite;
      protected set
      {
        if (value != _canWrite)
        {
          _canWrite = value;
          OnPropertyChanged(nameof(CanWrite));
        }
      }
    }

    private bool _isBusy = false;
    /// <summary>
    /// Gets a value indicating whether the property
    /// is busy with an asynchronous operation.
    /// </summary>
    [Category("Property Status")]
    public bool IsBusy
    {
      get => _isBusy;
      private set
      {
        if (value != _isBusy)
        {
          _isBusy = value;
          OnPropertyChanged(nameof(IsBusy));
        }
      }
    }

    private bool _isValid = true;
    /// <summary>
    /// Gets a value indicating whether the 
    /// property is valid.
    /// </summary>
    [Category("Property Status")]
    public bool IsValid
    {
      get => _isValid;
      private set
      {
        if (value != _isValid)
        {
          _isValid = value;
          OnPropertyChanged(nameof(IsValid));
        }
      }
    }

    private RuleSeverity _worst;
    /// <summary>
    /// Gets a value indicating the worst
    /// severity of all broken rules
    /// for this property (if IsValid is
    /// false).
    /// </summary>
    [Category("Property Status")]
    public RuleSeverity RuleSeverity
    {
      get => _worst;
      private set
      {
        if (value != _worst)
        {
          _worst = value;
          OnPropertyChanged(nameof(RuleSeverity));
        }
      }
    }

    private string _ruleDescription = string.Empty;
    /// <summary>
    /// Gets the description of the most severe
    /// broken rule for this property.
    /// </summary>
    [Category("Property Status")]
    public string RuleDescription
    {
      get => _ruleDescription;
      private set
      {
        if (value != _ruleDescription)
        {
          _ruleDescription = value ?? "";
          OnPropertyChanged(nameof(RuleDescription));
        }
      }
    }

    private object? _customTag;
    /// <summary>
    /// Gets or sets an arbitrary value associated with this
    /// PropertyInfo instance.
    /// </summary>
    [Category("Property Status")]
    public object? CustomTag
    {
      get => _customTag;
      set
      {
        if (!ReferenceEquals(_customTag, value))
        {
          _customTag = value;
          OnPropertyChanged(nameof(CustomTag));
        }
      }
    }

    #endregion

    #region State management

    /// <summary>
    /// Updates the metastate values on control
    /// based on the current state of the business
    /// object and property.
    /// </summary>
    public virtual void UpdateState()
    {
      if (_loading)
        return;

      if (Source == null || string.IsNullOrEmpty(BindingPath))
      {
        CanWrite = false;
        CanRead = false;
        IsValid = false;
        IsBusy = false;
        BrokenRules.Clear();
        RuleDescription = string.Empty;
      }
      else
      {
        if (Source is Security.IAuthorizeReadWrite iarw)
        {
          CanWrite = iarw.CanWriteProperty(BindingPath);
          CanRead = iarw.CanReadProperty(BindingPath);
        }

        if (Source is BusinessBase businessObject)
        {
          var allRules = (from r in businessObject.BrokenRulesCollection
                          where r.Property == BindingPath
                          select r).ToArray();

          var removeRules = (from r in BrokenRules
                             where !allRules.Contains(r)
                             select r).ToArray();

          var addRules = (from r in allRules
                          where !BrokenRules.Contains(r)
                          select r).ToArray();

          foreach (var rule in removeRules)
            BrokenRules.Remove(rule);
          foreach (var rule in addRules)
            BrokenRules.Add(rule);

          IsValid = BrokenRules.Count == 0;

          if (!IsValid)
          {
            BrokenRule? worst = (from r in BrokenRules
                                 orderby r.Severity
                                 select r).FirstOrDefault();

            if (worst != null)
            {
              RuleSeverity = worst.Severity;
              RuleDescription = worst.Description;
            }
            else
            {
              RuleDescription = string.Empty;
            }
          }
          else
          {
            RuleDescription = string.Empty;
          }
        }
        else
        {
          BrokenRules.Clear();
          RuleDescription = string.Empty;
        }
      }
      OnPropertyChanged(nameof(ErrorText));
      OnPropertyChanged(nameof(WarningText));
      OnPropertyChanged(nameof(InformationText));
    }

    #endregion

    #region INotifyPropertyChanged Members

#if !XAMARIN && !MAUI
    /// <summary>
    /// Event raised when a property has changed.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">Name of the changed property.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
#endif

    #endregion
  }
}
