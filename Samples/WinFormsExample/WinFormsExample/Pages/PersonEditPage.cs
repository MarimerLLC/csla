using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusinessLibrary;
using Csla;

namespace WinFormsExample.Pages
{
  public partial class PersonEditPage : UserControl, IUseContext
  {
    public PersonEditPage(IDataPortal<PersonEdit> portal)
    {
      _portal = portal;
      InitializeComponent();
    }

    public object Context { get; set; }
    private IDataPortal<PersonEdit> _portal;

    private async void PersonEditPage_Load(object sender, EventArgs e)
    {
      var personInfo = (PersonInfo)Context;
      PersonEdit personEdit;
      if (personInfo == null)
        personEdit = await _portal.CreateAsync();
      else
        personEdit = await _portal.FetchAsync(personInfo.Id);
      nameTextBox.DataBindings.Add(nameof(nameTextBox.Text), personEdit, nameof(personEdit.Name));
      saveButton.DataBindings.Add(nameof(saveButton.Enabled), personEdit, nameof(personEdit.IsSavable));
      errorProvider1.DataSource = personEdit;
    }

    private async void saveButton_Click(object sender, EventArgs e)
    {
      saveButton.Enabled = false;
      saveButton.Text = "Saving";
      var personEdit = (PersonEdit)errorProvider1.DataSource;
      personEdit.ApplyEdit();
      personEdit = await personEdit.SaveAsync();
      MainForm.Instance.ShowPage(typeof(PersonListPage));
    }
  }
}
