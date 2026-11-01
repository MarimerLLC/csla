//-----------------------------------------------------------------------
// <copyright file="SessionManagerJsonContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Source-generated JSON metadata for the session state messages</summary>
//-----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using Csla.Blazor.State.Messages;

namespace Csla.Blazor.WebAssembly.State
{
  /// <summary>
  /// Source-generated JSON metadata for the messages <see cref="SessionManager"/>
  /// exchanges with the state controller, so they serialize without reflection
  /// in trimmed and NativeAOT applications. Uses the web defaults that the
  /// HttpClient JSON extensions apply.
  /// </summary>
  [JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
  [JsonSerializable(typeof(StateResult))]
  [JsonSerializable(typeof(byte[]))]
  internal partial class SessionManagerJsonContext : JsonSerializerContext
  {
  }
}
