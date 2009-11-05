<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial class NewEmployee

    'NOTE: The following procedure is required by the Workflow Designer
    'It can be modified using the Workflow Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Private Sub InitializeComponent()
    Me.CanModifyActivities = True
    Dim ruleconditionreference1 As System.Workflow.Activities.Rules.RuleConditionReference = New System.Workflow.Activities.Rules.RuleConditionReference
    Me.SendWelcomePacket = New System.Workflow.Activities.CodeActivity
    Me.ArtificialDelay = New System.Workflow.Activities.DelayActivity
    Me.ScheduleMagenicU = New System.Workflow.Activities.CodeActivity
    Me.EmailManager = New System.Workflow.Activities.CodeActivity
    Me.AddToBench = New System.Workflow.Activities.CodeActivity
    Me.SendBenefitsEmail = New System.Workflow.Activities.CodeActivity
    Me.AddToBenefitsDB = New System.Workflow.Activities.CodeActivity
    Me.Welcome = New System.Workflow.Activities.SequenceActivity
    Me.Training = New System.Workflow.Activities.SequenceActivity
    Me.BenchManager = New System.Workflow.Activities.SequenceActivity
    Me.Benefits = New System.Workflow.Activities.SequenceActivity
    Me.SetErrorMessage = New System.Workflow.Activities.CodeActivity
    Me.SendConfirmationEmail = New System.Workflow.Activities.CodeActivity
    Me.SetupEmployee = New System.Workflow.Activities.ParallelActivity
    Me.AddToHR = New System.Workflow.Activities.CodeActivity
    Me.InvalidEmployee = New System.Workflow.Activities.IfElseBranchActivity
    Me.ValidEmployee = New System.Workflow.Activities.IfElseBranchActivity
    Me.ValidateEmployee = New System.Workflow.Activities.IfElseActivity
    '
    'SendWelcomePacket
    '
    Me.SendWelcomePacket.Name = "SendWelcomePacket"
    AddHandler Me.SendWelcomePacket.ExecuteCode, AddressOf Me.SendWelcomePacket_ExecuteCode
    '
    'ArtificialDelay
    '
    Me.ArtificialDelay.Description = "Artificial delay for effect"
    Me.ArtificialDelay.Name = "ArtificialDelay"
    Me.ArtificialDelay.TimeoutDuration = System.TimeSpan.Parse("00:00:05")
    '
    'ScheduleMagenicU
    '
    Me.ScheduleMagenicU.Name = "ScheduleMagenicU"
    AddHandler Me.ScheduleMagenicU.ExecuteCode, AddressOf Me.ScheduleMagenicU_ExecuteCode
    '
    'EmailManager
    '
    Me.EmailManager.Description = "Notify the branch manager that the new employee is on the bench"
    Me.EmailManager.Name = "EmailManager"
    AddHandler Me.EmailManager.ExecuteCode, AddressOf Me.EmailManager_ExecuteCode
    '
    'AddToBench
    '
    Me.AddToBench.Description = "Add the new employee to the bench so they can be scheduled for work"
    Me.AddToBench.Name = "AddToBench"
    AddHandler Me.AddToBench.ExecuteCode, AddressOf Me.AddToBench_ExecuteCode
    '
    'SendBenefitsEmail
    '
    Me.SendBenefitsEmail.Name = "SendBenefitsEmail"
    AddHandler Me.SendBenefitsEmail.ExecuteCode, AddressOf Me.SendBenefitsEmail_ExecuteCode
    '
    'AddToBenefitsDB
    '
    Me.AddToBenefitsDB.Name = "AddToBenefitsDB"
    AddHandler Me.AddToBenefitsDB.ExecuteCode, AddressOf Me.AddToBenefitsDB_ExecuteCode
    '
    'Welcome
    '
    Me.Welcome.Activities.Add(Me.SendWelcomePacket)
    Me.Welcome.Name = "Welcome"
    '
    'Training
    '
    Me.Training.Activities.Add(Me.ScheduleMagenicU)
    Me.Training.Activities.Add(Me.ArtificialDelay)
    Me.Training.Name = "Training"
    '
    'BenchManager
    '
    Me.BenchManager.Activities.Add(Me.AddToBench)
    Me.BenchManager.Activities.Add(Me.EmailManager)
    Me.BenchManager.Name = "BenchManager"
    '
    'Benefits
    '
    Me.Benefits.Activities.Add(Me.AddToBenefitsDB)
    Me.Benefits.Activities.Add(Me.SendBenefitsEmail)
    Me.Benefits.Name = "Benefits"
    '
    'SetErrorMessage
    '
    Me.SetErrorMessage.Name = "SetErrorMessage"
    AddHandler Me.SetErrorMessage.ExecuteCode, AddressOf Me.SetErrorMessage_ExecuteCode
    '
    'SendConfirmationEmail
    '
    Me.SendConfirmationEmail.Name = "SendConfirmationEmail"
    AddHandler Me.SendConfirmationEmail.ExecuteCode, AddressOf Me.SendConfirmationEmail_ExecuteCode
    '
    'SetupEmployee
    '
    Me.SetupEmployee.Activities.Add(Me.Benefits)
    Me.SetupEmployee.Activities.Add(Me.BenchManager)
    Me.SetupEmployee.Activities.Add(Me.Training)
    Me.SetupEmployee.Activities.Add(Me.Welcome)
    Me.SetupEmployee.Name = "SetupEmployee"
    '
    'AddToHR
    '
    Me.AddToHR.Description = "Add employee data to the HR database"
    Me.AddToHR.Name = "AddToHR"
    AddHandler Me.AddToHR.ExecuteCode, AddressOf Me.AddToHR_ExecuteCode
    '
    'InvalidEmployee
    '
    Me.InvalidEmployee.Activities.Add(Me.SetErrorMessage)
    Me.InvalidEmployee.Name = "InvalidEmployee"
    '
    'ValidEmployee
    '
    Me.ValidEmployee.Activities.Add(Me.AddToHR)
    Me.ValidEmployee.Activities.Add(Me.SetupEmployee)
    Me.ValidEmployee.Activities.Add(Me.SendConfirmationEmail)
    ruleconditionreference1.ConditionName = "ValidateEmployee"
    Me.ValidEmployee.Condition = ruleconditionreference1
    Me.ValidEmployee.Name = "ValidEmployee"
    '
    'ValidateEmployee
    '
    Me.ValidateEmployee.Activities.Add(Me.ValidEmployee)
    Me.ValidateEmployee.Activities.Add(Me.InvalidEmployee)
    Me.ValidateEmployee.Name = "ValidateEmployee"
    '
    'NewEmployee
    '
    Me.Activities.Add(Me.ValidateEmployee)
    Me.Name = "NewEmployee"
    Me.CanModifyActivities = False

  End Sub
  Private ArtificialDelay As System.Workflow.Activities.DelayActivity
  Private SetErrorMessage As System.Workflow.Activities.CodeActivity
  Private InvalidEmployee As System.Workflow.Activities.IfElseBranchActivity
  Private ValidEmployee As System.Workflow.Activities.IfElseBranchActivity
  Private ValidateEmployee As System.Workflow.Activities.IfElseActivity
  Private BenchManager As System.Workflow.Activities.SequenceActivity
  Private Benefits As System.Workflow.Activities.SequenceActivity
  Private SetupEmployee As System.Workflow.Activities.ParallelActivity
  Private SendBenefitsEmail As System.Workflow.Activities.CodeActivity
  Private AddToBenefitsDB As System.Workflow.Activities.CodeActivity
  Private Training As System.Workflow.Activities.SequenceActivity
  Private SendWelcomePacket As System.Workflow.Activities.CodeActivity
  Private ScheduleMagenicU As System.Workflow.Activities.CodeActivity
  Private AddToBench As System.Workflow.Activities.CodeActivity
  Private Welcome As System.Workflow.Activities.SequenceActivity
  Private EmailManager As System.Workflow.Activities.CodeActivity
  Private SendConfirmationEmail As System.Workflow.Activities.CodeActivity
  Private AddToHR As System.Workflow.Activities.CodeActivity

End Class
