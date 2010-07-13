Imports Microsoft.VisualBasic
Imports Csla
Imports System

Namespace WpfDesignTime.Business
  <Serializable()> _
  Public Class ResourceInfo
    Inherits ReadOnlyBase(Of ResourceInfo)
    Private _id As Integer
    Public ReadOnly Property Id() As Integer
      Get
        Return _id
      End Get
    End Property

    Private _name As String
    Public ReadOnly Property Name() As String
      Get
        Return _name
      End Get
    End Property

    Public Overrides Function ToString() As String
      Return _name
    End Function

    Friend Sub New(ByVal id As Integer, ByVal lastname As String, ByVal firstname As String)
      _id = id
      _name = String.Format("{0}, {1}", lastname, firstname)
    End Sub
  End Class
End Namespace