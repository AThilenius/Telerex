using System;
using System.Reactive.Linq;
using Telerex.Core.Events;

namespace Telerex.Core
{
  public static class Extensions
  {
    // TODO: Need to find a way to reduce this memory pressure.
    public static IObservable<DownstreamNetworkEvent<T>> Of<T>(this IObservable<DownstreamNetworkEvent<object>> enumerable) =>
        enumerable.Where(i => (T) i.Payload != null)
                  .Select(i => new DownstreamNetworkEvent<T> {Connection = i.Connection, Payload = (T) i.Payload});
  }
}