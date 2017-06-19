namespace Telerex.Core.Events
{
  public struct DownstreamNetworkEvent<T>
  {
    #region Fields / Properties

    public Connection Connection;
    public T Payload;

    #endregion
  }
}