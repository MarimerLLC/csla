Imports System.ComponentModel
Imports System.Windows.Forms

Namespace Windows

  ''' <summary>
  ''' HostComponentDesigner is an enhanced ComponentDesigner 
  ''' class used for linking a parent container control to a component
  ''' marked with the HostComponentDesigner attribute 
  ''' as a .NET designer service. 
  ''' </summary>
  Public Class HostComponentDesigner
    Inherits System.ComponentModel.Design.ComponentDesigner

#Region "Public Override Methods"

    ''' <summary>
    ''' InitializeNewComponent() overrides the base class InitializeNewComponent() method.  This version of
    ''' InitializeNewComponent() simply runs through the base class InitializeNewComponent functionality then
    ''' if the associated component contains the HostPropertyAttribute attribute the conponent's host property
    ''' is then set to the parent component if the component is a container control.
    ''' </summary>
    ''' <param name="defaultValues">The default values for initialising the new component.</param>
    Public Overrides Sub InitializeNewComponent(ByVal defaultValues As System.Collections.IDictionary)
      Dim attributes As AttributeCollection = Nothing
      Dim propertyName As String = String.Empty
      Dim index As Integer = 0

      ' Run through the default new component initialisation process.
      MyBase.InitializeNewComponent(defaultValues)


      ' Ignore if the parent is not a container.
      If Not (TypeOf ParentComponent Is ContainerControl) Then
        Return
      End If

      ' Retrieve the component's attributes.
      attributes = TypeDescriptor.GetAttributes(Component)

      ' If we have attributes then find our host property attribute.
      If attributes IsNot Nothing Then
        For index = 0 To attributes.Count - 1
          ' If we have a match the fetch the host property name.
          If TypeOf attributes(index) Is HostPropertyAttribute Then
            propertyName = (CType(attributes(index), HostPropertyAttribute)).HostPropertyName
            Exit For
          End If
        Next index
      End If

      ' If there is a host property name then set the host property to be the parent container.
      If (Not String.IsNullOrEmpty(propertyName)) Then
        Component.GetType().GetProperty(propertyName).SetValue(Component, CType(ParentComponent, ContainerControl), Nothing)
      End If
    End Sub

#End Region

  End Class

End Namespace