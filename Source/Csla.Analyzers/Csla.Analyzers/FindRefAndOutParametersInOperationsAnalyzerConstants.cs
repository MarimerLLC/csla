﻿using Csla.Analyzers.Properties;
using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  public static class FindRefAndOutParametersInOperationsAnalyzerConstants
  {
    public static readonly LocalizableResourceString Title = new LocalizableResourceString(nameof(Resources.FindRefAndOutParametersInOperations_Title), Resources.ResourceManager, typeof(Resources));

    public static readonly LocalizableResourceString Message = new LocalizableResourceString(nameof(Resources.FindRefAndOutParametersInOperations_Message), Resources.ResourceManager, typeof(Resources));
  }
}
