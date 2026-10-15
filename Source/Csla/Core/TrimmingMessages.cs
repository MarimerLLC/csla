//-----------------------------------------------------------------------
// <copyright file="TrimmingMessages.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Messages for trimming and NativeAOT annotations</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  internal static class TrimmingMessages
  {
    internal const string BindingList =
      "BindingList<T> raises ListChanged events with PropertyDescriptors, which aren't trim compatible. " +
      "Use the ObservableCollection based types (BusinessListBase, ReadOnlyListBase) in trimmed or NativeAOT applications.";

    internal const string TypeDescriptor =
      "Finds properties by name with TypeDescriptor, which isn't trim compatible.";

    internal const string BusinessTypeJustification =
      "The type is a CSLA business type (or a data portal target such as an object factory). The data portal " +
      "(IDataPortal<T>, IChildDataPortal<T>) and the CSLA base classes annotate business types with DynamicallyAccessedMembers.All, " +
      "and object factory types are annotated through IObjectFactoryLoader, so the members reflected over here are preserved. " +
      "Other types used this way must be preserved by the application.";

    internal const string ArbitraryObjectReflection =
      "Reflects over the members of an object's runtime type, which may have been removed by trimming.";

    internal const string BaseTypeWalkJustification =
      "Walks the base type chain looking for members. CSLA base classes are annotated with DynamicallyAccessedMembers.All, " +
      "which preserves the members of every type that derives from them.";

    internal const string BindingListMembersJustification =
      "Members are kept for reflection by DynamicallyAccessedMembers, but every instance is created through a constructor " +
      "annotated with RequiresUnreferencedCode, so code that creates the list is already warned.";

    internal const string InstanceOfBindingListJustification =
      "Only called on an instance of a BindingList based type, whose constructors are annotated with RequiresUnreferencedCode.";
  }
}
