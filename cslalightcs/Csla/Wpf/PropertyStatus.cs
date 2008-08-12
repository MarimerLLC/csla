using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Csla.Core;
using Csla.Reflection;
using Csla.Validation;
using System.Windows.Media.Animation;

namespace Csla.Wpf
{
  [TemplatePart(Name = "image", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "popup", Type = typeof(Popup))]
  [TemplateVisualState(Name = "Valid", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Error", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Warning", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Information", GroupName = "CommonStates")]
  public class PropertyStatus : ContentControl
  {
    #region Constructors

    public PropertyStatus()
    {
      DefaultStyleKey = typeof(PropertyStatus);
      RelativeTargetPath = "Parent";
      BrokenRules = new ObservableCollection<BrokenRule>();
      GoToState(true);

      Loaded += (o, e) => { GoToState(true); };
    }

    #endregion

    #region Dependency properties

    public static DependencyProperty SourceProperty = DependencyProperty.Register(
      "Source",
      typeof(object),
      typeof(PropertyStatus),
      new PropertyMetadata((o, e) => ((PropertyStatus)o).Source = e.NewValue));

    public static DependencyProperty PropertyProperty = DependencyProperty.Register(
      "Property",
      typeof(string),
      typeof(PropertyStatus),
      new PropertyMetadata((o, e) => ((PropertyStatus)o).Property = (string)e.NewValue));

    public static DependencyProperty BrokenRulesProperty = DependencyProperty.Register(
      "BrokenRules",
      typeof(ObservableCollection<BrokenRule>),
      typeof(PropertyStatus),
      null);

    public static DependencyProperty RelativeTargetPathProperty = DependencyProperty.Register(
      "RelativeTargetPath",
      typeof(string),
      typeof(PropertyStatus),
      new PropertyMetadata((o, e) => ((PropertyStatus)o).RelativeTargetPath = (string)e.NewValue));

    public static DependencyProperty RelativeTargetNameProperty = DependencyProperty.Register(
      "RelativeTargetName",
      typeof(string),
      typeof(PropertyStatus),
      new PropertyMetadata((o, e) => ((PropertyStatus)o).RelativeTargetName = (string)e.NewValue));

    #endregion

    #region Member fields and properties
    private DependencyObject _target;
    private object _source;
    private bool _isValid = true;
    private RuleSeverity _worst;
    private FrameworkElement _lastImage;
    private bool _isBusy;

    public object Source
    {
      get { return _source; }
      set
      {
        DetachSource(_source as INotifyPropertyBusy);
        _source = value;
        UpdateState();
        AttachSource(_source as INotifyPropertyBusy);
      }
    }

    public string Property
    {
      get { return (string)GetValue(PropertyProperty); }
      set { SetValue(PropertyProperty, value); }
    }

    public ObservableCollection<BrokenRule> BrokenRules
    {
      get { return (ObservableCollection<BrokenRule>)GetValue(BrokenRulesProperty); }
      private set { SetValue(BrokenRulesProperty, value); }
    }

    public string RelativeTargetPath
    {
      get { return (string)GetValue(RelativeTargetPathProperty); }
      set
      {
        SetValue(RelativeTargetPathProperty, value);
        _target = null;
      }
    }

    public string RelativeTargetName
    {
      get { return (string)GetValue(RelativeTargetNameProperty); }
      set
      {
        SetValue(RelativeTargetNameProperty, value);
        _target = null;
      }
    }

    #endregion

    #region Source

    private void AttachSource(INotifyPropertyBusy source)
    {
      if (source != null)
      {
        source.PropertyIdle += new PropertyChangedEventHandler(source_PropertyIdle);
        source.PropertyBusy += new PropertyChangedEventHandler(source_PropertyBusy);
        source.PropertyChanged += new PropertyChangedEventHandler(source_PropertyChanged);
      }
    }

    private void DetachSource(INotifyPropertyBusy source)
    {
      if (source != null)
      {
        source.PropertyIdle -= new PropertyChangedEventHandler(source_PropertyIdle);
        source.PropertyBusy -= new PropertyChangedEventHandler(source_PropertyBusy);
        source.PropertyChanged -= new PropertyChangedEventHandler(source_PropertyChanged);
      }
    }

    void source_PropertyIdle(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == Property)
        _isBusy = false;

      UpdateState();
    }

    void source_PropertyBusy(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == Property)
        _isBusy = true;

      UpdateState();
    }

    private void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == Property)
        UpdateState();
    }

    private void UpdateState()
    {
      Popup popup = (Popup)FindChild(this, "popup");
      if (popup != null)
        popup.IsOpen = false;

      BusinessBase businessObject = Source as BusinessBase;
      if (businessObject != null)
      {
        var allRules = (from r in businessObject.BrokenRulesCollection
                        where r.Property == Property
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
      if(image!=null)
        image.MouseEnter += new MouseEventHandler(image_MouseEnter);
    }

    private void DisablePopup(FrameworkElement image)
    {
      if(image!=null)
        image.MouseEnter -= new MouseEventHandler(image_MouseEnter);
    }

    private void image_MouseEnter(object sender, MouseEventArgs e)
    {
      Popup popup = (Popup)FindChild(this, "popup");
      if (popup != null)
      {
        Point p = e.GetPosition(Application.Current.RootVisual);
        
        // ensure events are attached only once.
        popup.Child.MouseLeave -= new MouseEventHandler(popup_MouseLeave);
        popup.Child.MouseLeave += new MouseEventHandler(popup_MouseLeave);
        ((ItemsControl)popup.Child).ItemsSource = BrokenRules;

        popup.VerticalOffset = p.Y - 5;
        popup.HorizontalOffset = p.X - 5;
        popup.IsOpen = true;
      }
    }

    void popup_MouseLeave(object sender, MouseEventArgs e)
    {
      Popup popup = (Popup)FindChild(this, "popup");
      popup.IsOpen = false;
    }

    #endregion

    #region State management

    private void GoToState(bool useTransitions)
    {
      DisablePopup(_lastImage);
      EnsureTarget();
      HandleTarget();

      BusyAnimation busy = FindChild(this, "busy") as BusyAnimation;
      if (busy != null)
        busy.IsRunning = _isBusy;

      if (_isBusy)
      {
        VisualStateManager.GoToState(this, "Busy", useTransitions);
      }
      else if (_isValid)
      {
        VisualStateManager.GoToState(this, "Valid", useTransitions);
      }
      else
      {
        VisualStateManager.GoToState(this, _worst.ToString(), useTransitions);
        _lastImage = (FrameworkElement)FindChild(this, string.Format("{0}Image", _worst.ToString().ToLower()));
        EnablePopup(_lastImage);
      }
    }

    
    #endregion

    #region RelativeTarget

    private void EnsureTarget()
    {
      if (_target == null)
      {
        _target = VisualTree.FindParent(RelativeTargetPath, this);
        if (!string.IsNullOrEmpty(RelativeTargetName))
          _target = VisualTree.FindByName(RelativeTargetName, _target);
      }
    }

    private void HandleTarget()
    {
      if (_target != null && !string.IsNullOrEmpty(Property))
      {
        BusinessBase b = Source as BusinessBase;
        if (b != null)
        {
          bool canRead = b.CanReadProperty(Property);
          bool canWrite = b.CanWriteProperty(Property);

          if (canWrite)
          {
            MethodCaller.CallMethodIfImplemented(_target, "set_IsReadOnly", false);
            MethodCaller.CallMethodIfImplemented(_target, "set_IsEnabled", true);
          }
          else
          {
            MethodCaller.CallMethodIfImplemented(_target, "set_IsReadOnly", true);
            MethodCaller.CallMethodIfImplemented(_target, "set_IsEnabled", false);
          }

          if (canRead)
          {
          }
          else
          {
            MethodCaller.CallMethodIfImplemented(_target, "set_Content", null);
            MethodCaller.CallMethodIfImplemented(_target, "set_Text", "");
          }
        }
      }
    }

    #endregion

    #region Helpers

    private DependencyObject FindChild(DependencyObject parent, string name)
    {
      DependencyObject found = null;
      int count = VisualTreeHelper.GetChildrenCount(parent);
      for (int x = 0; x < count; x++)
      {
        DependencyObject child = VisualTreeHelper.GetChild(parent, x);
        string childName = child.GetValue(FrameworkElement.NameProperty) as string;
        if (childName == name)
        {
          found = child;
          break;
        }
        else found = FindChild(child, name);
      }

      return found;
    }

    #endregion
  }
}
