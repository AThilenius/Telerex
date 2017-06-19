using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Telerex.Core
{
  // State object for reading client data asynchronously  
  public class TcpServer
  {
    #region Fields / Properties

    public IObservable<TcpConnection> NewConnectionsSubject => _newConnectionsSubject;
    private readonly IPEndPoint _ipEndPoint;
    private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();
    private readonly Subject<TcpConnection> _newConnectionsSubject = new Subject<TcpConnection>();
    private TcpListener _tcpListener;

    #endregion

    public TcpServer(IPEndPoint ipEndPoint)
    {
      _ipEndPoint = ipEndPoint;
    }

    public void Start()
    {
      _tcpListener = new TcpListener(_ipEndPoint);
      _tcpListener.Start();
      // Accept new connections.
      Task.Factory.StartNew(new Action(async () =>
      {
        while (!_cancellationToken.IsCancellationRequested)
        {
          var client = await _tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
          var tcpConnection = new TcpConnection(client, _cancellationToken.Token);
          _newConnectionsSubject.OnNext(tcpConnection);
        }
      }));
      // For listening with a raw socket:
      //// Create the socket for listening
      //var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
      //// Bind the listening socket to the port
      //listenSocket.Bind(_ipEndPoint);
      //// Start listening
      //listenSocket.Listen(100);
      //while (!_cancellationToken.IsCancellationRequested)
      //{
      //  var socket = listenSocket.Accept();
      //  var tcpConnection = new TcpConnection(socket, _cancellationToken.Token);
      //}
    }
  }
}