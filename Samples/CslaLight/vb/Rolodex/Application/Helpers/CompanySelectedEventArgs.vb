Imports Microsoft.VisualBasic
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

Namespace Rolodex
  Public Class CompanySelectedEventArgs
    Inherits EventArgs
    Private Sub New()
    End Sub
    Public Sub New(ByVal companyID As Integer)
      _companyID = companyID
    End Sub
    Private _companyID As Integer
    Public ReadOnly Property CompanyID() As Integer
      Get
        Return _companyID
      End Get
    End Property
  End Class
End Namespace
