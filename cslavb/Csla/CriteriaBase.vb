Imports System
Imports Csla.Serialization.Mobile
Imports Csla.Core
Imports System.ComponentModel

''' <summary>
''' Base type from which Criteria classes can be
''' derived in a business class. 
''' </summary>
<Serializable()> _
Public MustInherit Class CriteriaBase
  Inherits Core.ManagedObjectBase
  Implements ICriteria

  Private Shared _forceInit As Boolean = True

  ''' <summary>
  ''' Defines the TypeName property.
  ''' </summary>
  <EditorBrowsable(EditorBrowsableState.Never)> _
  Public Shared ReadOnly TypeNameProperty As PropertyInfo(Of String) = RegisterProperty( _
    GetType(CriteriaBase), New PropertyInfo(Of String)("TypeName"))

  <NonSerialized()> _
  <Csla.NotUndoable()> _
  Private _objectType As Type

  ''' <summary>
  ''' Assembly qualified type name of the business 
  ''' object to be instantiated by
  ''' the server-side DataPortal. 
  ''' </summary>
  Public Property TypeName() As String
    Get
      Return ReadProperty(TypeNameProperty)
    End Get
    Protected Set(ByVal value As String)
      LoadProperty(TypeNameProperty, value)
    End Set
  End Property

  ''' <summary>
  ''' Type of the business object to be instantiated by
  ''' the server-side DataPortal. 
  ''' </summary>
  Public ReadOnly Property ObjectType() As Type Implements ICriteria.ObjectType
    Get
      Return _objectType
    End Get
  End Property

#If SILVERLIGHT Then
' Do NOTHING fow now
#Else
  ''' <summary>
  ''' Initializes empty CriteriaBase. The type of
  ''' business object to be created by the DataPortal
  ''' MUST be supplied by the subclass.
  ''' </summary>
  Protected Sub New()

  End Sub

  <System.Runtime.Serialization.OnDeserialized()> _
  Private Sub OnDeserializedHandler(ByVal context As System.Runtime.Serialization.StreamingContext)
    OnDeserialized(context)
  End Sub

  ''' <summary>
  ''' This method is called on a newly deserialized object
  ''' after deserialization is complete.
  ''' </summary>
  ''' <param name="context">Serialization context object.</param>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub OnDeserialized(ByVal context As System.Runtime.Serialization.StreamingContext)

    _forceInit = _forceInit And False
    If FieldManager IsNot Nothing Then
      FieldManager.SetPropertyList(Me.GetType())
    End If
  End Sub
#End If

  ''' <summary>
  ''' Initializes CriteriaBase with the type of
  ''' business object to be created by the DataPortal.
  ''' </summary>
  ''' <param name="type">The type of the
  ''' business object the data portal should create.</param>
  Protected Sub New(ByVal type As Type)
    _objectType = type
  End Sub

End Class
