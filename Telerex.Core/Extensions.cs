using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Telerex.Core.Events;

namespace Telerex.Core
{
  public static class Extensions
  {
    // TODO: Need to find a way to reduce this memory pressure.
    public static IObservable<DownstreamNetworkEvent<T>> Of<T>(
      this IObservable<DownstreamNetworkEvent<object>> enumerable) =>
        enumerable.Where(i => i.Payload is T)
          .Select(i => new DownstreamNetworkEvent<T> {Connection = i.Connection, Payload = (T) i.Payload});

    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
      TValue defValue)
    {
      TValue outVal;
      return dictionary.TryGetValue(key, out outVal) ? outVal : defValue;
    }
  }
}