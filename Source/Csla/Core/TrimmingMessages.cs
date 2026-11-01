//-----------------------------------------------------------------------
// <copyright file="TrimmingMessages.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Messages for trimming and NativeAOT annotations</summary>
//-----------------------------------------------------------------------

using Csla.Reflection;

namespace Csla.Core
{
  /// <summary>
  /// Messages for RequiresUnreferencedCode/RequiresDynamicCode annotations and
  /// justifications for UnconditionalSuppressMessage attributes.
  /// </summary>
  internal static class TrimmingMessages
  {
    #region RequiresUnreferencedCode / RequiresDynamicCode messages

    internal const string BindingList =
      "BindingList<T> raises ListChanged events with PropertyDescriptors, which aren't trim compatible. " +
      "Use the ObservableCollection based types (BusinessListBase, ReadOnlyListBase) in trimmed or NativeAOT applications.";

    internal const string TypeDescriptor =
      "Finds properties by name with TypeDescriptor, which isn't trim compatible.";

    internal const string ArbitraryObjectReflection =
      "Reflects over the members of an object's runtime type, which may have been removed by trimming.";

    internal const string TypeNameLookup =
      "Resolves a type from its name, and the named type may have been removed by trimming.";

    internal const string CaseInsensitiveTypeLookup =
      $"A case-insensitive type lookup can't be analyzed for trimming. Use {nameof(MethodCaller)}.{nameof(MethodCaller.GetType)}(string, bool) instead.";

    internal const string GenericMethodInstantiation =
      "Creates a generic method instantiation from runtime types.";

    internal const string GenericMethodByName =
      "The generic method and its type arguments are resolved by name and may be trimmed.";

    internal const string PropertyTypeGenericInstantiation =
      "Creates a generic method instantiation for the property type at runtime.";

    internal const string ReflectionJsonSerializationDynamicCode =
      "Uses reflection-based System.Text.Json serialization; implement an IMobileSerializer that uses a source-generated JsonSerializerContext for NativeAOT.";

    internal const string ReflectionJsonSerializationUnreferencedCode =
      "Uses reflection-based System.Text.Json serialization; implement an IMobileSerializer that uses a source-generated JsonSerializerContext for trimmed apps.";

    internal const string BindingPathReflection =
      "Walks a binding path by reflecting over the properties of the binding source, which can be any object and may have been trimmed. " +
      "Preserve the types reached through binding paths in trimmed applications.";

    #endregion

    #region UnconditionalSuppressMessage justifications

    internal const string BusinessTypeJustification =
      "The type is a CSLA business type (or a data portal target such as an object factory). The data portal " +
      "(IDataPortal<T>, IChildDataPortal<T>) and the CSLA base classes annotate business types with DynamicallyAccessedMembers.All, " +
      "and object factory types are annotated through IObjectFactoryLoader, so the members reflected over here are preserved. " +
      "Other types used this way must be preserved by the application.";

    internal const string ObjectFactoryTypeNameJustification =
      "Factory names come from ObjectFactoryAttribute. Its Type constructors and the DynamicallyAccessedMembers annotated " +
      "factoryType string of its string constructors make the trimmer preserve the factory type, as long as the name is " +
      "assembly qualified or the type is in the business object's assembly. " +
      "Types resolved from names a custom IObjectFactoryLoader maps itself must be preserved by the application.";

    internal const string FactoryTypeToNameJustification =
      "The name is built from a factory Type annotated with DynamicallyAccessedMembers.All, so the type it names is already preserved.";

    internal const string BaseTypeWalkJustification =
      "Walks the base type chain looking for members. CSLA base classes are annotated with DynamicallyAccessedMembers.All, " +
      "which preserves the members of every type that derives from them.";

    internal const string TypeNameResolutionJustification =
      "Type names come from types CSLA serialized or from ObjectFactory attributes. " +
      "Trimmed applications must preserve their business and factory types (for example with TrimmerRootAssembly).";

    internal const string BindingListMembersJustification =
      "Members are kept for reflection by DynamicallyAccessedMembers, but every instance is created through a constructor " +
      "annotated with RequiresUnreferencedCode, so code that creates the list is already warned.";

    internal const string InstanceOfBindingListJustification =
      "Only called on an instance of a BindingList based type, whose constructors are annotated with RequiresUnreferencedCode.";

    internal const string TypeConverterFallbackJustification =
      "Last-resort conversion, only reached after Convert.ChangeType fails. If trimming removed a type's TypeConverter, " +
      "no converter is found and the original conversion exception is rethrown.";

    internal const string DataAnnotationJustification =
      "The validated instance is a CSLA business object. The business base classes annotate the business type with DynamicallyAccessedMembers.All, " +
      "so the members and attributes ValidationContext reads are preserved.";

    internal const string TaskResultJustification =
      "Result is a public property of Task<T>, preserved by the DynamicDependency on TaskConversionHelper.ConvertTask.";

    internal const string PropertyLoaderGenericMethodJustification =
      "The IManageProperties generic methods place no DynamicallyAccessedMembers requirements on the property type.";

    internal const string InterceptorTypeJustification =
      "Interceptor types are only added through the AddInterceptorProvider<T> overloads and the default list in DataPortalServerOptions, " +
      "both of which preserve public constructors.";

    internal const string LazyPropertiesJustification =
      "The object is the Lazy<T> held in a CSLA rule manager field. The DynamicDependency on the method preserves the public properties of Lazy<T>.";

    internal const string PermissionPolicyJustification =
      "Policy names built by CslaPolicy.GetPolicy or HasPermissionAttribute come from a type whose public constructors are preserved " +
      "by their objectType parameter. A policy name written by hand must name a type the application preserves.";

    internal const string MvcJustification =
      "ASP.NET Core MVC and Razor Pages don't support trimming or NativeAOT, and the SDK warns when an app that uses them is trimmed, " +
      "so this code doesn't run in a trimmed application.";

    #endregion
  }
}
