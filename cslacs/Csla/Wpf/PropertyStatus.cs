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

namespace Csla.Wpf
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
  public class PropertyStatus : ContentControl
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
          SetSource(e.OldValue, e.NewValue);
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
    /// Defines the business object property to watch for
    /// validation, authorization and busy status.
    /// </summary>
    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
      "Property",
      typeof(string),
      typeof(PropertyStatus));

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
    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
      "Target",
      typeof(DependencyObject),
      typeof(PropertyStatus));

    /// <summary>
    /// Gets or sets a value indicating whether the PropertyStatus
    /// control should be in busy mode.
    /// </summary>
    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
      "IsBusy",
      typeof(bool),
      typeof(PropertyStatus));

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
    private bool _isValid = true;
    private RuleSeverity _worst;
    private FrameworkElement _lastImage;

    ///// <summary>
    ///// Gets or sets a reference to the data source object.
    ///// </summary>
    //public object Source
    //{
    //  get { return GetValue(SourceProperty); }
    //  set
    //  {
    //    object oldValue = Source;
    //    object newValue = value;
    //    SetValue(SourceProperty, value);
    //    SetSource(oldValue, newValue);
    //  }
    //}

    /// <summary>
    /// Gets or sets the name of the business object
    /// property to be monitored.
    /// </summary>
    public string Property
    {
      get { return (string)GetValue(PropertyProperty); }
      set 
      { 
        SetValue(PropertyProperty, value);
        CheckProperty();
      }
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
    public DependencyObject Target
    {
      get { return (DependencyObject)GetValue(TargetProperty); }
      set { SetValue(TargetProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the PropertyStatus
    /// control should be in busy mode.
    /// </summary>
    public bool IsBusy
    {
      get { return (bool)GetValue(IsBusyProperty); }
      set { SetValue(IsBusyProperty, value); }
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

    private void SetSource(object oldSource, object newSource)
    {
      DetachSource(oldSource);
      AttachSource(newSource);

      BusinessBase bb = newSource as BusinessBase;
      if (bb != null && !string.IsNullOrEmpty(Property))
        IsBusy = bb.IsPropertyBusy(Property);

      CheckProperty();
      UpdateState();
    }

    private object Source { get; set; }

    private void AttachSource(object source)
    {
      this.Source = source;
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
      if (e.PropertyName == Property)
        IsBusy = e.Busy;

      UpdateState();
    }

    private void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == Property || string.IsNullOrEmpty(e.PropertyName))
        UpdateState();
    }

    private void UpdateState()
    {
      Popup popup = (Popup)FindName("popup");
      if (popup != null)
        popup.IsOpen = false;

      BusinessBase businessObject = Source as BusinessBase;
      if (businessObject != null)
      {
        // for some reason Linq does not work against BrokenRulesCollection...
        List<BrokenRule> allRules = new List<BrokenRule>();
        foreach (var r in businessObject.BrokenRulesCollection)
          if (r.Property == Property)
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
      Popup popup = (Popup)Template.FindName("popup", this);
      popup.IsOpen = true;
    }

    void image_MouseLeave(object sender, MouseEventArgs e)
    {
      Popup popup = (Popup)Template.FindName("popup", this);
      popup.IsOpen = false;
    }

    #endregion

    #region State management

    private void GoToState(bool useTransitions)
    {
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

    private void CheckProperty()
    {
      if (Source != null)
      {
        var desc = Csla.Reflection.MethodCaller.GetPropertyDescriptor(Source.GetType(), Property);
        if (desc != null)
          _isReadOnly = desc.IsReadOnly;
        else
          _isReadOnly = false;
      }
      else
      {
        _isReadOnly = true;
      }
    }

    private void HandleTarget()
    {
      if (Target != null && !string.IsNullOrEmpty(Property))
      {
        var b = Source as Csla.Security.IAuthorizeReadWrite;
        if (b != null)
        {
          bool canRead = b.CanReadProperty(Property);
          bool canWrite = b.CanWriteProperty(Property);

          if (canWrite && !_isReadOnly)
          {
            MethodCaller.CallMethodIfImplemented(Target, "set_IsReadOnly", false);
            MethodCaller.CallMethodIfImplemented(Target, "set_IsEnabled", true);
          }
          else
          {
            MethodCaller.CallMethodIfImplemented(Target, "set_IsReadOnly", true);
            MethodCaller.CallMethodIfImplemented(Target, "set_IsEnabled", false);
          }

          if (!canRead)
          {
            MethodCaller.CallMethodIfImplemented(Target, "set_Content", null);
            MethodCaller.CallMethodIfImplemented(Target, "set_Text", "");
          }
        }
      }
    }

    #endregion
  }
}
