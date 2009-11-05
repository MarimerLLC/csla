Imports System
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Namespace NavigationApp
  Public Class CustomControlFactory
    Implements Csla.Silverlight.IControlNameFactory

#Region "IControlNameFactory Members"

    Public Function ControlNameToControl(ByVal controlName As String) As Control Implements Csla.Silverlight.IControlNameFactory.ControlNameToControl
      If controlName.ToUpper().Contains("_1".ToUpper()) Then
        Return New ControlOne()
      ElseIf controlName.ToUpper().Contains("_2".ToUpper()) Then
        Return New ControlTwo()
      ElseIf controlName.ToUpper().Contains("_5".ToUpper()) Then
        Return New ControlFive()
      Else
        Return Nothing
      End If
    End Function

    Public Function ControlToControlName(ByVal control As Control) As String Implements Csla.Silverlight.IControlNameFactory.ControlToControlName
      If TypeOf control Is ControlOne Then
        Return "Control_1"
      End If
      If TypeOf control Is ControlTwo Then
        Return "Control_2"
      End If
      If TypeOf control Is ControlFive Then
        Return "Control_5"
      End If
      Return control.[GetType]().AssemblyQualifiedName
    End Function

#End Region
  End Class
End Namespace