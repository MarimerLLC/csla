<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial class CloseProject

    'NOTE: The following procedure is required by the Workflow Designer
    'It can be modified using the Workflow Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Private Sub InitializeComponent()
    Me.CanModifyActivities = True
    Me.doClose = New System.Workflow.Activities.CodeActivity
    '
    'doClose
    '
    Me.doClose.Name = "doClose"
    AddHandler Me.doClose.ExecuteCode, AddressOf Me.doClose_ExecuteCode
    '
    'CloseProject
    '
    Me.Activities.Add(Me.doClose)
    Me.Name = "CloseProject"
    Me.CanModifyActivities = False

  End Sub
  Private doClose As System.Workflow.Activities.CodeActivity

End Class

