Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Text

Namespace Windows

  Public Module BindingSourceHelper

    Private _RootSourceNode As BindingSourceNode

    Public Function InitializeBindingSourceTree(ByVal container As IContainer, ByVal rootSource As BindingSource) _
        As BindingSourceNode

      If rootSource Is Nothing Then
        Throw New ApplicationException("A root binding source has not been provided.")
      End If

      _RootSourceNode = New BindingSourceNode(rootSource)
      _RootSourceNode.Children.AddRange(GetChildBindingSources(container, rootSource, _RootSourceNode))

      Return _RootSourceNode

    End Function

    Private Function GetChildBindingSources(ByVal container As IContainer, ByVal parent As BindingSource, ByVal parentNode As BindingSourceNode) _
        As List(Of BindingSourceNode)

      Dim children As New List(Of BindingSourceNode)()

      For Each component As Component In container.Components
        If TypeOf component Is BindingSource Then
          Dim temp As BindingSource = TryCast(component, BindingSource)
          If temp.DataSource IsNot Nothing AndAlso temp.DataSource.Equals(parent) Then
            Dim childNode As New BindingSourceNode(temp)
            children.Add(childNode)
            childNode.Children.AddRange(GetChildBindingSources(container, temp, childNode))
            childNode.Parent = parentNode
          End If
        End If
      Next

      Return children

    End Function

  End Module

End Namespace
