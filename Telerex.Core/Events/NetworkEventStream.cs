using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Telerex.Core.Events
{
  /// <summary>
  ///   The global Network event stream. All message events pass through it, from all clients to all clients.
  /// </summary>
  public class NetworkEventStream : IObservable<DownstreamNetworkEvent<object>>, IObserver<UpstreamNetworkEvent<object>>
  {
    internal readonly Subject<DownstreamNetworkEvent<object>> DownstreamSubject = new Subject<DownstreamNetworkEvent<object>>();
    internal readonly Subject<UpstreamNetworkEvent<object>> UpstreamSubject = new Subject<UpstreamNetworkEvent<object>>();

    internal NetworkEventStream()
    {
    }

    public IDisposable Subscribe(IObserver<DownstreamNetworkEvent<object>> observer)
      => DownstreamSubject.Subscribe(observer);

    public void OnCompleted()
    {
      // Does nothing by design
    }

    public void OnError(Exception error)
    {
      // Does nothing design
    }

    public void OnNext(UpstreamNetworkEvent<object> value)
    {
      Connection[] connections = null;
      lock (TelerexGrandCentral.ConnectionsEnumerationLock)
      {
        connections = (value.ConnectionFilter ?? TelerexGrandCentral.Connections).ToArray();
      }
      foreach (var connection in connections) connection.EventStream.OnNext(value);
    }

    public void OnNext<T>(UpstreamNetworkEvent<T> value)
      => OnNext(new UpstreamNetworkEvent<object> {ConnectionFilter = value.ConnectionFilter, Payload = value.Payload});

    public void OnNext<T>(T value)
      => OnNext(new UpstreamNetworkEvent<T> {ConnectionFilter = TelerexGrandCentral.Connections, Payload = value});

    public void OnNext<T>(IEnumerable<Connection> connectionFilter, T value)
      => OnNext(new UpstreamNetworkEvent<T> {ConnectionFilter = connectionFilter, Payload = value});
  }
}