using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Telerex.Core.RTTI;

namespace Telerex.Core.Events
{
  public class ConnectionEventStream : IObservable<DownstreamNetworkEvent<object>>, IObserver<UpstreamNetworkEvent<object>>
  {
    private readonly Subject<DownstreamNetworkEvent<object>> _downstreamSubject = new Subject<DownstreamNetworkEvent<object>>();
    private readonly Subject<UpstreamNetworkEvent<object>> _upstreamSubject = new Subject<UpstreamNetworkEvent<object>>();
    private readonly Connection _connection;

    public ConnectionEventStream(Connection connection)
    {
      _connection = connection;
      // Probably a way cooler, more elegant way to do this with Rx
      _connection.TcpConnection.Subscribe(message =>
      {
        var downstramEvent = new DownstreamNetworkEvent<object>
        {
          Connection = _connection,
          Payload = Serializer.DeSerialize(message.Data)
        };
        _downstreamSubject.OnNext(downstramEvent);
        TelerexGrandCentral.EventStream.DownstreamSubject.OnNext(downstramEvent);
      });
    }

    public IDisposable Subscribe(IObserver<DownstreamNetworkEvent<object>> observer)
      => _downstreamSubject.Subscribe(observer);

    public void OnCompleted()
    {
      // Should close the client
      throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
      // Should send the client an exception and close?
      throw new NotImplementedException();
    }

    public void OnNext(UpstreamNetworkEvent<object> value)
    {
      _connection.TcpConnection.OnNext(Serializer.Serialize(value.Payload));
    }
  }
}
