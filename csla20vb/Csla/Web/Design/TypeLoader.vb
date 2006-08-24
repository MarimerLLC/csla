Imports System.Collections.Generic
Imports System.Text
Imports System.ComponentModel
Imports System.Reflection
Imports System.IO
Imports System.Web.UI
Imports System.Web.UI.Design

Namespace Web.Design

  ''' <summary>
  ''' Loads a Type object into the AppDomain
  ''' from the specified assembly in the most
  ''' current shadow directory used by VS 2005.
  ''' </summary>
  Public Class TypeLoader
    Inherits MarshalByRefObject

#Region "static methods for primary AppDomain"

    ''' <summary>
    ''' Gets a list of
    ''' <see cref="ObjectFieldInfo"/> describing
    ''' the most recent version of the specified
    ''' assembly and class.
    ''' </summary>
    ''' <param name="assemblyName">Name of the assembly</param>
    ''' <param name="typeName">Name of the type</param>
    ''' <returns></returns>
    Public Shared Function GetFields(ByVal assemblyName As String, ByVal typeName As String) As IDataSourceFieldSchema()

      Dim result As List(Of ObjectFieldInfo) = New List(Of ObjectFieldInfo)()

      Dim originalPath As String = GetOriginalPath(assemblyName, typeName)

      Dim tempDomain As AppDomain = GetTemporaryAppDomain()
      Try
        result = GetTypeLoader(tempDomain).GetFields(originalPath, assemblyName, typeName)
      Finally
        AppDomain.Unload(tempDomain)
      End Try
      Return result.ToArray()

    End Function

    ''' <summary>
    ''' Get a value indicating whether data binding can directly
    ''' delete the object.
    ''' </summary>
    ''' <param name="assemblyName">Name of the assembly</param>
    ''' <param name="typeName">Name of the type</param>
    Public Shared Function CanDelete(ByVal assemblyName As String, ByVal typeName As String) As Boolean

      Dim result As Boolean

      Dim originalPath As String = GetOriginalPath(assemblyName, typeName)

      Dim tempDomain As AppDomain = GetTemporaryAppDomain()
      Try
        result = GetTypeLoader(tempDomain).CanDelete(originalPath, assemblyName, typeName)
      Finally
        AppDomain.Unload(tempDomain)
      End Try
      Return result

    End Function

    ''' <summary>
    ''' Get a value indicating whether data binding can directly
    ''' insert an instance of the object.
    ''' </summary>
    ''' <param name="assemblyName">Name of the assembly</param>
    ''' <param name="typeName">Name of the type</param>
    Public Shared Function CanInsert(ByVal assemblyName As String, ByVal typeName As String) As Boolean

      Dim result As Boolean

      Dim originalPath As String = GetOriginalPath(assemblyName, typeName)

      Dim tempDomain As AppDomain = GetTemporaryAppDomain()
      Try
        result = GetTypeLoader(tempDomain).CanInsert(originalPath, assemblyName, typeName)
      Finally
        AppDomain.Unload(tempDomain)
      End Try
      Return result

    End Function

    ''' <summary>
    ''' Get a value indicating whether data binding can directly
    ''' update or edit the object.
    ''' </summary>
    ''' <param name="assemblyName">Name of the assembly</param>
    ''' <param name="typeName">Name of the type</param>
    Public Shared Function CanUpdate(ByVal assemblyName As String, ByVal typeName As String) As Boolean

      Dim result As Boolean

      Dim originalPath As String = GetOriginalPath(assemblyName, typeName)

      Dim tempDomain As AppDomain = GetTemporaryAppDomain()
      Try
        result = GetTypeLoader(tempDomain).CanUpdate(originalPath, assemblyName, typeName)
      Finally
        AppDomain.Unload(tempDomain)
      End Try
      Return result

    End Function

    Private Shared Function GetTypeLoader(ByVal tempDomain As AppDomain) As TypeLoader

      ' load the TypeLoader object in the temp AppDomain
      Dim thisAssembly As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
      Dim loader As TypeLoader = CType(tempDomain.CreateInstanceFromAndUnwrap(thisAssembly.CodeBase, GetType(TypeLoader).FullName), TypeLoader)
      Return loader

    End Function

    Private Shared Function GetTemporaryAppDomain() As AppDomain

      Dim fulltrust As System.Security.NamedPermissionSet = New System.Security.NamedPermissionSet("FullTrust")
      Dim tempDomain As AppDomain = AppDomain.CreateDomain("__CslaDataSource__temp", AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation, fulltrust, New System.Security.Policy.StrongName() {})
      Return tempDomain

    End Function

    Private Shared Function GetOriginalPath(ByVal assemblyName As String, ByVal typeName As String) As String

      Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.Load(assemblyName)
      Return asm.CodeBase

    End Function

