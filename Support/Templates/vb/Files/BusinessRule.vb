Imports Csla.Core
Imports Csla.Rules

Public Class BusinessRule
  Inherits Csla.Rules.BusinessRule


  ' TODO: Add additional parameters to your rule to the constructor
  Public Sub New(ByVal primaryProperty As IPropertyInfo)
    MyBase.New(primaryProperty)
    ' TODO: If you are  going to add InputProperties make sure to uncomment line below as InputProperties is NULL by default
    'if (InputProperties == null) InputProperties = new List<IPropertyInfo>();

    ' TODO: Add additional constructor code here 



    ' TODO: Marke rule for IsAsync if Execute contains asyncronous code 
    ' IsAsync = true; 

  End Sub

  Protected Overrides Sub Execute(ByVal context As RuleContext)
    ' TODO: Add actual rule code here. 
    'if (broken condition)
    '{
    '  context.AddErrorResult("Broken rule message");
    '}

    If IsAsync Then
      context.Complete()
    End If
  End Sub

End Class
