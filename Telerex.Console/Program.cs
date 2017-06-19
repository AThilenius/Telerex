using System;
using System.Linq;
using System.Net;
using Telerex.Core;

namespace Telerex.Console
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      // Create a server, and a client that connects to it.
      TelerexGrandCentral.HostTcp(12345);
      TelerexGrandCentral.ConnectTcp(new IPEndPoint(IPAddress.Loopback, 12345));

      // Listen for HelloWorld events
      TelerexGrandCentral.EventStream.Of<HelloWorld>()
        .Subscribe(hw => System.Console.WriteLine("Hello World Object: " + hw.Payload));

      // Listen for AnotherObject events
      TelerexGrandCentral.EventStream.Of<AnotherObject>()
        .Subscribe(hw => System.Console.WriteLine("Another Object: " + hw.Payload));

      TelerexGrandCentral.EventStream.OnNext(TelerexGrandCentral.Connections.Take(1), new HelloWorld("Hello, world"));
      TelerexGrandCentral.EventStream.OnNext(TelerexGrandCentral.Connections.Take(1), new HelloWorld("Another hello world."));
      TelerexGrandCentral.EventStream.OnNext(TelerexGrandCentral.Connections.Take(1), new AnotherObject("Another hello world.", 42));

      System.Console.ReadLine();
    }
  }
}