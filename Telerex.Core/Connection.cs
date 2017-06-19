using Telerex.Core.Events;

namespace Telerex.Core
{
  public class Connection
  {
    #region Fields / Properties

    public ConnectionEventStream EventStream;
    public TcpConnection TcpConnection;

    #endregion

    internal Connection(TcpConnection connection)
    {
      TcpConnection = connection;
      EventStream = new ConnectionEventStream(this);
    }
  }
}