Imports Csla.Core
Imports Csla.Rules

Public Class BusinessRuleClass
  Inherits Csla.Rules.BusinessRule


  ' TODO: Add additional parameters to your rule to the constructor
  ''' <summary>
  ''' Initializes a new instance of the <see cref="BusinessRule" /> class.
  ''' </summary>
  ''' <param name="primaryProperty">The primary property.</param>
  Public Sub New(ByVal primaryProperty As IPropertyInfo)
    MyBase.New(primaryProperty)
    ' TODO: If you are  going to add InputProperties make sure to uncomment line below as InputProperties is NULL by default
    If InputProperties Is Nothing Then
      InputProperties = New List(Of IPropertyInfo)()
    End If

    ' TODO: Add additional constructor code here 


    ' TODO: Marke rule for IsAsync if Execute method implemets asyncronous calls
    ' IsAsync = True
  End Sub

  Protected Overrides Sub Execute(ByVal context As IRuleContext)
    ' TODO: Asyncronous rules 
    ' If rule is async make sure that ALL excution paths call context.Complete

    Dim brokencondition As Boolean
    ' TODO: Add actual rule code here. 
    If brokencondition Then
      context.AddErrorResult("Broken rule message")
    End If
  End Sub

End Class
