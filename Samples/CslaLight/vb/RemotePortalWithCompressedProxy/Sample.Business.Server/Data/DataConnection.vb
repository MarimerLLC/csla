'INSTANT C# NOTE: Formerly VB.NET project-level imports:

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Diagnostics
Imports System.Linq
Imports System.Xml.Linq

Namespace Sample.Business
	Public Class DataConnection
	  Private Sub New()
	  End Sub

	  Public Shared ReadOnly Property ConnectionString() As String
		Get
		  Return System.Configuration.ConfigurationManager.ConnectionStrings("SampleConnectionString").ConnectionString
		End Get
	  End Property
	End Class

End Namespace 'end of root namespace