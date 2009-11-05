using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Csla.Test.Windows
{
  public partial class PersonForm : Form
  {
    public PersonForm()
    {
      InitializeComponent();
    }

    public void BindUI(EditablePerson person)
    {
      editablePersonBindingSource.DataSource = person;
      readWriteAuthorization1.ResetControlAuthorization();
    }

    private void editablePersonBindingSource_CurrentItemChanged(object sender, EventArgs e)
    {
      Debug.Print("applying authorization");
      readWriteAuthorization1.ResetControlAuthorization();
    }
  }
}
