using System.Security.Policy;
using ProtoBuf;

namespace Telerex.Console
{
  [ProtoContract]
  public class HelloWorld
  {
    #region Fields / Properties

    [ProtoMember(1)] public string Message;

    #endregion

    public HelloWorld() { }

    public HelloWorld(string message)
    {
      Message = message;
    }

    public override string ToString() => "HelloWorld: " + Message;
  }

  [ProtoContract]
  public class AnotherObject
  {
    #region Fields / Properties

    [ProtoMember(1)] public string SomeString;
    [ProtoMember(2)] public int SomeInt;

    #endregion

    public AnotherObject() { }

    public AnotherObject(string someString, int someInt)
    {
      SomeString = someString;
      SomeInt = someInt;
    }

    public override string ToString() => "AnotherObject: " + SomeInt + " | " + SomeString;
  }
}