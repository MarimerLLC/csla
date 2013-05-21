using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Views;

namespace Csla.Axml.Binding
{
  public class BindingManager
  {
    private readonly Activity _activity;

    public BindingManager(Activity activity)
    {
      _activity = activity;
      Bindings = new List<Binding>();
    }

    public List<Binding> Bindings { get; private set; }

    public void Add(int viewId, string targetProperty, object source, string sourceProperty)
    {
      var target = _activity.FindViewById(viewId);
      Add(target, targetProperty, source, sourceProperty);
    }

    public void Add(View target, string targetProperty, object source, string sourceProperty)
    {
      Add(new Binding(target, targetProperty, source, sourceProperty));
    }

    public void Add(Binding binding)
    {
      Remove(binding.Target, binding.TargetProperty.Name, binding.Source, binding.SourceProperty.Name);
      Bindings.Add(binding);
    }

    public void Remove(View target, string targetProperty, object source, string sourceProperty)
    {
      var binding = Bindings.FirstOrDefault(r => ReferenceEquals(r.Target, target) &&
                                                 r.TargetProperty.Name == targetProperty &&
                                                 ReferenceEquals(r.Source, source) &&
                                                 r.SourceProperty.Name == sourceProperty);
      if (binding != null)
        Remove(binding);
    }

    public void Remove(Binding binding)
    {
      binding.Dispose();
      Bindings.Remove(binding);
    }

    public void RemoveAll()
    {
        for (var i = Bindings.Count - 1; i >= 0; i--)
        {
            Remove(Bindings[i]);
        }
    }

    public IEnumerable<Binding> GetBindingsForView(View view)
    {
      return Bindings.Where(r => ReferenceEquals(r.Target, view));
    }

    public void UpdateSourceForView(View view)
    {
      foreach (var item in GetBindingsForView(view))
        item.UpdateSource();
    }

    public void UpdateSourceForLastView()
    {
      var view = _activity.CurrentFocus;
      if (view != null)
        UpdateSourceForView(view);
    }
  }
}