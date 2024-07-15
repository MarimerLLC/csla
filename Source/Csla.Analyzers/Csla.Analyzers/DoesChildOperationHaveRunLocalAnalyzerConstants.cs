﻿using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  public static class DoesChildOperationHaveRunLocalAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.DoesChildOperationHaveRunLocal_Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.DoesChildOperationHaveRunLocal_Message), Resources.ResourceManager, typeof(Resources));
  }

  public static class DoesChildOperationHaveRunLocalRemoveAttributeCodeFixConstants
  {
    public static string RemoveRunLocalDescription => Resources.DoesChildOperationHaveRunLocalRemoveAttribute_RemoveRunLocalDescription;
  }
}
