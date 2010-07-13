Imports Csla.Core
Imports Csla.Rules

Public Class AuthorizationRule
  Inherits Csla.Rules.AuthorizationRule
  ' TODO: Add additional parameters to your rule to the constructor
  ''' <summary>
  ''' Initializes a new instance of the <see cref="AuthorizationRule"/> class.
  ''' </summary>
  Private Class RuleName
    Inherits Csla.Rules.BusinessRule

    ' TODO: Add additional parameters to your rule to the constructor
    Public Sub New(ByVal primaryProperty As IPropertyInfo)

      ' TODO: If you are  going to add InputProperties make sure to uncomment line below as InputProperties is NULL by default
      'if (InputProperties == null) InputProperties = new List<IPropertyInfo>();

      ' TODO: Add additional constructor code here 



      ' TODO: Marke rule for IsAsync if Execute contains asyncronous code 
      ' IsAsync = true; 
      MyBase.New(primaryProperty)
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

  ''' <param name="action">Action this rule will enforce.</param>
  ''' <param name="element">Method or property.</param>
  Public Sub New(ByVal action As AuthorizationActions, ByVal element As IMemberInfo)
    MyBase.New(action, element)
    ' TODO: Add additional constructor code here 


  End Sub


  ' TODO: Add additional parameters to your rule to the constructor
  ''' <summary>
  ''' Initializes a new instance of the <see cref="AuthorizationRule"/> class.
  ''' </summary>
  ''' <param name="action">The action.</param>
  Public Sub New(ByVal action As AuthorizationActions)
    MyBase.New(action)
    ' TODO: Add additional constructor code here 

  End Sub

  Protected Overrides Sub Execute(ByVal context As AuthorizationContext)
    ' TODO: Add actual rule code here. 
    'if (!access_condition)
    '{
    '  context.HasPermission = false;
    '}
  End Sub
End Class
