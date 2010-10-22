//-----------------------------------------------------------------------
// <copyright file="PropertyInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Expose metastate information about a property.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using Csla.Reflection;
using Csla.Core;
using Csla.Rules;
using System.Collections.ObjectModel;

namespace Csla.Xaml
{
  /// <summary>
  /// Expose metastate information about a property.
  /// </summary>
  public class PropertyInfo : FrameworkElement, INotifyPropertyChanged
  {
    private bool _loading = true;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public PropertyInfo()
    {
      Visibility = System.Windows.Visibility.Collapsed;
      Height = 20;
      Width = 20;
      BrokenRules = new ObservableCollection<BrokenRule>();
      Loaded += (o, e) =>
      {
        _loading = false;
        UpdateState();
      };
    }

    /// <summary>
    /// Creates an instance of the object for testing.
    /// </summary>
    /// <param name="testing">Test mode parameter.</param>
    public PropertyInfo(bool testing)
      : this()
    {
      _loading = false;
      UpdateState();
    }

    #region BrokenRules property

    /// <summary>
    /// Gets the broken rules collection from the
    /// business object.
    /// </summary>
    public static readonly DependencyProperty BrokenRulesProperty = DependencyProperty.Register(
      "BrokenRules",
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
      private set { SetValue(BrokenRulesProperty, value); }
    }

    #endregion

    #region Source property

    /// <summary>
    /// Gets or sets the source business
    /// property to which this control is bound.
    /// </summary>
    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
      "Property",
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
    public object Property
    {
      get { return GetValue(PropertyProperty); }
      set
      {
        SetValue(PropertyProperty, value);
        SetSource(false);
      }
    }

    private object _source = null;
    /// <summary>
    /// Gets or sets the Source.
    /// </summary>
    /// <value>The source.</value>
    protected object Source
    {
      get
      {
        return _source;
      }
      set
      {
        _source = value;
      }
    }

    private string _bindingPath = string.Empty;
    /// <summary>
    /// Gets or sets the binding path.
    /// </summary>
    /// <value>The binding path.</value>
    protected string BindingPath
    {
      get
      {
        return _bindingPath;
      }
      set
      {
        _bindingPath = value;
      }
    }

    private object _oldDataContext;
    private System.Windows.Data.BindingExpression _oldBinding;

    /// <summary>
    /// Sets the source binding and updates status.
    /// </summary>
    protected virtual void SetSource(bool propertyValueChanged)
    {
      var old = Source;
      var binding = GetBindingExpression(PropertyProperty);
      if (!ReferenceEquals(_oldBinding, binding) || !ReferenceEquals(_oldDataContext, this.DataContext) || !propertyValueChanged)
      {
        _oldDataContext = this.DataContext;
        _oldBinding = binding;
        if (binding != null)
        {
          if (binding.ParentBinding != null && binding.ParentBinding.Path != null)
            BindingPath = binding.ParentBinding.Path.Path;
          else
            BindingPath = string.Empty;
          Source = GetRealSource(binding.DataItem, BindingPath);
          if (BindingPath.IndexOf('.') > 0)
            BindingPath = BindingPath.Substring(BindingPath.LastIndexOf('.') + 1);
        }
        else
        {
          Source = null;
          BindingPath = string.Empty;
        }

        HandleSourceEvents(old, Source);
        UpdateState();
      }
    }

    /// <summary>
    /// Gets the real source helper method.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="bindingPath">The binding path.</param>
    /// <returns></returns>
    protected object GetRealSource(object source, string bindingPath)
    {
      var icv = source as ICollectionView;
      if (icv != null)
        source = icv.CurrentItem;
      if (source != null && bindingPath.IndexOf('.') > 0)
      {
        var firstProperty = bindingPath.Substring(0, bindingPath.IndexOf('.'));
        var p = MethodCaller.GetProperty(source.GetType(), firstProperty);
        return GetRealSource(
          MethodCaller.GetPropertyValue(source, p),
          bindingPath.Substring(bindingPath.IndexOf('.') + 1));
      }
      else
        return source;
    }

