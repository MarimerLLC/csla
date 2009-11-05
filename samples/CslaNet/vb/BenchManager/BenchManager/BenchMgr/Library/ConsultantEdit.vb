Imports System.Data.SqlClient

Namespace Library

  <Serializable()> _
  Public Class ConsultantEdit
    Inherits BusinessBase(Of ConsultantEdit)

#Region " Business Methods "

    Private _id As Integer
    Public ReadOnly Property Id() As Integer
      <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
      Get
        CanReadProperty(True)
        Return _id
      End Get
    End Property

    Private _name As String = ""
    Public Property Name() As String
      <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
      Get
        CanReadProperty(True)
        Return _name
      End Get
      <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
      Set(ByVal value As String)
        CanWriteProperty(True)
        If Not _name.Equals(value) Then
          _name = value
          PropertyHasChanged()
        End If
      End Set
    End Property

    Private _onBench As Boolean
    Public Property OnBench() As Boolean
      <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
      Get
        CanReadProperty(True)
        Return _onBench
      End Get
      <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
      Set(ByVal value As Boolean)
        CanWriteProperty(True)
        If Not _onBench.Equals(value) Then
          _onBench = value
          PropertyHasChanged()
        End If
      End Set
    End Property

    Protected Overrides Function GetIdValue() As Object

      Return _id

    End Function

#End Region

#Region " Business Rules "

    Protected Overrides Sub AddBusinessRules()

      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringRequired, "Name")

    End Sub

#End Region

#Region " Factory Methods "

    Public Shared Function NewConsultant() As ConsultantEdit

      Return DataPortal.Create(Of ConsultantEdit)()

    End Function

    Friend Shared Function GetConsultant(ByVal dr As Csla.Data.SafeDataReader) As ConsultantEdit

      Return New ConsultantEdit(dr)

    End Function

    Private Sub New()

      ' require use of factory methods

    End Sub

    Private Sub New(ByVal dr As Csla.Data.SafeDataReader)
      Me.New()
      Fetch(dr)
    End Sub

#End Region

#Region " Data Access "

    Private Shared _lastId As Integer

    <RunLocal()> _
    Private Overloads Sub DataPortal_Create()

      _id = System.Threading.Interlocked.Decrement(_lastId)
      _onBench = True
      ValidationRules.CheckRules()

    End Sub

    Private Sub Fetch(ByVal dr As Csla.Data.SafeDataReader)

      _id = dr.GetInt32("id")
      _name = dr.GetString("name")
      _onBench = dr.GetBoolean("onBench")
      MarkOld()

    End Sub

    Protected Overrides Sub DataPortal_Insert()

      Using cn As New SqlConnection(Database.BenchMgrConnectionString)
        cn.Open()
        ApplicationContext.LocalContext.Add("cn", cn)
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandType = System.Data.CommandType.Text
          cm.CommandText = "INSERT INTO Consultant (name,onbench) VALUES (@name,@onBench);SELECT @newId=id FROM Consultant WHERE id=SCOPE_IDENTITY()"
          Dim idParam As New SqlParameter("@newId", System.Data.SqlDbType.Int)
          idParam.Direction = System.Data.ParameterDirection.Output
          cm.Parameters.Add(idParam)
          cm.Parameters.AddWithValue("@name", _name)
          cm.Parameters.AddWithValue("@onBench", _onBench)

          cm.ExecuteNonQuery()

          _id = CInt(cm.Parameters("@newId").Value)
        End Using
        If Not OnBench Then
          ClearProjects()
        End If
        MarkOld()
        ApplicationContext.LocalContext.Remove("cn")
      End Using

    End Sub

    Protected Overrides Sub DataPortal_Update()

      Using cn As New SqlConnection(Database.BenchMgrConnectionString)
        cn.Open()
        ApplicationContext.LocalContext.Add("cn", cn)
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandType = System.Data.CommandType.Text
          cm.CommandText = "UPDATE Consultant SET name=@name,onbench=@onBench WHERE id=@id"
          cm.Parameters.AddWithValue("@id", _id)
          cm.Parameters.AddWithValue("@name", _name)
          cm.Parameters.AddWithValue("@onBench", _onBench)
          cm.ExecuteNonQuery()
        End Using
        If Not OnBench Then
          ClearProjects()
        End If
        MarkOld()
        ApplicationContext.LocalContext.Remove("cn")
      End Using

    End Sub

    Protected Overrides Sub DataPortal_DeleteSelf()

      Using cn As New SqlConnection(Database.BenchMgrConnectionString)
        cn.Open()
        ApplicationContext.LocalContext.Add("cn", cn)
        ClearProjects()
        Using cm As SqlCommand = cn.CreateCommand
          cm.CommandType = System.Data.CommandType.Text
          cm.CommandText = "DELETE Consultant WHERE id=@id"
          cm.Parameters.AddWithValue("@id", _id)
          cm.ExecuteNonQuery()
        End Using
        MarkNew()
        ApplicationContext.LocalContext.Remove("cn")
      End Using

    End Sub

    Private Sub ClearProjects()

      Dim cn As SqlConnection = CType(ApplicationContext.LocalContext("cn"), SqlConnection)
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "DELETE ConsultantProject WHERE ConsultantId=@id"
        cm.Parameters.AddWithValue("@id", _id)
        cm.ExecuteNonQuery()
      End Using

    End Sub

#End Region

  End Class

End Namespace
