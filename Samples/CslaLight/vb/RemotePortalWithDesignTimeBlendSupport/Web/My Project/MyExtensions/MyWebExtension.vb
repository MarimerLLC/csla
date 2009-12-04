'INSTANT C# NOTE: Formerly VB.NET project-level imports:

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Xml.Linq
Imports System.Diagnostics
Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.Caching
Imports System.Web.SessionState
Imports System.Web.Security
Imports System.Web.Profile
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

#If VB Then

Namespace Web
	Namespace My
		''' <summary>
		''' Module used to define the properties that are available in the My Namespace for Web projects.
		''' </summary>
		''' <remarks></remarks>

		Friend NotInheritable Class MyWebExtension
			Private Shared s_Computer As ThreadSafeObjectProvider(Of Microsoft.VisualBasic.Devices.ServerComputer) = New ThreadSafeObjectProvider(Of Microsoft.VisualBasic.Devices.ServerComputer)()
			Private Shared s_User As ThreadSafeObjectProvider(Of Microsoft.VisualBasic.ApplicationServices.WebUser) = New ThreadSafeObjectProvider(Of Microsoft.VisualBasic.ApplicationServices.WebUser)()
			Private Shared s_Log As ThreadSafeObjectProvider(Of Microsoft.VisualBasic.Logging.AspLog) = New ThreadSafeObjectProvider(Of Microsoft.VisualBasic.Logging.AspLog)()
			''' <summary>
			''' Returns information about the host computer.
			''' </summary>
			Private Sub New()
			End Sub
			<Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
			Friend Shared ReadOnly Property Computer() As Microsoft.VisualBasic.Devices.ServerComputer
				Get
					Return s_Computer.GetInstance()
				End Get
			End Property
			''' <summary>
			''' Returns information for the current Web user.
			''' </summary>
			<Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
			Friend Shared ReadOnly Property User() As Microsoft.VisualBasic.ApplicationServices.WebUser
				Get
					Return s_User.GetInstance()
				End Get
			End Property
			''' <summary>
			''' Returns Request object.
			''' </summary>
			<Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), Global.System.ComponentModel.Design.HelpKeyword("My.Request")> _
			Friend Shared ReadOnly Property Request() As Global.System.Web.HttpRequest
				<Global.System.Diagnostics.DebuggerHidden()> _
				Get
					Dim CurrentContext As Global.System.Web.HttpContext = Global.System.Web.HttpContext.Current
					If CurrentContext IsNot Nothing Then
						Return CurrentContext.Request
					End If
					Return Nothing
				End Get
			End Property
			''' <summary>
			''' Returns Response object.
			''' </summary>
			<Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), Global.System.ComponentModel.Design.HelpKeyword("My.Response")> _
			Friend Shared ReadOnly Property Response() As Global.System.Web.HttpResponse
				<Global.System.Diagnostics.DebuggerHidden()> _
				Get
					Dim CurrentContext As Global.System.Web.HttpContext = Global.System.Web.HttpContext.Current
					If CurrentContext IsNot Nothing Then
						Return CurrentContext.Response
					End If
					Return Nothing
				End Get
			End Property
			''' <summary>
			''' Returns the Asp log object.
			''' </summary>
			<Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
			Friend Shared ReadOnly Property Log() As Microsoft.VisualBasic.Logging.AspLog
				Get
					Return s_Log.GetInstance()
				End Get
			End Property
		End Class
	End Namespace

End Namespace 'end of root namespace
#End If