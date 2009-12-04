//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

#if VB

namespace Web
{
	namespace My
	{
		/// <summary>
		/// Module used to define the properties that are available in the My Namespace for Web projects.
		/// </summary>
		/// <remarks></remarks>
		
		internal static class MyWebExtension
		{
			private static ThreadSafeObjectProvider<Microsoft.VisualBasic.Devices.ServerComputer> s_Computer = new ThreadSafeObjectProvider<Microsoft.VisualBasic.Devices.ServerComputer>();
			private static ThreadSafeObjectProvider<Microsoft.VisualBasic.ApplicationServices.WebUser> s_User = new ThreadSafeObjectProvider<Microsoft.VisualBasic.ApplicationServices.WebUser>();
			private static ThreadSafeObjectProvider<Microsoft.VisualBasic.Logging.AspLog> s_Log = new ThreadSafeObjectProvider<Microsoft.VisualBasic.Logging.AspLog>();
			/// <summary>
			/// Returns information about the host computer.
			/// </summary>
			[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			internal static Microsoft.VisualBasic.Devices.ServerComputer Computer
			{
				get
				{
					return s_Computer.GetInstance();
				}
			}
			/// <summary>
			/// Returns information for the current Web user.
			/// </summary>
			[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			internal static Microsoft.VisualBasic.ApplicationServices.WebUser User
			{
				get
				{
					return s_User.GetInstance();
				}
			}
			/// <summary>
			/// Returns Request object.
			/// </summary>
			[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), global::System.ComponentModel.Design.HelpKeyword("My.Request")]
			internal static global::System.Web.HttpRequest Request
			{
				[global::System.Diagnostics.DebuggerHidden()]
				get
				{
					global::System.Web.HttpContext CurrentContext = global::System.Web.HttpContext.Current;
					if (CurrentContext != null)
					{
						return CurrentContext.Request;
					}
					return null;
				}
			}
			/// <summary>
			/// Returns Response object.
			/// </summary>
			[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), global::System.ComponentModel.Design.HelpKeyword("My.Response")]
			internal static global::System.Web.HttpResponse Response
			{
				[global::System.Diagnostics.DebuggerHidden()]
				get
				{
					global::System.Web.HttpContext CurrentContext = global::System.Web.HttpContext.Current;
					if (CurrentContext != null)
					{
						return CurrentContext.Response;
					}
					return null;
				}
			}
			/// <summary>
			/// Returns the Asp log object.
			/// </summary>
			[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
			internal static Microsoft.VisualBasic.Logging.AspLog Log
			{
				get
				{
					return s_Log.GetInstance();
				}
			}
		 }
	}


} //end of root namespace

#endif