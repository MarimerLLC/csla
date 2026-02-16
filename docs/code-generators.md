# Code Generators

## .NET Code Generator Packages

Starting with CSLA 9, there are code generators available as NuGet packages. These packages are designed to work with the CSLA .NET framework to generate code for your business objects as part of the .NET build process.

* [AutoImplementProperties](https://github.com/MarimerLLC/csla/tree/main/Source/Csla.Generators/cs/AutoImplementProperties) - Uses code generation to radically reduce the amount of code you need to write to declare properties in a business domain class
* [AutoSerialization](https://github.com/MarimerLLC/csla/tree/main/Source/Csla.Generators/cs/AutoSerialization) - Uses code generation to improve serialization performance

Other modern code generator tools also exist:

* [DataPortal Extensions](https://github.com/StefanOssendorf/Csla.DataPortalExtensions) - Generates extension methods for CSLA.NET attributed methods (e.g. Fetch) for type and compile-time safety
* [CslaGeneratorSerialization](https://github.com/JasonBock/CslaGeneratorSerialization) - A custom CSLA serialization formatter that uses C# source generators.

## CSLA .NET Code Generators

There are many traditional project or solution level code generation tools available for CSLA .NET. This is a partial index of the options available, including both tools and templates for those tools.

Only CslaGenFork is affiliated directly with the CSLA .NET project, the others come from various other sources. Some are commercial, some are free, you can decide which is best for you.

* [CslaGenFork](https://github.com/MarimerLLC/CslaGenFork) - Generates CSLA .NET classes suitable for Windows Forms, ASP.NET, and WPF
* [DesiGen](https://desigen-docs.dotnotstandard.com/docs) - Design and code generation tool
* [CodeSmith](https://www.codesmithtools.com/) - General code generation tool
  * Templates: http://www.codesmithtools.com/csla
  * Forums: http://community.codesmithtools.com/Template_Frameworks/f/68.aspx
* [CSLADesignerLibrary3](https://archive.codeplex.com/?p=CSLADesignLibrary3) - Generates CSLA .NET classes
* [CslaDBAGuidance](https://archive.codeplex.com/?p=CslaDBAGuidance) - Generates data access layer using CSLA .NET
* [CslaExtension](https://archive.codeplex.com/?p=t4csla) - Generates CSLA classes from an EF model diagram
* [CSLA Class Diagram and Code](https://marketplace.visualstudio.com/items?itemName=HeinzErnst.CSLAClassDiagramsCode) - Visual Studio extension
* [Sculpture](https://archive.codeplex.com/?p=Sculpture) - Open source code generator for numerous frameworks, including CSLA .NET

If you are aware of a generator or template missing from this list, please let me know and I'll add it.
