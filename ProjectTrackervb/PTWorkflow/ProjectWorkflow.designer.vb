<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProjectWorkflow

  'NOTE: The following procedure is required by the Workflow Designer
  'It can be modified using the Workflow Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Private Sub InitializeComponent()
    Me.CanModifyActivities = True
    Dim activitybind1 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind
    Dim activitybind2 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind
    Me.notifySponsor = New System.Workflow.Activities.CodeActivity
    Me.notifyResources = New System.Workflow.Activities.CodeActivity
    Me.sequenceActivity2 = New System.Workflow.Activities.SequenceActivity
    Me.sequenceActivity1 = New System.Workflow.Activities.SequenceActivity
    Me.parallelActivity1 = New System.Workflow.Activities.ParallelActivity
    Me.closeProject1 = New PTWfActivities.CloseProject
    Me.getProject1 = New PTWfActivities.GetProject
    '
    'notifySponsor
    '
    Me.notifySponsor.Name = "notifySponsor"
    AddHandler Me.notifySponsor.ExecuteCode, AddressOf Me.notifySponsor_ExecuteCode
    '
    'notifyResources
    '
    Me.notifyResources.Name = "notifyResources"
    AddHandler Me.notifyResources.ExecuteCode, AddressOf Me.notifyResources_ExecuteCode
    '
    'sequenceActivity2
    '
    Me.sequenceActivity2.Activities.Add(Me.notifySponsor)
    Me.sequenceActivity2.Name = "sequenceActivity2"
    '
    'sequenceActivity1
    '
    Me.sequenceActivity1.Activities.Add(Me.notifyResources)
    Me.sequenceActivity1.Name = "sequenceActivity1"
    '
    'parallelActivity1
    '
    Me.parallelActivity1.Activities.Add(Me.sequenceActivity1)
    Me.parallelActivity1.Activities.Add(Me.sequenceActivity2)
    Me.parallelActivity1.Name = "parallelActivity1"
    '
    'closeProject1
    '
    Me.closeProject1.Name = "closeProject1"
    activitybind1.Name = "getProject1"
    activitybind1.Path = "Project"
    Me.closeProject1.SetBinding(System.Workflow.ComponentModel.DependencyProperty.FromName("Project", GetType(PTWfActivities.CloseProject)), CType(activitybind1, System.Workflow.ComponentModel.ActivityBind))
    '
    'getProject1
    '
    Me.getProject1.Name = "getProject1"
    Me.getProject1.Project = Nothing
    activitybind2.Name = "ProjectWorkflow"
    activitybind2.Path = "ProjectId"
    Me.getProject1.SetBinding(System.Workflow.ComponentModel.DependencyProperty.FromName("ProjectId", GetType(PTWfActivities.GetProject)), CType(activitybind2, System.Workflow.ComponentModel.ActivityBind))
    '
    'ProjectWorkflow
    '
    Me.Activities.Add(Me.getProject1)
    Me.Activities.Add(Me.closeProject1)
    Me.Activities.Add(Me.parallelActivity1)
    Me.Name = "ProjectWorkflow"
    Me.CanModifyActivities = False

  End Sub
  Private closeProject1 As PTWfActivities.CloseProject
  Private getProject1 As PTWfActivities.GetProject
  Private notifySponsor As System.Workflow.Activities.CodeActivity
  Private notifyResources As System.Workflow.Activities.CodeActivity
  Private sequenceActivity2 As System.Workflow.Activities.SequenceActivity
  Private sequenceActivity1 As System.Workflow.Activities.SequenceActivity
  Private parallelActivity1 As System.Workflow.Activities.ParallelActivity

End Class
