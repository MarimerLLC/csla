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
  public class PropertyStatus : ContentControl
  {
    #region Constructors

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public PropertyStatus()
      : base()
    {
      RelativeTargetPath = "Parent";
      BrokenRules = new ObservableCollection<BrokenRule>();
      DefaultStyleKey = typeof(PropertyStatus);
      IsTabStop = false;

      Loaded += (o, e) => 
      { 
        var b = new System.Windows.Data.Binding();
        this.SetBinding(SourceProperty, b);
        GoToState(true); 
      };
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

    #region Dependency properties

    /// <summary>
    /// Gets or sets the source business
    /// object to which this control is bound.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
      "Source",
      typeof(object),
      typeof(PropertyStatus),
      new PropertyMetadata((o, e) => ((PropertyStatus)o).SetSource(e.OldValue, e.NewValue)));

    /// <summary>
    /// Gets or sets the name of the business object
    /// property to use for authorization and validation
    /// rule processing.
    /// </summary>
    public static readonly DependencyProperty PropertyProperty = DependencyProperty.Register(
      "Property",
      typeof(string),
      typeof(PropertyStatus),
      null);

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
    /// Gets or sets the path to the UI control
    /// to manipulate based on authorization processing.
    /// </summary>
    public static readonly DependencyProperty RelativeTargetPathProperty = DependencyProperty.Register(
      "RelativeTargetPath",
      typeof(string),
      typeof(PropertyStatus),
      new PropertyMetadata((o, e) => ((PropertyStatus)o).Target = null));

    /// <summary>
    /// Gets or sets the name of the UI control
    /// to manipulate based on authorization processing.
    /// </summary>
    public static readonly DependencyProperty RelativeTargetNameProperty = DependencyProperty.Register(
      "RelativeTargetName",
      typeof(string),
      typeof(PropertyStatus),
      new PropertyMetadata((o, e) => ((PropertyStatus)o).Target = null));

    #endregion

    #region Member fields and properties

    private DependencyObject _target;
    private bool _isReadOnly = false;
    private bool _isValid = true;
    private RuleSeverity _worst;
    private FrameworkElement _lastImage;
    private bool _isBusy;

    /// <summary>
    /// Gets or sets the source business
    /// object to which this control is bound.
    /// </summary>
    private object Source
    {
      get { return GetValue(SourceProperty); }
      set
      {
        object old = Source;
        SetValue(SourceProperty, value);
        SetSource(old, value);
      }
    }

    private bool _settingSource;

    private void SetSource(object old, object @new)
    {
      _settingSource = true;

      DetachSource(old);
      AttachSource(@new);

      BusinessBase bb = @new as BusinessBase;
      if (bb != null && !string.IsNullOrEmpty(Property))
        _isBusy = bb.IsPropertyBusy(Property);

      CheckProperty();
      UpdateState();

      _settingSource = false;
    }

    /// <summary>
    /// Gets or sets the name of the business object
    /// property to use for authorization and validation
    /// rule processing.
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
    /// Gets the broken rules collection from the
    /// business object.
    /// </summary>
    public ObservableCollection<BrokenRule> BrokenRules
    {
      get { return (ObservableCollection<BrokenRule>)GetValue(BrokenRulesProperty); }
      private set { SetValue(BrokenRulesProperty, value); }
    }

    /// <summary>
    /// Gets or sets the path to the UI control
    /// to manipulate based on authorization processing.
    /// </summary>
    public string RelativeTargetPath
    {
      get { return (string)GetValue(RelativeTargetPathProperty); }
      set
      {
        SetValue(RelativeTargetPathProperty, value);
        _target = null;
      }
    }

    /// <summary>
    /// Gets or sets the name of the UI control
    /// to manipulate based on authorization processing.
    /// </summary>
    public string RelativeTargetName
    {
      get { return (string)GetValue(RelativeTargetNameProperty); }
      set
      {
        SetValue(RelativeTargetNameProperty, value);
        _target = null;
      }
    }

    /// <summary>
    /// Gets or sets the UI control to manipulate
    /// based on authorization processing.
    /// </summary>
    public DependencyObject Target
    {
      get { return _target; }
      set { _target = value; }
    }

    #endregion

    #region Source

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
      {
        bool busy = e.Busy;
        BusinessBase bb = Source as BusinessBase;
        if (bb != null)
          busy = bb.IsPropertyBusy(Property);

        if (busy != _isBusy)
        {
          _isBusy = busy;
          UpdateState();
        }
      }
    }

    private void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!_isBusy && e.PropertyName == Property || string.IsNullOrEmpty(e.PropertyName))
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
      if (!_settingSource && _target == null)
      {
        _target = VisualTree.FindParent(RelativeTargetPath, this);
        if (!string.IsNullOrEmpty(RelativeTargetName))
          _target = VisualTree.FindByName(RelativeTargetName, _target);
      }
    }

    private void CheckProperty()
    {
      if (Source != null && !string.IsNullOrEmpty(Property))
      {
        var info = Source.GetType().GetProperty(Property);
        if (info != null)
        {
          _isReadOnly = !info.CanWrite;
          if (!_isReadOnly)
          {
            var setter = Source.GetType().GetMethod("set_" + Property);
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
      if (_target != null && !string.IsNullOrEmpty(Property))
      {
        var b = Source as Csla.Security.IAuthorizeReadWrite;
        if (b != null)
        {
          bool canWrite = b.CanWriteProperty(Property);
          if (canWrite && !_isReadOnly)
          {
            if (MethodCaller.IsMethodImplemented(_target, "set_IsReadOnly", false))
              MethodCaller.CallMethod(_target, "set_IsReadOnly", false);
            else
              MethodCaller.CallMethodIfImplemented(_target, "set_IsEnabled", true);
          }
          else
          {
            if (MethodCaller.IsMethodImplemented(_target, "set_IsReadOnly", true))
              MethodCaller.CallMethod(_target, "set_IsReadOnly", true);
            else
              MethodCaller.CallMethodIfImplemented(_target, "set_IsEnabled", false);
          }

          bool canRead = b.CanReadProperty(Property);
          if (!canRead)
          {
            if (MethodCaller.IsMethodImplemented(_target, "set_Content", null))
              MethodCaller.CallMethod(_target, "set_Content", null);
            else
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
