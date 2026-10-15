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

    internal const string BindingListMembersJustification =
      "Members are kept for reflection by DynamicallyAccessedMembers, but every instance is created through a constructor " +
      "annotated with RequiresUnreferencedCode, so code that creates the list is already warned.";

    internal const string InstanceOfBindingListJustification =
      "Only called on an instance of a BindingList based type, whose constructors are annotated with RequiresUnreferencedCode.";
  }
}
