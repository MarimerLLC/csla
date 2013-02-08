using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.ComponentModel;

namespace SimpleApp
{
  [Activity(Label = "CSLA 4 Demo", MainLauncher = true)]
  public class Activity1 : Activity
  {
    protected override void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);

      // Set our view from the "main" layout resource
      SetContentView(Resource.Layout.Main);

      Initialize();
    }

    bool Initialized;
    BindingManager Bindings;
    TextView ErrorText;
    Person Model;

    private void Initialize()
    {
      if (Initialized) return;
      Initialized = true;

      // create binding manager
      Bindings = new BindingManager(this);

      ErrorText = FindViewById<TextView>(Resource.Id.ErrorText);

      Button saveButton = 
        FindViewById<Button>(Resource.Id.SaveButton);
      saveButton.Click += saveButton_Click;

      Person.GetPerson(123, (o, e) =>
      {
        if (e.Error != null)
          ErrorText.Text =
            string.Format("Get: {0}-{1}", e.Error.GetType().Name, e.Error.Message);
        else
          InitializeBindings(e.Object);
      });
    }

    void saveButton_Click(object sender, EventArgs e)
    {
      ErrorText.Text = string.Empty;
      Bindings.UpdateSourceForLastView();
      if (Model.IsSavable)
      {
        try
        {
          Model.BeginSave((src, a) =>
          {
            if (a.Error != null)
              ErrorText.Text =
              string.Format("Save: {0}-{1}", a.Error.GetType().Name, a.Error.Message);
            else
              InitializeBindings((Person)a.NewObject);
          });
        }
        catch (Exception ex)
        {
          ErrorText.Text =
            string.Format("Save: {0}-{1}", ex.GetType().Name, ex.Message);
        }
      }
      else if (!Model.IsValid)
      {
        var error = Model.BrokenRulesCollection.Where(r => r.Severity == Csla.Rules.RuleSeverity.Error).FirstOrDefault();
        if (error != null)
          ErrorText.Text = error.Description;
        else
          ErrorText.Text = "Object is invalid";
      }
      else
      {
        ErrorText.Text = "Nothing to save";
      }
    }

    private void InitializeBindings(Person model)
    {
      Model = model;
      Bindings.Add(Resource.Id.IdText, "Text", Model, "Id");
      Bindings.Add(Resource.Id.NameText, "Text", Model, "Name");
      Bindings.Add(Resource.Id.NameTextView, "Text", Model, "Name");
      Bindings.Add(Resource.Id.StatusText, "Text", Model, "Status");
      //Bindings.Add(Resource.Id.SaveButton, "Enabled", Model, "IsSavable");
      //Bindings.Add(new Binding(FindViewById(Resource.Id.SaveButton), "Visibility", Model, "IsSavable",
      //    (value) =>
      //    {
      //      if ((bool)value)
      //        return ViewStates.Visible;
      //      else
      //        return ViewStates.Gone;
      //    }));
    }
  }
}

