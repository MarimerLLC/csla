Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Configuration

Namespace Rolodex.Business.Data
  Public NotInheritable Class DataConnection
	Private Sub New()
	End Sub
	Public Shared ReadOnly Property ConnectionString() As String
	  Get
		Return System.Configuration.ConfigurationManager.ConnectionStrings("WcfHostWeb.Properties.Settings.RollodexConnectionString").ConnectionString
	  End Get
	End Property
  End Class
End Namespace
