using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Csla.Core;
using Csla.Reflection;
using Csla.Validation;

namespace Csla.Silverlight
{
  /// <summary>
  /// Displays validation information for a business
  /// object property, and manipulates an associated
  /// UI control based on the business object's
  /// authorization rules.
  /// </summary>
  [TemplatePart(Name = "image", Type = typeof(FrameworkElement))]
  [TemplatePart(Name = "popup", Type = typeof(Popup))]
  [TemplatePart(Name = "busy", Type = typeof(BusyAnimation))]
  [TemplateVisualState(Name = "Valid", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Error", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Warning", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Information", GroupName = "CommonStates")]
  public class PropertyStatus : ContentControl,
    INotifyPropertyChanged
  {
    private bool _isReadOnly = false;
    private FrameworkElement _lastImage;

    #region Constructors

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public PropertyStatus()
      : base()
    {
      BrokenRules = new ObservableCollection<BrokenRule>();
      DefaultStyleKey = typeof(PropertyStatus);
      IsTabStop = false;

      Loaded += (o, e) => GoToState(true);
    }

    /// <summary>
    /// Applies the visual template.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      UpdateState();
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
      typeof(PropertyStatus),
      new PropertyMetadata((o, e) => ((PropertyStatus)o).SetSource()));

    /// <summary>
    /// Gets or sets the source business
    /// property to which this control is bound.
    /// </summary>
    public object Property
    {
      get { return GetValue(PropertyProperty); }
      set
      {
        SetValue(PropertyProperty, value);
        SetSource();
      }
    }

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

      HandleIsBusy(old, _source);

      FindIsReadOnly();
      UpdateState();
    }

    private object GetRealSource(object source, string bindingPath)
    {
      if (bindingPath.IndexOf('.') > 0)
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

    private void HandleIsBusy(object old, object source)
    {
      if (!ReferenceEquals(old, source))
      {
        DetachSource(old);
        AttachSource(source);
        BusinessBase bb = _source as BusinessBase;
        if (bb != null)
        {
          IsBusy = bb.IsPropertyBusy(_bindingPath);
        }
      }
    }

    private void DetachSource(object source)
    {
      INotifyBusy busy = source as INotifyBusy;
      if (busy != null)
        busy.BusyChanged -= source_BusyChanged;
    }

    private void AttachSource(object source)
    {
      INotifyBusy busy = source as INotifyBusy;
      if (busy != null)
        busy.BusyChanged += source_BusyChanged;
    }

    void source_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      if (e.PropertyName == _bindingPath)
      {
        bool busy = e.Busy;
        BusinessBase bb = _source as BusinessBase;
        if (bb != null)
          busy = bb.IsPropertyBusy(_bindingPath);

        if (busy != IsBusy)
        {
          IsBusy = busy;
          UpdateState();
        }
      }
    }

    #endregion

    #region Target property

    /// <summary>
    /// Gets or sets the target control to which this control is bound.
    /// </summary>
    public static readonly DependencyProperty TargetControlProperty = DependencyProperty.Register(
      "TargetControl",
      typeof(object),
      typeof(PropertyStatus),
      null);

    /// <summary>
    /// Gets or sets the target control to which this control is bound.
    /// </summary>
    public object TargetControl
    {
      get { return GetValue(TargetControlProperty); }
      set { SetValue(TargetControlProperty, value); }
    }

    #endregion

    #region BrokenRules property

    /// <summary>
    /// Gets the broken rules collection from the
    /// business object.
    /// </summary>
    public static readonly DependencyProperty BrokenRulesProperty = DependencyProperty.Register(
      "BrokenRules",
      typeof(ObservableCollection<BrokenRule>),
      typeof(PropertyStatus),
      null);

    /// <summary>
    /// Gets the broken rules collection from the
    /// business object.
    /// </summary>
    public ObservableCollection<BrokenRule> BrokenRules
    {
      get { return (ObservableCollection<BrokenRule>)GetValue(BrokenRulesProperty); }
      private set { SetValue(BrokenRulesProperty, value); }
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

    private bool _isBusy = false;
    /// <summary>
    /// Gets a value indicating whether the property
    /// is busy with an asynchronous operation.
    /// </summary>
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
      Popup popup = (Popup)FindChild(this, "popup");
      if (popup != null && sender is UIElement)
      {
        Point p = e.GetPosition((UIElement)sender);
        Size size = ((UIElement)sender).DesiredSize;
        // ensure events are attached only once.
        popup.Child.MouseLeave -= new MouseEventHandler(popup_MouseLeave);
        popup.Child.MouseLeave += new MouseEventHandler(popup_MouseLeave);
        ((ItemsControl)popup.Child).ItemsSource = BrokenRules;

        popup.VerticalOffset = p.Y + size.Height;
        popup.HorizontalOffset = p.X + size.Width;
        popup.IsOpen = true;
      }
    }

    private void image_MouseLeave(object sender, MouseEventArgs e)
    {
      Popup popup = (Popup)FindChild(this, "popup");
      popup.IsOpen = false;
    }

    void popup_MouseLeave(object sender, MouseEventArgs e)
    {
      Popup popup = (Popup)FindChild(this, "popup");
      popup.IsOpen = false;
    }

    #endregion

    #region State management

    private void UpdateState()
    {
      Popup popup = (Popup)FindChild(this, "popup");
      if (popup != null)
        popup.IsOpen = false;

      BusinessBase businessObject = _source as BusinessBase;
      if (businessObject != null)
      {
        var allRules = (from r in businessObject.BrokenRulesCollection
                        where r.Property == _bindingPath
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
        {
          RuleSeverity = worst.Severity;
          RuleDescription = worst.Description;
        }
        else
          RuleDescription = string.Empty;

        IsValid = BrokenRules.Count == 0;
        GoToState(true);
      }
      else
      {
        BrokenRules.Clear();
        RuleDescription = string.Empty;
        IsValid = true;
        GoToState(true);
      }
    }

    private void GoToState(bool useTransitions)
    {
      DisablePopup(_lastImage);
      HandleTarget();

      BusyAnimation busy = FindChild(this, "busy") as BusyAnimation;
      if (busy != null)
        busy.IsRunning = IsBusy;

      if (IsBusy)
      {
        VisualStateManager.GoToState(this, "Busy", useTransitions);
      }
      else if (IsValid)
      {
        VisualStateManager.GoToState(this, "Valid", useTransitions);
      }
      else
      {
        VisualStateManager.GoToState(this, RuleSeverity.ToString(), useTransitions);
        _lastImage = (FrameworkElement)FindChild(this, string.Format("{0}Image", RuleSeverity.ToString().ToLower()));
        EnablePopup(_lastImage);
      }
    }

    #endregion

    #region TargetControl

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
              if (MethodCaller.IsMethodImplemented(TargetControl, "set_IsReadOnly", false))
                MethodCaller.CallMethod(TargetControl, "set_IsReadOnly", false);
              else
                MethodCaller.CallMethodIfImplemented(TargetControl, "set_IsEnabled", true);
            }
            else
            {
              if (MethodCaller.IsMethodImplemented(TargetControl, "set_IsReadOnly", true))
                MethodCaller.CallMethod(TargetControl, "set_IsReadOnly", true);
              else
                MethodCaller.CallMethodIfImplemented(TargetControl, "set_IsEnabled", false);
            }
          }

          CanRead = b.CanReadProperty(_bindingPath);
          if (TargetControl != null && !CanRead)
          {
            if (MethodCaller.IsMethodImplemented(TargetControl, "set_Content", null))
              MethodCaller.CallMethod(TargetControl, "set_Content", null);
            else
              MethodCaller.CallMethodIfImplemented(TargetControl, "set_Text", "");
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