    private void HandleSourceEvents(object old, object source)
    {
      if (!ReferenceEquals(old, source))
      {
        DetachSource(old);
        AttachSource(source);
        BusinessBase bb = Source as BusinessBase;
        if (bb != null)
        {
          IsBusy = bb.IsPropertyBusy(BindingPath);
        }
      }
    }

    private void DetachSource(object source)
    {
      var p = source as INotifyPropertyChanged;
      if (p != null)
        p.PropertyChanged -= source_PropertyChanged;
      INotifyBusy busy = source as INotifyBusy;
      if (busy != null)
        busy.BusyChanged -= source_BusyChanged;
    }

    private void AttachSource(object source)
    {
      var p = source as INotifyPropertyChanged;
      if (p != null)
        p.PropertyChanged += source_PropertyChanged;
      INotifyBusy busy = source as INotifyBusy;
      if (busy != null)
        busy.BusyChanged += source_BusyChanged;
    }

    void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == BindingPath || string.IsNullOrEmpty(e.PropertyName))
        UpdateState();
    }

    void source_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      if (e.PropertyName == BindingPath || string.IsNullOrEmpty(e.PropertyName))
      {
        bool busy = e.Busy;
        BusinessBase bb = Source as BusinessBase;
        if (bb != null)
          busy = bb.IsPropertyBusy(BindingPath);

        if (busy != IsBusy)
        {
          IsBusy = busy;
          UpdateState();
        }
      }
    }

    #endregion

    #region State properties

    private bool _canRead = true;
    /// <summary>
    /// Gets a value indicating whether the user
    /// is authorized to read the property.
    /// </summary>
    [Category("Property Status")]
    public bool CanRead
    {
      get { return _canRead; }
      protected set
      {
        if (value != _canRead)
        {
          _canRead = value;
          OnPropertyChanged("CanRead");
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
      get { return _canWrite; }
      protected set
      {
        if (value != _canWrite)
        {
          _canWrite = value;
          OnPropertyChanged("CanWrite");
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
      get { return _isBusy; }
      private set
      {
        if (value != _isBusy)
        {
          _isBusy = value;
          OnPropertyChanged("IsBusy");
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
      get { return _isValid; }
      private set
      {
        if (value != _isValid)
        {
          _isValid = value;
          OnPropertyChanged("IsValid");
        }
      }
    }

    private RuleSeverity _worst;
    /// <summary>
    /// Gets a valud indicating the worst
    /// severity of all broken rules
    /// for this property (if IsValid is
    /// false).
    /// </summary>
    [Category("Property Status")]
    public RuleSeverity RuleSeverity
    {
      get { return _worst; }
      private set
      {
        if (value != _worst)
        {
          _worst = value;
          OnPropertyChanged("RuleSeverity");
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
      get { return _ruleDescription; }
      private set
      {
        if (value != _ruleDescription)
        {
          _ruleDescription = value;
          OnPropertyChanged("RuleDescription");
        }
      }
    }

    #endregion

    #region State management

    /// <summary>
    /// Updates the state on control Property.
    /// </summary>
    protected virtual void UpdateState()
    {
      if (_loading) return;
      if (Source == null || string.IsNullOrEmpty(BindingPath)) return;

      var iarw = Source as Csla.Security.IAuthorizeReadWrite;
      if (iarw != null)
      {
        CanWrite = iarw.CanWriteProperty(_bindingPath);
        CanRead = iarw.CanReadProperty(_bindingPath);
      }

      BusinessBase businessObject = Source as BusinessBase;
      if (businessObject != null)
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
          BrokenRule worst = (from r in BrokenRules
                              orderby r.Severity
                              select r).FirstOrDefault();

          if (worst != null)
          {
            RuleSeverity = worst.Severity;
            RuleDescription = worst.Description;
          }
          else
            RuleDescription = string.Empty;
        }
        else
          RuleDescription = string.Empty;
      }
      else
      {
        BrokenRules.Clear();
        RuleDescription = string.Empty;
        IsValid = true;
      }
    }

    #endregion

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Event raised when a property has changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">Name of the changed property.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}
