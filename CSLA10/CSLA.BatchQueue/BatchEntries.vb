Imports System.Messaging
Imports System.Runtime.Serialization.Formatters.Binary

''' <summary>
''' Contains a list of holding, pending and active batch
''' queue entries.
''' </summary>
<Serializable()> _
Public Class BatchEntries
  Inherits CollectionBase

  ''' <summary>
  ''' Returns a reference to an object with information about
  ''' a specific batch queue entry.
  ''' </summary>
  Default Public ReadOnly Property Entry(ByVal index As Integer) As BatchEntryInfo
    Get
      Return CType(list.Item(index), BatchEntryInfo)
    End Get
  End Property

  ''' <summary>
  ''' Returns a reference to an object with information about
  ''' a specific batch queue entry.
  ''' </summary>
  ''' <param name="ID">The ID value of the entry to return.</param>
  Default Public ReadOnly Property Entry(ByVal ID As Guid) As BatchEntryInfo
    Get
      Dim obj As BatchEntryInfo

      For Each obj In list
        If obj.ID.Equals(ID) Then
          Return obj
        End If
      Next
    End Get
  End Property

  Friend Sub New()

    ' prevent direct creation

  End Sub

  Friend Sub Add(ByVal Value As BatchEntryInfo)
    list.Add(Value)
  End Sub

End Class
