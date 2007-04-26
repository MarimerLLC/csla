<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProjectWorkflow

  'NOTE: The following procedure is required by the Workflow Designer
  'It can be modified using the Workflow Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Private Sub InitializeComponent()
    Me.CanModifyActivities = True
    Me.saveProject = New System.Workflow.Activities.CodeActivity
    Me.closeProject = New System.Workflow.Activities.CodeActivity
    Me.getProject = New System.Workflow.Activities.CodeActivity
    Me.login = New System.Workflow.Activities.CodeActivity
    '
    'saveProject
    '
    Me.saveProject.Name = "saveProject"
    AddHandler Me.saveProject.ExecuteCode, AddressOf Me.saveProject_ExecuteCode
    '
    'closeProject
    '
    Me.closeProject.Name = "closeProject"
    AddHandler Me.closeProject.ExecuteCode, AddressOf Me.closeProject_ExecuteCode
    '
    'getProject
    '
    Me.getProject.Name = "getProject"
    AddHandler Me.getProject.ExecuteCode, AddressOf Me.getProject_ExecuteCode
    '
    'login
    '
    Me.login.Name = "login"
    AddHandler Me.login.ExecuteCode, AddressOf Me.login_ExecuteCode
    '
    'ProjectWorkflow
    '
    Me.Activities.Add(Me.login)
    Me.Activities.Add(Me.getProject)
    Me.Activities.Add(Me.closeProject)
    Me.Activities.Add(Me.saveProject)
    Me.Name = "ProjectWorkflow"
    Me.CanModifyActivities = False

  End Sub
  Private saveProject As System.Workflow.Activities.CodeActivity
  Private getProject As System.Workflow.Activities.CodeActivity
  Private login As System.Workflow.Activities.CodeActivity
  Private closeProject As System.Workflow.Activities.CodeActivity

End Class
