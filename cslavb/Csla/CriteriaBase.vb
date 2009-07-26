Imports System
Imports Csla.Serialization.Mobile
Imports Csla.Core
Imports System.ComponentModel
#If SILVERLIGHT Then
Imports Csla.Serialization
#End If

''' <summary>
''' Base type from which Criteria classes can be
''' derived in a business class. 
''' </summary>
<Serializable()> _
Public Class CriteriaBase
  Inherits Core.ManagedObjectBase
  Implements ICriteria

  Private Shared _forceInit As Boolean = True

  ''' <summary>
  ''' Defines the TypeName property.
  ''' </summary>
  <EditorBrowsable(EditorBrowsableState.Never)> _
  Public Shared ReadOnly TypeNameProperty As PropertyInfo(Of String) = _
    RegisterProperty(Of CriteriaBase, String)(Function(c) (c.TypeName)) 'TODO: Is this correct?

  <NonSerialized()> _
  <Csla.NotUndoable()> _
  Private _objectType As Type

  ''' <summary>
  ''' Type of the business object to be instantiated by
  ''' the server-side DataPortal. 
  ''' </summary>
  Public ReadOnly Property ObjectType() As Type Implements ICriteria.ObjectType
    Get
      If _objectType Is Nothing AndAlso FieldManager.FieldExists(TypeNameProperty) Then
        Dim typeName As String = ReadProperty(TypeNameProperty)
        If Not String.IsNullOrEmpty(typeName) Then
          _objectType = Csla.Reflection.MethodCaller.GetType(typeName, False)
        End If
      End If
      Return _objectType
    End Get
  End Property

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

#If SILVERLIGHT Then

  Private Shared _forceInit As Boolean = False

   ''' <summary>
    ''' Creates an instance of the object. For use by
    ''' MobileFormatter only - you must provide a 
    ''' Type parameter in your code.
    ''' </summary>
    Public Sub new()
      _forceInit = _forceInit andalso false
    End Sub

  ''' <summary>
    ''' Method called by MobileFormatter when an object
    ''' should be deserialized. The data should be
    ''' deserialized from the SerializationInfo parameter.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the serialized data.
    ''' </param>
    ''' <param name="mode">Serialization mode.</param>
    Protected Overrides Sub OnSetState(ByVal info As SerializationInfo,ByVal mode As StateMode)
    {
      _forceInit = _forceInit andalso false
      base.OnSetState(info, mode)
    }
#Else
  ''' <summary>
  ''' Initializes empty CriteriaBase. The type of
  ''' business object to be created by the DataPortal
  ''' MUST be supplied by the subclass.
  ''' </summary>
  Protected Sub New()

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
    TypeName = type.AssemblyQualifiedName
  End Sub

End Class
