//-----------------------------------------------------------------------
// <copyright file="RabbitMqPortalOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for RabbitMqProxy</summary>
//-----------------------------------------------------------------------

namespace Csla.Channels.RabbitMq;

/// <summary>
/// Options for RabbitMqPortal
/// </summary>
public class RabbitMqPortalOptions
{
  private Uri? _dataPortalUri;

  /// <summary>
  /// Gets or sets the data portal server endpoint URL
  /// </summary>
  /// <exception cref="ArgumentNullException">DataPortalUri</exception>
  /// <exception cref="UriFormatException">Scheme != rabbitmq://</exception>
  /// <exception cref="UriFormatException">Host</exception>
  /// <exception cref="UriFormatException">Port</exception>
  /// <exception cref="UriFormatException">QueueName</exception>
  public Uri? DataPortalUri 
  {
    get => _dataPortalUri;
    set
    {
      if (value == null)
        throw new ArgumentNullException(nameof(DataPortalUri));
      if (value.Scheme != "rabbitmq")
        throw new UriFormatException("Scheme != rabbitmq://");
      if (string.IsNullOrWhiteSpace(value.Host))
        throw new UriFormatException("Host");
      if (value.Port == 0)
        throw new UriFormatException("Port");
      var dataPortalQueueName = value.AbsolutePath[1..];
      if (string.IsNullOrWhiteSpace(dataPortalQueueName))
        throw new UriFormatException("QueueName");
      _dataPortalUri = value;
    }
  }
}
