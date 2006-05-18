Imports System.Collections.Generic
Imports System.Text
Imports System.ComponentModel
Imports System.Reflection
Imports System.IO

Namespace Web.Design

  ''' <summary>
  ''' Loads a Type object into the AppDomain
  ''' from the specified assembly in the most
  ''' current shadow directory used by VS 2005.
  ''' </summary>
  Public Class TypeLoader
    Inherits MarshalByRefObject

    Public Function GetFields(ByVal assemblyName As String, ByVal typeName As String) As List(Of ObjectFieldInfo)

      Dim result As New List(Of ObjectFieldInfo)

      Dim t As Type = TypeLoader.GetType(assemblyName, typeName)
      If GetType(IEnumerable).IsAssignableFrom(t) Then
        ' this is a list so get the item type
        t = Utilities.GetChildItemType(t)
      End If

      Dim props As PropertyDescriptorCollection = _
        TypeDescriptor.GetProperties(t)
      For Each item As PropertyDescriptor In props
        If item.IsBrowsable Then
          result.Add(New ObjectFieldInfo(item))
        End If
      Next
      Return result

    End Function

      ''' <summary>
      ''' Gets a <see cref="System.Type"/> object
      ''' corresponding to the business type specified.
      ''' </summary>
    Private Overloads Shared Function [GetType]( _
      ByVal assemblyName As String, ByVal typeName As String) As Type

      Dim thisAssembly As Assembly = Assembly.GetExecutingAssembly
      Dim assemblyPath As String = GetCodeBase(thisAssembly.CodeBase)

      Dim asm As Assembly = Assembly.LoadFrom(assemblyPath + assemblyName + ".dll")
      Dim result As Type = asm.GetType(typeName, True, True)
      Return result

    End Function

    ''' <summary>
    ''' Determines the most recent shadow directory
    ''' path used by VS 2005 to store the project's
    ''' assemblies.
    ''' </summary>
    ''' <param name="cslaPath">
    ''' Path to the Csla.dll from which the CslaDataSource
    ''' control has been loaded (typically not the latest
    ''' shadow directory).
    ''' </param>
    ''' <returns>
    ''' Directory path for the shadow directory,
    ''' ending in a \ character.
    ''' </returns>
    Private Shared Function GetCodeBase(ByVal cslaPath As String) As String

      If cslaPath.StartsWith("file:///") Then
        cslaPath = cslaPath.Substring(8)
        cslaPath = cslaPath.Replace("/", "\")
      End If
      Dim count As Integer = 0
      Dim endPos As Integer = 1
      For pos As Integer = cslaPath.Length - 1 To 0 Step -1
        If cslaPath.Substring(pos, 1) = "\" Then
          count += 1
          If count = 2 Then
            endPos = pos
            Exit For
          End If
        End If
      Next
      Dim codeBase As String = cslaPath.Substring(0, endPos)

      Dim baseDir As DirectoryInfo = New DirectoryInfo(codeBase)
      Dim result As DirectoryInfo = Nothing
      Dim maxDate As Date = DateTime.MinValue
      For Each dir As DirectoryInfo In baseDir.GetDirectories
        If dir.LastWriteTime > maxDate Then
          maxDate = dir.LastWriteTime
          result = dir
        End If
      Next

      If result IsNot Nothing Then
        Return result.FullName & "\"
      Else
        Return Nothing
      End If
    End Function

  End Class

End Namespace
