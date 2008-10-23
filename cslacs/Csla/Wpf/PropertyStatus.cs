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

    public PropertyStatus()
    {
      DefaultStyleKey = typeof(PropertyStatus);
      BrokenRules = new ObservableCollection<BrokenRule>();

      Loaded += (o, e) => { GoToState(true); };
    }

    #endregion

    #region Dependency properties

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
      "Source",
      typeof(object),
      typeof(PropertyStatus),
      new FrameworkPropertyMetadata(
        null,
        FrameworkPropertyMetadataOptions.AffectsRender,
        (o, e) => ((PropertyStatus)o).SetSource(e.OldValue, e.NewValue)));

    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
      "Property",
      typeof(string),
      typeof(PropertyStatus));

    public static readonly DependencyProperty BrokenRulesProperty = DependencyProperty.Register(
      "BrokenRules",
      typeof(ObservableCollection<BrokenRule>),
      typeof(PropertyStatus));

    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
      "Target",
      typeof(DependencyObject),
      typeof(PropertyStatus));

    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
      "IsBusy",
      typeof(bool),
      typeof(PropertyStatus));

    public static readonly DependencyProperty PopupTemplateProperty = DependencyProperty.Register(
      "PopupTemplate",
      typeof(ControlTemplate),
      typeof(PropertyStatus));

    #endregion

    #region Member fields and properties


    private bool _isValid = true;
    private RuleSeverity _worst;
    private FrameworkElement _lastImage;

    public object Source
    {
      get { return GetValue(SourceProperty); }
      set
      {
        object oldValue = Source;
        object newValue = value;
        SetValue(SourceProperty, value);
        SetSource(oldValue, newValue);
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

    public DependencyObject Target
    {
      get { return (DependencyObject)GetValue(TargetProperty); }
      set { SetValue(TargetProperty, value); }
    }

    public bool IsBusy
    {
      get { return (bool)GetValue(IsBusyProperty); }
      set { SetValue(IsBusyProperty, value); }
    }

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

      UpdateState();
    }

    private void AttachSource(object source)
    {
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
      if (IsLoaded)
      {
        DisablePopup(_lastImage);
        HandleTarget();

        FrameworkElement root = (FrameworkElement)Template.FindName("root", this);

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


    #endregion

    #region RelativeTarget

    private void HandleTarget()
    {
      if (Target != null && !string.IsNullOrEmpty(Property))
      {
        var b = Source as Csla.Security.IAuthorizeReadWrite;
        if (b != null)
        {
          bool canRead = b.CanReadProperty(Property);
          bool canWrite = b.CanWriteProperty(Property);

          if (canWrite)
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
