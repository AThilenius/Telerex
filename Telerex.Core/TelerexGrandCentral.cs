using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telerex.Core.Events;

namespace Telerex.Core
{
  /// <summary>
  ///   The main entry point into the TelerexGrandCentral API.
  /// </summary>
  public static class TelerexGrandCentral
  {
    #region Fields / Properties

    public static IEnumerable<Connection> Connections => _telerexConnections;
    public static NetworkEventStream EventStream = new NetworkEventStream();
    internal static object ConnectionsEnumerationLock = new object();
    private static readonly List<Connection> _telerexConnections = new List<Connection>();
    private static TcpServer _server;

    #endregion

    public static void HostTcp(int port) => HostTcp(new IPEndPoint(IPAddress.Any, port));

    public static void HostTcp(IPEndPoint ipEndPoint)
    {
      _server = new TcpServer(ipEndPoint);
      _server.Start();
      _server.NewConnectionsSubject.Subscribe(newTcpConnection =>
      {
        // Convert new connections to a Connection
        var telerexConnection = new Connection(newTcpConnection);
        lock (ConnectionsEnumerationLock) _telerexConnections.Add(telerexConnection);
      });
    }

    public static async Task ConnectTcp(IPEndPoint ipEndPoint)
    {
      var client = await TcpConnection.ConnectTo(ipEndPoint);
      // Convert the server connection to a Connection
      var telerexConnection = new Connection(client);
      lock (ConnectionsEnumerationLock) _telerexConnections.Add(telerexConnection);
    }
  }
}