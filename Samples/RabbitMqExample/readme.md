# RabbitMq Data Portal Channel Example

This example shows the basic use of the RabbitMq data portal channel in `Csla.Channels.RabbitMq`.

To run the example, use the following steps:

1. Install Docker Desktop
2. Run the RabbitMq container
   1. `docker run -d --hostname my-rabbit --name some-rabbit -p 5672:5672 rabbitmq:latest`
3. Open the solution in Visual Studio
4. Set the startup to multiple projects
   1. `RabbitMqExample.Server` and `RabbitMqExample.Client`
   2. Make sure the `RabbitMqExample.Server` is the first project executed
5. Run the solution

The example shows a simple `EditPerson` object retrieved from the app server.

Look at the `Program.cs` file in the service to see how to configure the data portal server to use the RabbitMq channel.

Look at the `Program.cs` file in the client to see how to configure the data portal client to use the RabbitMq proxy.
