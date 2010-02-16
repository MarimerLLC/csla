using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Reflection;
using System.Windows;
using Csla.Core;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Csla.Validation;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Csla.Xaml
{ 
  /// <summary>
  /// Control providing services around business object
  /// validation, authorization and async busy status.
  /// </summary>
  [TemplatePart(Name = "root", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "popup", Type = typeof(Popup))]
  [TemplatePart(Name = "errorImage", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "warningImage", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "informationImage", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "busy", Type = typeof(BusyAnimation))]
  [TemplatePart(Name = "Valid", Type = typeof(Storyboard))]
  [TemplatePart(Name = "Error", Type = typeof(Storyboard))]
  [TemplatePart(Name = "Warning", Type = typeof(Storyboard))]
  [TemplatePart(Name = "Information", Type = typeof(Storyboard))]
  public class PropertyStatus : ContentControl,
    INotifyPropertyChanged
  {
    #region Constructors

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public PropertyStatus()
    {
      DefaultStyleKey = typeof(PropertyStatus);
      BrokenRules = new ObservableCollection<BrokenRule>();

      DataContextChanged += (o, e) =>
        {
          SetSource();
          //SetSource(e.OldValue, e.NewValue);
        };

      Loaded += (o, e) => { UpdateState(); };
    }

    /// <summary>
    /// Invoked whenever application code
    /// or internal processes call System.Windows.FrameworkElement.ApplyTemplate()
    /// Once template is applied to the control,force state update.
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateState();
    }

    #endregion

    #region Dependency properties

    /// <summary>
    /// Gets a reference to the business object's
    /// broken rules collection.
    /// </summary>
    public static readonly DependencyProperty BrokenRulesProperty = DependencyProperty.Register(
      "BrokenRules",
      typeof(ObservableCollection<BrokenRule>),
      typeof(PropertyStatus));

    /// <summary>
    /// Reference to the target UI control to be managed
    /// for authorization rules.
    /// </summary>
    public static readonly DependencyProperty TargetControlProperty = DependencyProperty.Register(
      "TargetControl",
      typeof(DependencyObject),
      typeof(PropertyStatus),
      new PropertyMetadata(null, (o, e) => { ((PropertyStatus)o).HandleTarget(); }));

    /// <summary>
    /// Reference to the template for the validation rule popup.
    /// </summary>
    public static readonly DependencyProperty PopupTemplateProperty = DependencyProperty.Register(
      "PopupTemplate",
      typeof(ControlTemplate),
      typeof(PropertyStatus));

    #endregion

    #region Member fields and properties

    private bool _isReadOnly = false;
    private FrameworkElement _lastImage;

    /// <summary>
    /// Defines the business object property to watch for
    /// validation, authorization and busy status.
    /// </summary>
    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
      "Property",
      typeof(object),
      typeof(PropertyStatus),
      new PropertyMetadata((o, e) => ((PropertyStatus)o).SetSource()));

    /// <summary>
    /// Gets or sets the name of the business object
    /// property to be monitored.
    /// </summary>
    public object Property
    {
      get { return GetValue(PropertyProperty); }
      set { SetValue(PropertyProperty, value); }
    }

    /// <summary>
    /// Gets a reference to the business object's
    /// broken rules collection.
    /// </summary>
    public ObservableCollection<BrokenRule> BrokenRules
    {
      get { return (ObservableCollection<BrokenRule>)GetValue(BrokenRulesProperty); }
      private set { SetValue(BrokenRulesProperty, value); }
    }

    /// <summary>
    /// Gets or sets a reference to the UI control to
    /// be managed based on authorization rules.
    /// </summary>
    public DependencyObject TargetControl
    {
      get { return (DependencyObject)GetValue(TargetControlProperty); }
      set { SetValue(TargetControlProperty, value); }
    }

    /// <summary>
    /// Gets or sets the template for the validation rules
    /// popup.
    /// </summary>
    public ControlTemplate PopupTemplate
    {
      get { return (ControlTemplate)GetValue(PopupTemplateProperty); }
      set { SetValue(PopupTemplateProperty, value); }
    }

    #endregion

    #region Source

    private object _source = null;
    private string _bindingPath = string.Empty;

    private void SetSource()
    {
      var old = _source;
      var binding = GetBindingExpression(PropertyProperty);
      if (binding != null)
      {
        if (binding.ParentBinding != null && binding.ParentBinding.Path != null)
          _bindingPath = binding.ParentBinding.Path.Path;
        else
          _bindingPath = string.Empty;
        _source = GetRealSource(binding.DataItem, _bindingPath);
        if (_bindingPath.IndexOf('.') > 0)
          _bindingPath = _bindingPath.Substring(_bindingPath.LastIndexOf('.') + 1);
      }
      else
      {
        _source = null;
        _bindingPath = string.Empty;
      }

      if (!ReferenceEquals(old, _source))
      {
        DetachSource(old);
        AttachSource(_source);
      }

      FindIsReadOnly();
      UpdateState();
    }

    private object GetRealSource(object source, string bindingPath)
    {
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

    private void AttachSource(object source)
    {
      this._source = source;
      INotifyBusy busy = source as INotifyBusy;
      if (busy != null)
        busy.BusyChanged += new BusyChangedEventHandler(source_BusyChanged);

      INotifyPropertyChanged changed = source as INotifyPropertyChanged;
      if (changed != null)
        changed.PropertyChanged += new PropertyChangedEventHandler(source_PropertyChanged);
    }

    private void DetachSource(object source)
    {
      INotifyBusy busy = source as INotifyBusy;
      if (busy != null)
        busy.BusyChanged -= new BusyChangedEventHandler(source_BusyChanged);

      INotifyPropertyChanged changed = source as INotifyPropertyChanged;
      if (changed != null)
        changed.PropertyChanged -= new PropertyChangedEventHandler(source_PropertyChanged);
    }

    void source_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      if (e.PropertyName == _bindingPath)
        IsBusy = e.Busy;

      UpdateState();
    }

    private void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == _bindingPath || string.IsNullOrEmpty(e.PropertyName))
        UpdateState();
    }

    private void UpdateState()
    {
      Popup popup = (Popup)FindName("popup");
      if (popup != null)
        popup.IsOpen = false;

      BusinessBase businessObject = _source as BusinessBase;
      if (businessObject != null)
      {
        // for some reason Linq does not work against BrokenRulesCollection...
        List<BrokenRule> allRules = new List<BrokenRule>();
        foreach (var r in businessObject.BrokenRulesCollection)
          if (r.Property == _bindingPath)
            allRules.Add(r);

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

        BrokenRule worst = (from r in BrokenRules
                            orderby r.Severity
                            select r).FirstOrDefault();

        if (worst != null)
          _worst = worst.Severity;

        _isValid = BrokenRules.Count == 0;
        GoToState(true);
      }
      else
      {
        BrokenRules.Clear();
        _isValid = true;
        GoToState(true);
      }
    }

    #endregion

    #region State properties

    private bool _canRead = true;
    /// <summary>
    /// Gets a value indicating whether the user
    /// is authorized to read the property.
    /// </summary>
    public bool CanRead
    {
      get { return _canRead; }
      private set
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
    public bool CanWrite
    {
      get { return _canWrite; }
      private set
      {
        if (value != _canWrite)
        {
          _canWrite = value;
          OnPropertyChanged("CanWrite");
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the property
    /// is busy with an asynchronous operation.
    /// </summary>
    public static readonly DependencyProperty IsBusyProperty =
        DependencyProperty.Register("IsBusy", typeof(bool), typeof(PropertyStatus), null);

    /// <summary>
    /// Gets a value indicating whether the property
    /// is busy with an asynchronous operation.
    /// </summary>
    public bool IsBusy
    {
      get { return (bool)GetValue(IsBusyProperty); }
      set { SetValue(IsBusyProperty, value); }
    }

    private bool _isValid = true;
    /// <summary>
    /// Gets a value indicating whether the 
    /// property is valid.
    /// </summary>
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

    #region Image

    private void EnablePopup(FrameworkElement image)
    {
      if (image != null)
      {
        image.MouseEnter += new MouseEventHandler(image_MouseEnter);
        image.MouseLeave += new MouseEventHandler(image_MouseLeave);
      }
    }

    private void DisablePopup(FrameworkElement image)
    {
      if (image != null)
      {
        image.MouseEnter -= new MouseEventHandler(image_MouseEnter);
        image.MouseLeave -= new MouseEventHandler(image_MouseLeave);
      }
    }

    private void image_MouseEnter(object sender, MouseEventArgs e)
    {
      if (Template == null)
        return;
      Popup popup = (Popup)Template.FindName("popup", this);
      popup.IsOpen = true;
    }

    void image_MouseLeave(object sender, MouseEventArgs e)
    {
      if (Template == null)
        return;
      Popup popup = (Popup)Template.FindName("popup", this);
      popup.IsOpen = false;
    }

    #endregion

    #region State management

    private void GoToState(bool useTransitions)
    {
      if (Template == null)
        return;
      if (IsLoaded && !DataPortal.IsInDesignMode)
      {
        DisablePopup(_lastImage);
        HandleTarget();

        FrameworkElement root = (FrameworkElement)Template.FindName("root", this);

        if (root != null)
        {
          if (_isValid)
          {
            Storyboard validStoryboard = (Storyboard)Template.Resources["Valid"];
            validStoryboard.Begin(root);
          }
          else
          {
            Storyboard errorStoryboard = (Storyboard)Template.Resources[_worst.ToString()];
            errorStoryboard.Begin(root);
            _lastImage = (FrameworkElement)Template.FindName(string.Format("{0}Image", _worst.ToString().ToLower()), this);
            EnablePopup(_lastImage);
          }
        }
      }
    }


    #endregion

    #region RelativeTarget

    private void FindIsReadOnly()
    {
      if (_source != null && !string.IsNullOrEmpty(_bindingPath))
      {
        var info = _source.GetType().GetProperty(_bindingPath);
        if (info != null)
        {
          _isReadOnly = !info.CanWrite;
          if (!_isReadOnly)
          {
            var setter = _source.GetType().GetMethod("set_" + _bindingPath);
            if (setter == null)
              _isReadOnly = true;
            else
              _isReadOnly = !setter.IsPublic;
          }
        }
      }
      else
      {
        _isReadOnly = false;
      }
    }

    private void HandleTarget()
    {
      if (!string.IsNullOrEmpty(_bindingPath))
      {
        var b = _source as Csla.Security.IAuthorizeReadWrite;
        if (b != null)
        {
          CanWrite = b.CanWriteProperty(_bindingPath);
          if (TargetControl != null)
          {
            if (CanWrite && !_isReadOnly)
            {
              MethodCaller.CallMethodIfImplemented(TargetControl, "set_IsReadOnly", false);
              MethodCaller.CallMethodIfImplemented(TargetControl, "set_IsEnabled", true);
            }
            else
            {
              MethodCaller.CallMethodIfImplemented(TargetControl, "set_IsReadOnly", true);
              MethodCaller.CallMethodIfImplemented(TargetControl, "set_IsEnabled", false);
            }
          }

          CanRead = b.CanReadProperty(_bindingPath);
          if (TargetControl != null && !CanRead)
          {
            MethodCaller.CallMethodIfImplemented(TargetControl, "set_Content", null);
            MethodCaller.CallMethodIfImplemented(TargetControl, "set_Text", "");
          }
        }
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
