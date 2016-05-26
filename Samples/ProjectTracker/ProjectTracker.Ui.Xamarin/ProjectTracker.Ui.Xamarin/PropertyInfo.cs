////-----------------------------------------------------------------------
//// <copyright file="PropertyInfo.cs" company="Marimer LLC">
////     Copyright (c) Marimer LLC. All rights reserved.
////     Website: http://www.lhotka.net/cslanet/
//// </copyright>
//// <summary>Expose metastate information about a property.</summary>
////-----------------------------------------------------------------------
//using System;
//using System.Linq;
//using System.ComponentModel;
//using Csla.Reflection;
//using Csla.Core;
//using Csla.Rules;
//using System.Collections.ObjectModel;
//using System.Reflection;

//namespace Csla.Xaml
//{
//  /// <summary>
//  /// Expose metastate information about a property.
//  /// </summary>
//  public class PropertyInfo : INotifyPropertyChanged
//  {
//    private bool _loading = true;
//    private const string _dependencyPropertySuffix = "Property";

//    /// <summary>
//    /// Creates an instance of the object.
//    /// </summary>
//    public PropertyInfo(object source, PropertyInfo property)
//    {
//      Source = source;
//      Property = property;
//      BrokenRules = new ObservableCollection<BrokenRule>();
//      UpdateState();
//    }

//    public ObservableCollection<BrokenRule> BrokenRules { get; private set; }
//    public Object Source { get; private set; }
//    public PropertyInfo Property { get; private set; }


//    private void HandleSourceEvents(object old, object source)
//    {
//      if (!ReferenceEquals(old, source))
//      {
//        DetachSource(old);
//        AttachSource(source);
//        BusinessBase bb = Source as BusinessBase;
//        if (bb != null)
//        {
//          IsBusy = bb.IsPropertyBusy(Property.p);
//        }
//      }
//    }

//    private void DetachSource(object source)
//    {
//      var p = source as INotifyPropertyChanged;
//      if (p != null)
//        p.PropertyChanged -= source_PropertyChanged;
//      INotifyBusy busy = source as INotifyBusy;
//      if (busy != null)
//        busy.BusyChanged -= source_BusyChanged;
//    }

//    private void AttachSource(object source)
//    {
//      var p = source as INotifyPropertyChanged;
//      if (p != null)
//        p.PropertyChanged += source_PropertyChanged;
//      INotifyBusy busy = source as INotifyBusy;
//      if (busy != null)
//        busy.BusyChanged += source_BusyChanged;
//    }

//    void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
//    {
//      if (e.PropertyName == BindingPath || string.IsNullOrEmpty(e.PropertyName))
//        UpdateState();
//    }

//    void source_BusyChanged(object sender, BusyChangedEventArgs e)
//    {
//      if (e.PropertyName == BindingPath || string.IsNullOrEmpty(e.PropertyName))
//      {
//        bool busy = e.Busy;
//        BusinessBase bb = Source as BusinessBase;
//        if (bb != null)
//          busy = bb.IsPropertyBusy(BindingPath);

//        if (busy != IsBusy)
//        {
//          IsBusy = busy;
//          UpdateState();
//        }
//      }
//    }

//    #endregion

//    #region State properties

//    private bool _canRead = true;
//    /// <summary>
//    /// Gets a value indicating whether the user
//    /// is authorized to read the property.
//    /// </summary>
//    [Category("Property Status")]
//    public bool CanRead
//    {
//      get { return _canRead; }
//      protected set
//      {
//        if (value != _canRead)
//        {
//          _canRead = value;
//          OnPropertyChanged("CanRead");
//        }
//      }
//    }

//    private bool _canWrite = true;
//    /// <summary>
//    /// Gets a value indicating whether the user
//    /// is authorized to write the property.
//    /// </summary>
//    [Category("Property Status")]
//    public bool CanWrite
//    {
//      get { return _canWrite; }
//      protected set
//      {
//        if (value != _canWrite)
//        {
//          _canWrite = value;
//          OnPropertyChanged("CanWrite");
//        }
//      }
//    }

