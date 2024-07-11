//-----------------------------------------------------------------------
// <copyright file="RabbitMqPortalFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implement extension methods data portal channel</summary>
//-----------------------------------------------------------------------

using Csla.Server;

namespace Csla.Channels.RabbitMq;

/// <summary>
/// Factory used to get an instance of RabbitMqPortal
/// </summary>
/// <param name="applicationContext"></param>
/// <param name="dataPortal"></param>
/// <param name="rabbitMqPortalOptions"></param>
public class RabbitMqPortalFactory(ApplicationContext applicationContext, IDataPortalServer dataPortal, RabbitMqPortalOptions rabbitMqPortalOptions)
  : IRabbitMqPortalFactory
{
  /// <summary>
  /// Creates an instance of RabbitMqPortal
  /// </summary>
  public RabbitMqPortal CreateRabbitMqPortal()
  {
    return new RabbitMqPortal(applicationContext, dataPortal, rabbitMqPortalOptions);
  }
}
