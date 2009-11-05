Public Class NewEmployee
  Inherits SequentialWorkflowActivity

#Region " Dependency properties "

  Public Property EmployeeData() As NewEmployeeData
    Get
      Return DirectCast(GetValue(EmployeeDataProperty), NewEmployeeData)
    End Get

    Set(ByVal value As NewEmployeeData)
      SetValue(EmployeeDataProperty, value)
    End Set
  End Property

  Public Shared ReadOnly EmployeeDataProperty As DependencyProperty = _
          DependencyProperty.Register("EmployeeData", GetType(NewEmployeeData), GetType(NewEmployee), Nothing)


  Public Shared ErrorTextProperty As System.Workflow.ComponentModel.DependencyProperty = DependencyProperty.Register("ErrorText", GetType(System.String), GetType(EmployeeWorkflows.NewEmployee))

  <System.ComponentModel.DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)> _
    <System.ComponentModel.BrowsableAttribute(True)> _
      <System.ComponentModel.CategoryAttribute("Activity")> _
        Public Property ErrorText() As System.String
    Get
      Return CType(MyBase.GetValue(EmployeeWorkflows.NewEmployee.ErrorTextProperty), String)

    End Get
    Set(ByVal value As System.String)
      MyBase.SetValue(EmployeeWorkflows.NewEmployee.ErrorTextProperty, value)

    End Set
  End Property

#End Region

  ''' <summary>
  ''' Add employee to the list of consultants on
  ''' the bench.
  ''' </summary>
  Private Sub AddToBench_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    Dim svc As New BenchService.BenchSvcClient
    Dim request As New BenchService.ConsultantUpdateData
    request.Name = EmployeeData.Name
    request.IsNew = True
    Dim reply As BenchService.ConsultantData = svc.UpdateConsultant(request)

  End Sub

  Private Sub SetErrorMessage_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

    ErrorText = "Invalid employee data"

  End Sub


#Region " Other activities "

  Private Sub AddToHR_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

  End Sub

  Private Sub AddToBenefitsDB_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

  End Sub

  Private Sub SendBenefitsEmail_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

  End Sub

  Private Sub SendWelcomePacket_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

  End Sub

  Private Sub ScheduleMagenicU_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

  End Sub

  Private Sub EmailManager_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

  End Sub

  Private Sub SendConfirmationEmail_ExecuteCode(ByVal sender As System.Object, ByVal e As System.EventArgs)

  End Sub
  Public _benchServiceChannelToken As System.Workflow.Activities.ChannelToken = New System.Workflow.Activities.ChannelToken
  Public _updatedConsultant As EmployeeWorkflows.BenchService.ConsultantData = New EmployeeWorkflows.BenchService.ConsultantData
  Public _newConsultant As EmployeeWorkflows.BenchService.ConsultantUpdateData = New EmployeeWorkflows.BenchService.ConsultantUpdateData

#End Region

End Class