//    private bool _isBusy = false;
//    /// <summary>
//    /// Gets a value indicating whether the property
//    /// is busy with an asynchronous operation.
//    /// </summary>
//    [Category("Property Status")]
//    public bool IsBusy
//    {
//      get { return _isBusy; }
//      private set
//      {
//        if (value != _isBusy)
//        {
//          _isBusy = value;
//          OnPropertyChanged("IsBusy");
//        }
//      }
//    }

//    private bool _isValid = true;
//    /// <summary>
//    /// Gets a value indicating whether the 
//    /// property is valid.
//    /// </summary>
//    [Category("Property Status")]
//    public bool IsValid
//    {
//      get { return _isValid; }
//      private set
//      {
//        if (value != _isValid)
//        {
//          _isValid = value;
//          OnPropertyChanged("IsValid");
//        }
//      }
//    }

//    private RuleSeverity _worst;
//    /// <summary>
//    /// Gets a valud indicating the worst
//    /// severity of all broken rules
//    /// for this property (if IsValid is
//    /// false).
//    /// </summary>
//    [Category("Property Status")]
//    public RuleSeverity RuleSeverity
//    {
//      get { return _worst; }
//      private set
//      {
//        if (value != _worst)
//        {
//          _worst = value;
//          OnPropertyChanged("RuleSeverity");
//        }
//      }
//    }

//    private string _ruleDescription = string.Empty;
//    /// <summary>
//    /// Gets the description of the most severe
//    /// broken rule for this property.
//    /// </summary>
//    [Category("Property Status")]
//    public string RuleDescription
//    {
//      get { return _ruleDescription; }
//      private set
//      {
//        if (value != _ruleDescription)
//        {
//          _ruleDescription = value;
//          OnPropertyChanged("RuleDescription");
//        }
//      }
//    }

//    #endregion

//    #region State management

//    /// <summary>
//    /// Updates the state on control Property.
//    /// </summary>
//    protected virtual void UpdateState()
//    {
//      if (_loading) return;
//      if (Source == null || string.IsNullOrEmpty(BindingPath)) return;

//      var iarw = Source as Csla.Security.IAuthorizeReadWrite;
//      if (iarw != null)
//      {
//        CanWrite = iarw.CanWriteProperty(_bindingPath);
//        CanRead = iarw.CanReadProperty(_bindingPath);
//      }

//      BusinessBase businessObject = Source as BusinessBase;
//      if (businessObject != null)
//      {
//        var allRules = (from r in businessObject.BrokenRulesCollection
//                        where r.Property == BindingPath
//                        select r).ToArray();

//        var removeRules = (from r in BrokenRules
//                           where !allRules.Contains(r)
//                           select r).ToArray();

//        var addRules = (from r in allRules
//                        where !BrokenRules.Contains(r)
//                        select r).ToArray();

//        foreach (var rule in removeRules)
//          BrokenRules.Remove(rule);
//        foreach (var rule in addRules)
//          BrokenRules.Add(rule);

//        IsValid = BrokenRules.Count == 0;

//        if (!IsValid)
//        {
//          BrokenRule worst = (from r in BrokenRules
//                              orderby r.Severity
//                              select r).FirstOrDefault();

//          if (worst != null)
//          {
//            RuleSeverity = worst.Severity;
//            RuleDescription = worst.Description;
//          }
//          else
//            RuleDescription = string.Empty;
//        }
//        else
//          RuleDescription = string.Empty;
//      }
//      else
//      {
//        BrokenRules.Clear();
//        RuleDescription = string.Empty;
//        IsValid = true;
//      }
//    }

//    #endregion

//    #region INotifyPropertyChanged Members

//    /// <summary>
//    /// Event raised when a property has changed.
//    /// </summary>
//    public event PropertyChangedEventHandler PropertyChanged;

//    /// <summary>
//    /// Raises the PropertyChanged event.
//    /// </summary>
//    /// <param name="propertyName">Name of the changed property.</param>
//    protected virtual void OnPropertyChanged(string propertyName)
//    {
//      if (PropertyChanged != null)
//        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
//    }

//    #endregion
//  }
//}
