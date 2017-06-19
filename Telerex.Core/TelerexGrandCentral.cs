using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Telerex.Core.Events;
using Telerex.Core.RTTI;

namespace Telerex.Core
{
  /// <summary>
  ///   The main entry point into the TelerexGrandCentral API.
  /// </summary>
  public static class TelerexGrandCentral
  {
    #region Fields / Properties

    public static readonly NetworkEventStream EventStream = new NetworkEventStream();
    public static IEnumerable<Connection> Connections => _telerexConnections;
    internal static readonly object ConnectionsEnumerationLock = new object();
    private static readonly List<Connection> _telerexConnections = new List<Connection>();
    private static TcpServer _server;
    private static bool _isInitialized;

    #endregion

    public static void HostTcp(int port)
    {
      if (!_isInitialized) Initialize(Assembly.GetCallingAssembly());
      HostTcp(new IPEndPoint(IPAddress.Any, port));
    }

    public static void HostTcp(IPEndPoint ipEndPoint)
    {
      if (!_isInitialized) Initialize(Assembly.GetCallingAssembly());
      _server = new TcpServer(ipEndPoint);
      _server.Start();
      _server.NewConnectionsSubject.Subscribe(newTcpConnection =>
      {
        // Convert new connections to a Connection
        var telerexConnection = new Connection(newTcpConnection);
        lock (ConnectionsEnumerationLock) _telerexConnections.Add(telerexConnection);
      });
    }

    public static void ConnectTcp(IPEndPoint ipEndPoint)
      => Task.Run(async () => await ConnectTcpAsync(ipEndPoint)).GetAwaiter().GetResult();

    public static async Task ConnectTcpAsync(IPEndPoint ipEndPoint)
    {
      if (!_isInitialized) Initialize(Assembly.GetCallingAssembly());
      var client = await TcpConnection.ConnectTo(ipEndPoint);
      // Convert the server connection to a Connection
      var telerexConnection = new Connection(client);
      lock (ConnectionsEnumerationLock) _telerexConnections.Add(telerexConnection);
    }

    public static void AddRttiAssembly(Assembly assembly) => TypeMapper.AddAssembly(assembly);

    private static void Initialize(Assembly callingAssembly)
    {
      if (_isInitialized) return;
      _isInitialized = true;
      foreach (var assembly in new[]
      {
        callingAssembly,
        Assembly.GetEntryAssembly(),
        Assembly.GetExecutingAssembly()
      }.Distinct())
      {
        AddRttiAssembly(assembly);
      }
    }
  }
}