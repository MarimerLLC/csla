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
        throw new UriFormatException("DataPortalQueueName");
      _dataPortalUri = value;
    }
  }
  
  /// <summary>
  /// Gets or sets the timeout for network
  /// operations in seconds (default is 30 seconds).
  /// </summary>
  public int Timeout { get; set; } = 30;
}
