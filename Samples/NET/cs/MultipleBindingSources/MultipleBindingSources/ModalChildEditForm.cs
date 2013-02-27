using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultipleBindingSources
{
  public partial class ModalChildEditForm : Form
  {
    private ModalChildEditForm()
    {
      InitializeComponent();
    }

    public ModalChildEditForm(Child data) : this()
    {
       childrenBindingSource.BindDataSource(data); 
    }

    private void Form3_FormClosed(object sender, FormClosedEventArgs e)
    {
       childrenBindingSource.UnbindDataSource(true, false);
    }

  }
}
