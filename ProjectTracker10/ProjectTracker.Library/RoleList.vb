<Serializable()> _
Public Class RoleList
  Inherits NameValueList

#Region " Shared Methods "

  Public Shared Function GetList() As RoleList
    Return CType(DataPortal.Fetch(New Criteria), RoleList)
  End Function

#End Region

#Region " Constructors "

  Private Sub New()
    ' prevent direct creation
  End Sub

  ' this constructor overload is required because
  ' the base class (NameObjectCollectionBase) implements
  ' ISerializable
  Private Sub New( _
        ByVal info As System.Runtime.Serialization.SerializationInfo, _
        ByVal context As System.Runtime.Serialization.StreamingContext)
    MyBase.New(info, context)
  End Sub

#End Region

#Region " Criteria "

  <Serializable()> _
  Private Class Criteria
    ' add criteria here
  End Class

#End Region

#Region " Data Access "

  ' called by DataPortal to load data from the database
  Protected Overrides Sub DataPortal_Fetch(ByVal Criteria As Object)
    SimpleFetch("PTracker", "Roles", "id", "name")
  End Sub

#End Region

End Class
