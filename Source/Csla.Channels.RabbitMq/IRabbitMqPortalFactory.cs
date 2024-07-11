//-----------------------------------------------------------------------
// <copyright file="IRabbitMqPortalFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods data portal channel</summary>
//-----------------------------------------------------------------------

namespace Csla.Channels.RabbitMq;

/// <summary>
/// Factory used to get an instance of RabbitMqPortal
/// </summary>
public interface IRabbitMqPortalFactory
{
  /// <summary>
  /// Creates an instance of RabbitMqPortal
  /// </summary>
  RabbitMqPortal CreateRabbitMqPortal();
}
