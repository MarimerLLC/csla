Imports System
Imports System.ComponentModel
Imports Csla.Serialization
Imports Csla.Serialization.Mobile

Namespace Core
  ''' <summary> 
  ''' Inherit from this base class to easily 
  ''' create a serializable class. 
  ''' </summary> 
  <Serializable()> _
  Public MustInherit Class MobileObject
    Implements IMobileObject
#Region "Serialize"

    Private Sub GetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter) Implements IMobileObject.GetChildren
      OnGetChildren(info, formatter)
    End Sub

    Private Sub GetState(ByVal info As SerializationInfo) Implements IMobileObject.GetState
      OnGetState(info, StateMode.Serialization)
    End Sub

    ''' <summary> 
    ''' Override this method to insert your field values 
    ''' into the MobileFormatter serialzation stream. 
    ''' </summary> 
    ''' <param name="info"> 
    ''' Object containing the data to serialize. 
    ''' </param> 
    ''' <param name="mode"> 
    ''' The StateMode indicating why this method was invoked. 
    ''' </param> 
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnGetState(ByVal info As SerializationInfo, ByVal mode As StateMode)
    End Sub

    ''' <summary> 
    ''' Override this method to insert your child object 
    ''' references into the MobileFormatter serialzation stream. 
    ''' </summary> 
    ''' <param name="info"> 
    ''' Object containing the data to serialize. 
    ''' </param> 
    ''' <param name="formatter"> 
    ''' Reference to MobileFormatter instance. Use this to 
    ''' convert child references to/from reference id values. 
    ''' </param> 
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnGetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter)
    End Sub

#End Region

#Region "Deserialize"

    Private Sub SetState(ByVal info As SerializationInfo) Implements IMobileObject.SetState
      OnSetState(info, StateMode.Serialization)
    End Sub

    Private Sub SetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter) Implements IMobileObject.SetChildren
      OnSetChildren(info, formatter)
    End Sub

    ''' <summary> 
    ''' Override this method to retrieve your field values 
    ''' from the MobileFormatter serialzation stream. 
    ''' </summary> 
    ''' <param name="info"> 
    ''' Object containing the data to serialize. 
    ''' </param> 
    ''' <param name="mode"> 
    ''' The StateMode indicating why this method was invoked. 
    ''' </param> 
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnSetState(ByVal info As SerializationInfo, ByVal mode As StateMode)
    End Sub

    ''' <summary> 
    ''' Override this method to retrieve your child object 
    ''' references from the MobileFormatter serialzation stream. 
    ''' </summary> 
    ''' <param name="info"> 
    ''' Object containing the data to serialize. 
    ''' </param> 
    ''' <param name="formatter"> 
    ''' Reference to MobileFormatter instance. Use this to 
    ''' convert child references to/from reference id values. 
    ''' </param> 
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub OnSetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter)
    End Sub

#End Region
  End Class
End Namespace