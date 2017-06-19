using System.Collections.Generic;

namespace Telerex.Core.Events
{
  public struct UpstreamNetworkEvent<T>
  {
    #region Fields / Properties

    public IEnumerable<Connection> ConnectionFilter;
    public T Payload;

    #endregion
  }
}