using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Telerex.Core.Events;

namespace Telerex.Core
{
  public struct RawMessage
  {
    #region Fields / Properties

    public byte[] Data;
    public DateTime ReceivedTimestamp;
    public TcpConnection Connection;

    #endregion
  }

  public class TcpConnection : IObservable<RawMessage>, IObserver<byte[]>
  {
    #region Fields / Properties

    public readonly TcpClient TcpClient;
    private readonly Subject<RawMessage> _downstreamSubject = new Subject<RawMessage>();
    private readonly CancellationTokenSource _cancellationTokenSource;
    private bool _isClosed;

    #endregion

    internal TcpConnection(TcpClient tcpClient)
    {
      _cancellationTokenSource = new CancellationTokenSource();
      TcpClient = tcpClient;
      ReadSocket();
    }

    internal TcpConnection(TcpClient tcpClient, CancellationToken parentCancellationToken)
    {
      _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(parentCancellationToken);
      TcpClient = tcpClient;
      ReadSocket();
    }

    public IDisposable Subscribe(IObserver<RawMessage> observer)
    {
      return _downstreamSubject.Subscribe(observer);
    }

    /// <summary>
    ///   Writes data to the TCP socket.
    /// </summary>
    /// <param name="value"></param>
    public void OnNext(byte[] value)
    {
      if (_isClosed) return;
      try
      {
        var stream = TcpClient.GetStream();
        stream.WriteAsync(BitConverter.GetBytes((short) value.Length), 0, 2);
        stream.WriteAsync(value, 0, value.Length);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
        Close();
      }
    }

    /// <summary>
    ///   Closes the TCP socket.
    /// </summary>
    /// <param name="error"></param>
    public void OnError(Exception error)
    {
      Close();
    }

    /// <summary>
    ///   Closes the TCP socket.
    /// </summary>
    public void OnCompleted()
    {
      Close();
    }

    /// <summary>
    ///   Creates and connects a TcpConnection to the given socket.
    /// </summary>
    /// <param name="ipEndPoint">The IP endpoint to connect the new TcpConnection to.</param>
    /// <returns>The new TcpConnection that is already connected.</returns>
    public static async Task<TcpConnection> ConnectTo(IPEndPoint ipEndPoint)
    {
      var tcpClient = new TcpClient();
      await tcpClient.ConnectAsync(ipEndPoint.Address, ipEndPoint.Port);
      if (!tcpClient.Connected) throw new Exception("Failed to connect to " + ipEndPoint);
      return new TcpConnection(tcpClient);
    }

    private async void ReadSocket()
    {
      var stream = TcpClient.GetStream();
      try
      {
        var lengthBuffer = new byte[2];
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
          // Parse the message length
          var lengthReadAmount = await stream.ReadAsync(lengthBuffer, 0, 2, _cancellationTokenSource.Token);
          if (lengthReadAmount == 0) break;
          var length = BitConverter.ToInt16(lengthBuffer, 0);
          // If the message is bigger then 100MB, we probably have corruption.
          if (length > 1 << 8) break;
          var buffer = new byte[length];
          // Read the full message
          for (var i = 0; i < length;)
          {
            var messageReadAmount = await stream.ReadAsync(buffer, i, length - i, _cancellationTokenSource.Token);
            if (messageReadAmount == 0) break;
            i += messageReadAmount;
          }
          _downstreamSubject.OnNext(new RawMessage
          {
            Data = buffer,
            Connection = this,
            ReceivedTimestamp = DateTime.Now
          });
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
      // Client disconnected
      Close();
    }

    private void Close()
    {
      _isClosed = true;
      TcpClient.Close();
      _downstreamSubject.OnCompleted();
    }
  }
}