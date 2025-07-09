//-----------------------------------------------------------------------
// <copyright file="StateResult.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Message type for communication between StateController</summary>
//-----------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace Csla.Blazor.State.Messages
{
  /// <summary>
  /// Message type for communication between StateController
  /// and the Blazor wasm client.
  /// </summary>
  public class StateResult
  {
    /// <summary>
    /// Gets or sets the result status of the message.
    /// </summary>
    public ResultStatuses ResultStatus { get; set; }
    /// <summary>
    /// Gets or sets the serialized session data.
    /// </summary>
    public byte[]? SessionData { get; set; }

    /// <summary>
    /// Gets a value that indicates if <see cref="ResultStatus"/> is <see cref="ResultStatuses.Success"/>.
    /// </summary>
    [MemberNotNullWhen(true, nameof(SessionData))]
    public bool IsSuccess => ResultStatus == ResultStatuses.Success;
  }
}