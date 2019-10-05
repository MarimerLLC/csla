//-----------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("CSLA .NET for Windows (Xaml)")]
[assembly: AssemblyDescription("CSLA .NET Xaml (.NET 4.5)")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Marimer LLC")]
[assembly: AssemblyProduct("CSLA .NET")]
[assembly: AssemblyCopyright("Copyright © 2010-18 Marimer LLC")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Mark the assembly as CLS compliant
[assembly: System.CLSCompliant(true)]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1e436172-6909-4ac1-9cfb-5c034a3c3c06")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("5.1.0.0")]
[assembly: AssemblyFileVersion("5.1.0.0")]


[assembly: ThemeInfo(ResourceDictionaryLocation.SourceAssembly, ResourceDictionaryLocation.SourceAssembly)]

[assembly: XmlnsDefinition("http://schemas.lhotka.net/4.2.0/xaml", "Csla.Xaml")]
[assembly: XmlnsPrefix("http://schemas.lhotka.net/4.2.0/xaml", "csla")]
