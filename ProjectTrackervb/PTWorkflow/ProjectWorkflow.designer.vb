<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProjectWorkflow

  'NOTE: The following procedure is required by the Workflow Designer
  'It can be modified using the Workflow Designer.  
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerNonUserCode()> _
  Private Sub InitializeComponent()
    Me.CanModifyActivities = True
    Me.notifySponsor = New System.Workflow.Activities.CodeActivity
    Me.notifyResources = New System.Workflow.Activities.CodeActivity
    Me.sequenceActivity2 = New System.Workflow.Activities.SequenceActivity
    Me.sequenceActivity1 = New System.Workflow.Activities.SequenceActivity
    Me.parallelActivity1 = New System.Workflow.Activities.ParallelActivity
    Me.closeProject = New System.Workflow.Activities.CodeActivity
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
    'closeProject
    '
    Me.closeProject.Name = "closeProject"
    AddHandler Me.closeProject.ExecuteCode, AddressOf Me.closeProject_ExecuteCode
    '
    'ProjectWorkflow
    '
    Me.Activities.Add(Me.closeProject)
    Me.Activities.Add(Me.parallelActivity1)
    Me.Name = "ProjectWorkflow"
    Me.CanModifyActivities = False

  End Sub
  Private notifySponsor As System.Workflow.Activities.CodeActivity
  Private notifyResources As System.Workflow.Activities.CodeActivity
  Private sequenceActivity2 As System.Workflow.Activities.SequenceActivity
  Private sequenceActivity1 As System.Workflow.Activities.SequenceActivity
  Private parallelActivity1 As System.Workflow.Activities.ParallelActivity
  Private closeProject As System.Workflow.Activities.CodeActivity

End Class
