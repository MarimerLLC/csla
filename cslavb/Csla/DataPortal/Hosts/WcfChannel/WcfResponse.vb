#If Not NET20 Then
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.Serialization

Namespace Server.Hosts.WcfChannel
  ''' <summary>
  ''' Response message for returning
  ''' the results of a data portal call.
  ''' </summary>
  <DataContract()> _
  Public Class WcfResponse
    <DataMember()> _
    Private _result As Object

    ''' <summary>
    ''' Create new instance of object.
    ''' </summary>
    ''' <param name="result">Result object to be returned.</param>
    Public Sub New(ByVal result As Object)
      _result = result
    End Sub

    ''' <summary>
    ''' Criteria object describing business object.
    ''' </summary>
    Public Property Result() As Object
      Get
        Return _result
      End Get
      Set(ByVal value As Object)
        _result = value
      End Set
    End Property
  End Class
End Namespace
#End If