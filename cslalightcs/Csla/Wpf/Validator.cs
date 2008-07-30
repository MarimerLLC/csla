using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.ComponentModel;
using Csla.Core;
using System.Globalization;
using System.Windows.Controls.Primitives;
using Csla.Validation;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace Csla.Wpf
{
  [TemplatePart(Name = "image", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "popup", Type = typeof(Popup))]
  [TemplateVisualState(Name = "Valid", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Error", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Warning", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Information", GroupName = "CommonStates")]
  public class Validator : ContentControl
  {
    public Validator()
    {
      DefaultStyleKey = typeof(Validator);
      //RelativeTargetPath = "Parent";
      BrokenRules = new ObservableCollection<BrokenRule>();
      GoToState(true);
    }

    #region Dependency properties

    public static DependencyProperty SourceProperty = DependencyProperty.Register(
      "Source",
      typeof(object),
      typeof(Validator),
      new PropertyMetadata((o, e) => ((Validator)o).Source = e.NewValue));

    public static DependencyProperty PropertyProperty = DependencyProperty.Register(
      "Property",
      typeof(string),
      typeof(Validator),
      new PropertyMetadata((o, e) => ((Validator)o).Property = (string)e.NewValue));

    public static DependencyProperty BrokenRulesProperty = DependencyProperty.Register(
      "BrokenRules",
      typeof(ObservableCollection<BrokenRule>),
      typeof(Validator),
      null);

    //public static DependencyProperty RelativeTargetPathProperty = DependencyProperty.Register(
    //  "RelativeTargetPath",
    //  typeof(string),
    //  typeof(Validator),
    //  new PropertyMetadata((o, e) => ((Validator)o).RelativeTargetPath = (string)e.NewValue));

    //public static DependencyProperty RelativeTargetNameProperty = DependencyProperty.Register(
    //  "RelativeTargetName",
    //  typeof(string),
    //  typeof(Validator),
    //  new PropertyMetadata((o, e) => ((Validator)o).RelativeTargetName = (string)e.NewValue));

    #endregion

    #region Member fields and properties
    private FrameworkElement _target;
    private object _source;
    private bool _isValid;
    private RuleSeverity _worst;
    private FrameworkElement _lastImage;

    public object Source
    {
      get { return _source; }
      set
      {
        DetachSource(_source as INotifyPropertyChanged);
        _source = value;
        UpdateState();
        AttachSource(_source as INotifyPropertyChanged);
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

    //public string RelativeTargetPath
    //{
    //  get { return (string)GetValue(RelativeTargetPathProperty); }
    //  set
    //  {
    //    SetValue(RelativeTargetPathProperty, value);
    //    _target = null;
    //  }
    //}

    //public string RelativeTargetName
    //{
    //  get { return (string)GetValue(RelativeTargetNameProperty); }
    //  set
    //  {
    //    SetValue(RelativeTargetNameProperty, value);
    //    _target = null;
    //  }
    //}

    #endregion

    #region Source

    private void AttachSource(INotifyPropertyChanged source)
    {
      if (source != null)
        source.PropertyChanged += new PropertyChangedEventHandler(source_PropertyChanged);
    }

    private void DetachSource(INotifyPropertyChanged source)
    {
      if (source != null)
        source.PropertyChanged -= new PropertyChangedEventHandler(source_PropertyChanged);
    }

    private void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == Property)
        UpdateState();
    }

    private void UpdateState()
    {
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


        RuleSeverity severity = RuleSeverity.Error;
        if (worst != null)
          _worst = worst.Severity;

        _isValid = BrokenRules.Count == 0;
        GoToState(true);
      }
    }

    #endregion

    #region Image
    
    // ToolTip is not currently bindable in Silverlight so we will manually set it for now...
    private void UpdateToolTip(FrameworkElement image, string value)
    {
      if (image != null)
        image.SetValue(ToolTipService.ToolTipProperty, value);
    }

    private void EnablePopup(FrameworkElement image)
    {
      if(image!=null)
        image.MouseLeftButtonDown += new MouseButtonEventHandler(image_MouseLeftButtonDown);
    }

    private void DisablePopup(FrameworkElement image)
    {
      if(image!=null)
        image.MouseLeftButtonDown -= new MouseButtonEventHandler(image_MouseLeftButtonDown);
    }

    private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      Popup popup = (Popup)FindChild(this, "popup");
      if (popup != null)
      {
        Point p = e.GetPosition(Application.Current.RootVisual);
        
        // ensure events are attached only once.
        popup.Child.MouseLeave -= new MouseEventHandler(popup_MouseLeave);
        popup.Child.MouseLeave += new MouseEventHandler(popup_MouseLeave);
        ((ItemsControl)popup.Child).ItemsSource = BrokenRules;

        popup.VerticalOffset = p.Y;
        popup.HorizontalOffset = p.X;
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
      UpdateToolTip(_lastImage, null);
      DisablePopup(_lastImage);

      if (_isValid)
      {
        VisualStateManager.GoToState(this, "Valid", useTransitions);
      }
      else
      {
        VisualStateManager.GoToState(this, _worst.ToString(), useTransitions);
        switch (_worst)
        {
          case RuleSeverity.Error:
            _lastImage = (FrameworkElement)FindChild(this, "errorImage");
            break;
          case RuleSeverity.Warning:
            _lastImage = (FrameworkElement)FindChild(this, "warningImage");
            break;
          case RuleSeverity.Information:
            _lastImage = (FrameworkElement)FindChild(this, "informationImage");
            break;
        }
        UpdateToolTip(_lastImage, "Click to view messages...");
        EnablePopup(_lastImage);
      }
    }
    #endregion

    #region RelativeTarget

    //private void EnsureTarget()
    //{
    //  if (_target == null)
    //  {
    //    var parentPath = new Queue<string>(RelativeTargetPath.Split('.'));
    //    _target = FindParent(parentPath, this);
    //    if (!string.IsNullOrEmpty(RelativeTargetName))
    //      _target = FindByName(_target, RelativeTargetName);
    //  }
    //}

    //private FrameworkElement FindParent(Queue<string> queue, FrameworkElement current)
    //{
    //  string propertyName = queue.Dequeue();

    //  object[] indexParameters = new object[0];
    //  if (propertyName.Contains('['))
    //  {
    //    int start = propertyName.IndexOf('[');
    //    int end = propertyName.IndexOf(']');
    //    if (end != propertyName.Length - 1)
    //      throw new InvalidOperationException("Indexed expressions must be closed");

    //    int length = (end - start) - 1;
    //    indexParameters = propertyName.Substring(start + 1, length).Split(',').Cast<object>().ToArray();

    //    propertyName = propertyName.Substring(0, start);
    //  }

    //  PropertyInfo property = current.GetType().GetProperty(propertyName);
    //  ParameterInfo[] parameters = property.GetIndexParameters();
    //  if (parameters.Length != indexParameters.Length)
    //    throw new InvalidOperationException(string.Format(
    //      "This property requires {0} index arguments, {1} were provided",
    //      parameters.Length,
    //      indexParameters.Length));

    //  for (int x = 0; x < parameters.Length; x++)
    //  {
    //    indexParameters[x] = Convert.ChangeType(
    //      indexParameters[x],
    //      parameters[x].ParameterType,
    //      CultureInfo.InvariantCulture);
    //  }

    //  return (FrameworkElement)property.GetValue(current, indexParameters);

    //}

    //private FrameworkElement FindByName(FrameworkElement parent, string name)
    //{
    //  for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
    //  {
    //    FrameworkElement child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
    //    if (child != null && child.Name == name)
    //    {
    //      return child;
    //    }
    //  }

    //  return null;
    //} 

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