#End Region

#Region "Implementation for temporary AppDomain"

    ''' <summary>
    ''' Gets a list of
    ''' <see cref="ObjectFieldInfo"/> describing
    ''' the most recent version of the specified
    ''' assembly and class.
    ''' </summary>
    ''' <param name="originalPath">Path to the assembly
    ''' as determined by Visual Studio</param>
    ''' <param name="assemblyName">Name of the assembly</param>
    ''' <param name="typeName">Name of the type</param>
    ''' <returns></returns>
    Public Function GetFields(ByVal originalPath As String, ByVal assemblyName As String, ByVal typeName As String) As List(Of ObjectFieldInfo)

      Dim result As List(Of ObjectFieldInfo) = New List(Of ObjectFieldInfo)()

      Dim t As Type = TypeLoader.GetType(originalPath, assemblyName, typeName)
      If GetType(IEnumerable).IsAssignableFrom(t) Then
        ' this is a list so get the item type
        t = Utilities.GetChildItemType(t)
      End If
      Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(t)
      For Each item As PropertyDescriptor In props
        If item.IsBrowsable Then
          result.Add(New ObjectFieldInfo(item))
        End If
      Next item

      Return result
    End Function

    ''' <summary>
    ''' Get a value indicating whether data binding can directly
    ''' delete the object.
    ''' </summary>
    ''' <param name="originalPath">Path to the assembly
    ''' as determined by Visual Studio</param>
    ''' <param name="assemblyName">Name of the assembly</param>
    ''' <param name="typeName">Name of the type</param>
    Public Function CanDelete(ByVal originalPath As String, ByVal assemblyName As String, ByVal typeName As String) As Boolean

      Dim objectType As Type = TypeLoader.GetType(originalPath, assemblyName, typeName)
      If GetType(Csla.Core.IUndoableObject).IsAssignableFrom(objectType) Then
        Return True
      ElseIf Not objectType.GetMethod("Remove") Is Nothing Then
        Return True
      Else
        Return False
      End If

    End Function

    ''' <summary>
    ''' Get a value indicating whether data binding can directly
    ''' insert an instance of the object.
    ''' </summary>
    ''' <param name="originalPath">Path to the assembly
    ''' as determined by Visual Studio</param>
    ''' <param name="assemblyName">Name of the assembly</param>
    ''' <param name="typeName">Name of the type</param>
    Public Function CanInsert(ByVal originalPath As String, ByVal assemblyName As String, ByVal typeName As String) As Boolean

      Dim objectType As Type = TypeLoader.GetType(originalPath, assemblyName, typeName)
      If GetType(Csla.Core.IUndoableObject).IsAssignableFrom(objectType) Then
        Return True
      Else
        Return False
      End If

    End Function

    ''' <summary>
    ''' Get a value indicating whether data binding can directly
    ''' update or edit the object.
    ''' </summary>
    ''' <param name="originalPath">Path to the assembly
    ''' as determined by Visual Studio</param>
    ''' <param name="assemblyName">Name of the assembly</param>
    ''' <param name="typeName">Name of the type</param>
    Public Function CanUpdate(ByVal originalPath As String, ByVal assemblyName As String, ByVal typeName As String) As Boolean

      Dim objectType As Type = TypeLoader.GetType(originalPath, assemblyName, typeName)
      If GetType(Csla.Core.IUndoableObject).IsAssignableFrom(objectType) Then
        Return True
      Else
        Return False
      End If

    End Function

    ''' <summary>
    ''' Gets a <see cref="System.Type"/> object
    ''' corresponding to the business type specified.
    ''' </summary>
    Private Overloads Shared Function [GetType](ByVal originalPath As String, ByVal assemblyName As String, ByVal typeName As String) As Type

      Dim assemblyPath As String = GetCodeBase(originalPath)

      Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.LoadFrom(assemblyPath & assemblyName & ".dll")
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
      Dim [end] As Integer = 1
      For pos As Integer = cslaPath.Length - 1 To 1 Step -1
        If cslaPath.Substring(pos, 1) = "\" Then
          count += 1
          If count = 2 Then
            [end] = pos
            Exit For
          End If
        End If
      Next pos
      Dim codeBase As String = cslaPath.Substring(0, [end])

      Dim baseDir As DirectoryInfo = New DirectoryInfo(codeBase)
      Dim result As DirectoryInfo = Nothing
      Dim maxDate As DateTime = DateTime.MinValue
      For Each dir As DirectoryInfo In baseDir.GetDirectories()
        If dir.LastWriteTime > maxDate Then
          maxDate = dir.LastWriteTime
          result = dir
        End If
      Next dir

      If Not result Is Nothing Then
        Return result.FullName & "\"
      Else
        Return Nothing
      End If

    End Function

#End Region
  End Class

End Namespace
