//-----------------------------------------------------------------------
// <copyright file="PersonForm.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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

    public void BindUI(EditablePerson person, bool useStandardActionExtender)
    {
      editablePersonBindingSource.DataSource = person;
      readWriteAuthorization1.ResetControlAuthorization();
      if (useStandardActionExtender)
      {
        cslaActionExtender1.ResetActionBehaviors(person);
      }
      else
      {
        cslaActionExtenderToolStrip1.ResetActionBehaviors(person);
      }
    }

    public void BindUI(EditablePerson person)
    {
      BindUI(person, true);
    }

    private void editablePersonBindingSource_CurrentItemChanged(object sender, EventArgs e)
    {
      Debug.Print("applying authorization");
      readWriteAuthorization1.ResetControlAuthorization();
    }
  }
}