# RabbitMq Data Portal Channel

This data portal channel consists of `RabbitMqProxy` on the client and `RabbitMqPortal` to help create a server host.

Messages flow from client to server and back to the client through RabbitMQ queues. Messages contain a "return address", so when a client puts a message on the server's queue and the server processes that message, the server can use the "return address" to put the reply message on the client's queue. This implies that each client and each server will have its own queue. These queues are created by the data portal channel.

The channel requires a functioning RabbitMQ instance in your environment. 

For testing purposes, the simplest way to get such an instance is to use a Docker container, such as the [RabbitMQ docker official image](https://hub.docker.com/_/rabbitmq).

## Creating a Host

To create a host, you need to create a .NET project that can continually listen for inbound messages on the server's queue. The easiest way to do this is often to create a Console Application, though many other options can be implemented.

The host project needs to reference the `Csla` and `Csla.Channels.RabbitMq` NuGet packages.

The host project should define a scoped service for `RabbitMqPortal`, and use that service to create and manage the data portal server "endpoint". You may be able to use the type directly, or you can create a subclass of the type for customization.

```c#
  services.AddCsla(o => o
    .DataPortal(o => o
      .DataPortalServer(o => o
        .UseRabbitMqPortal(o => o.DataPortalUrl = "rabbitmq://host/queue"))));
```

In this example, `host` is the host name of the RabbitMQ service instance, and `queue` is the name of the server queue on which this data portal endpoint should listen. 

You can provide RabbitMQ username and password details on the URI by following standard URI conventions for adding those values.

```
rabbitmq://username:password@host/queue
```

> ℹ️ Note that multiple servers can listen on the same queue name to distribute the server load across multiple servers for load balancing and to achieve fault tolerance.

In your main program code, use the injected service to start the endpoint:

```c#
  rabbitMqPortal.StartListening();
```

The data portal will now process messages arriving on the queue until the app exits.

## Creating a Client

A client app should use the `RabbitMqProxy` like any other data portal proxy. First by configuring it during app startup:

```c#
  services.AddCsla(o => o
    .DataPortal(o => o
      .DataPortalClient(o => o
        .UseRabbitMqProxy(o => o.DataPortalUrl = "rabbitmq://host/queue"))));
```

In this example, `host` is the host name of the RabbitMQ service instance, and `queue` is the name of the server queue to which messages will be sent. 

You can provide RabbitMQ username and password details on the URI by following standard URI conventions for adding those values.

```
rabbitmq://username:password@host/queue
```

By default, the client will create an unnamed queue on which it will listen for replies from the server. You can provide a unique per-client queue name if you desire:

```c#
  services.AddCsla(o => o
    .DataPortal(o => o
      .DataPortalClient(o => o
        .UseRabbitMqProxy(o => o.DataPortalUrl = "rabbitmq://host/queue?reply=[uniqueName]"))));
```

The client app can now use the data portal like normal, and all data portal requests will flow to the server via RabbitMQ messages.
