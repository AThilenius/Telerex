using ProtoBuf;

namespace Telerex.Core.RTTI
{
  [ProtoContract]
  public struct RttiHandshakeHello
  {
    #region Fields / Properties

    [ProtoMember(1)] public byte[] TypePathMapSha1;

    #endregion
  }

  [ProtoContract]
  public struct RttiHandshakeResponse
  {
    #region Fields / Properties

    [ProtoMember(1)] public string[] TypePathMapCorrections;

    #endregion
  }
}