using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Telerex.Core;
using Telerex.Core.Events;

namespace Telerex.Console
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      // Configure Telerex as a server
      // Telerex.HostTcpIp(1234);

      // Configure Telerex as a client (connect to a server)
      // Telerex.ConnectTcp("192.168.1.24", 1234);

      // Send an object to [server | all clients]
      // Telerex.Broadcast<HelloWorld>(new HelloWorld { Message = "Awesome" });
      // Telerex.Connections.MergeEventStreams<HelloWorld>().OnNext(new HelloWorld { Message = "Awesome" });
      // Telerex.Connections.MergeEventStreams<HelloWorld>()
      //                    .Timestamped()
      //                    .Where(hw => !String.IsNullOrBlank(hw.Message))
      //                    .Subscribe(hw => Console.WriteLine(hw.Message));

      // Listen to events from [server | all clients]
      // Telerex.BroadcastStream<HelloWorld>().Subscribe(hw => Console.WriteLine(hw.Message));

      // Send data to specific clients (as the server)
      // Telerex.Clients
      //        .Where(c => c.LastMessageTimestamp < ThreeSecondsAgo)
      //        .Broadcast<HelloWorld>(new HelloWorld { Message = "Awesome" }); // Via overloading of (this TelerexClient[]...)

      // Listen to a stream from a specific client
      // Telerex.Clients
      //        .First()
      //        .Stream<HelloWorld>()
      //        .Subscribe(hw => Console.WriteLine("Message from first client: " + hw.Message));

      // Register closure based response handlers for a client
      // Telerex.ClientConnectedStream
      //        .Subscribe(e =>
      //           e.Client
      //            .Stream<HelloWorld>()
      //            .Subscribe(hw => e.Client.Stream<HelloWorld>().OnNext(new HelloWorld()));

      //var server = new TcpServer(new IPEndPoint(IPAddress.Any, 12345));
      //server.Start();
      //server.NewConnectionsSubject.Subscribe(
      //  newClient =>
      //  {
      //    System.Console.WriteLine("[Server] New connection from " + newClient.TcpClient.Client.RemoteEndPoint);
      //    newClient.Subscribe(
      //      message =>
      //      {
      //        var text = Encoding.ASCII.GetString(message.Data);
      //        System.Console.WriteLine("[Server] Got: " + text);
      //        newClient.OnNext(Encoding.ASCII.GetBytes("Echo: " + text));
      //      });
      //  });
      //Task.Run(async () =>
      //{
      //  var client = await TcpConnection.ConnectTo(new IPEndPoint(IPAddress.Loopback, 12345));
      //  client.Subscribe(message =>
      //  {
      //    System.Console.WriteLine("[Client] Got: " + Encoding.ASCII.GetString(message.Data));
      //  });
      //  while (true)
      //  {
      //    var line = System.Console.ReadLine();
      //    client.OnNext(Encoding.ASCII.GetBytes(line));
      //  }
      //}).GetAwaiter().GetResult();

      Task.Run(async () =>
      {
        TelerexGrandCentral.HostTcp(12345);
        await TelerexGrandCentral.ConnectTcp(new IPEndPoint(IPAddress.Loopback, 12345));
        TelerexGrandCentral.EventStream.Subscribe(downstreamEvent =>
        {
          System.Console.WriteLine((string) downstreamEvent.Payload);
        });
        TelerexGrandCentral.EventStream.OnNext(new UpstreamNetworkEvent<object>
        {
          ConnectionFilter = TelerexGrandCentral.Connections.Take(1),
          Payload = "Hello, world"
        });
      }).GetAwaiter().GetResult();

      System.Console.ReadLine();
    }
  }
}