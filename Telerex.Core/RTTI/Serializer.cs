using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProtoBuf;

namespace Telerex.Core.RTTI
{
  [ProtoContract]
  internal struct RttiPayload
  {
    #region Fields / Properties

    [ProtoMember(1)] public short RttiId;
    [ProtoMember(2)] public byte[] Payload;

    #endregion
  }

  public static class Serializer
  {
    public static byte[] Serialize(object obj)
    {
      var id = TypeMapper.GetTypeId(obj.GetType());
      if (id < 0) throw new Exception("Unknown RTTI type");
      var payload = new RttiPayload {RttiId = (short) id, Payload = SerializeToByteArray(obj)};
      return SerializeToByteArray(payload);
    }

    public static object DeSerialize(byte[] data)
    {
      var payload = ProtoBuf.Serializer.Deserialize<RttiPayload>(new MemoryStream(data));
      var type = TypeMapper.GetTypeFromId(payload.RttiId);
      if (type == null) throw new Exception("Unknown RTTI type");
      return ProtoBuf.Serializer.Deserialize(type, new MemoryStream(payload.Payload));
    }

    private static byte[] SerializeToByteArray(object obj)
    {
      var stream = new MemoryStream();
      ProtoBuf.Serializer.Serialize(stream, obj);
      return stream.ToArray();
    }

  }
}