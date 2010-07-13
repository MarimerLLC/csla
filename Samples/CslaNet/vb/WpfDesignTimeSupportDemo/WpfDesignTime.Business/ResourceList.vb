Imports Microsoft.VisualBasic
Imports Csla
Imports System

Namespace WpfDesignTime.Business
  <Serializable()> _
  Public Class ResourceList
    Inherits ReadOnlyListBase(Of ResourceList, ResourceInfo)
#Region " Factory Methods"

    Public Shared Function EmptyList() As ResourceList
      Return New ResourceList()
    End Function

    Public Shared Function GetResourceList() As ResourceList
      Return DataPortal.Fetch(Of ResourceList)()
    End Function

    Private Sub New()
      'INSTANT VB NOTE: Embedded comments are not maintained by Instant VB
      'ORIGINAL LINE: { /* require use of factory methods */ }
    End Sub

#End Region

#Region " Data Access"

    Private Overloads Sub DataPortal_Fetch()
      RaiseListChangedEvents = False
      IsReadOnly = False
      ' Simulated Database access
      For i As Integer = 1 To 9
        Me.Add(New ResourceInfo(i, "Last Name # " & i.ToString(), "First Name # " & i.ToString()))
      Next i

      IsReadOnly = True
      RaiseListChangedEvents = True
    End Sub

#End Region

#Region " Design Time Support"
    Private Function DesignTime_Create() As ResourceList
      IsReadOnly = False
      Me.Add(New ResourceInfo(1, "Doe", "John"))
      Me.Add(New ResourceInfo(2, "Doe", "Jane"))
      IsReadOnly = True
      Return Me
    End Function
#End Region

  End Class
End Namespace