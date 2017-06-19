using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ProtoBuf;

namespace Telerex.Core.RTTI
{
  public static class TypeMapper
  {
    #region Fields / Properties

    public static byte[] TypeListSha1;
    private static List<Type> _typeList = new List<Type>();
    private static Dictionary<Type, int> _typeMappings = new Dictionary<Type, int>();
    private static string _typeListString = "";
    private static readonly List<Assembly> _AddedAssemblies = new List<Assembly>(); 

    #endregion

    public static void AddAssembly(Assembly assembly)
    {
      if (_AddedAssemblies.Contains(assembly)) return;
      _AddedAssemblies.Add(assembly);
      foreach (var type in from type in assembly.GetTypes()
        let protoContracts = type.GetCustomAttributes(typeof (ProtoContractAttribute), false)
        where protoContracts.Length >= 1
        select type)
      {
        var constructor = type.GetConstructor(Type.EmptyTypes);
        if (!type.IsValueType && constructor == null)
          throw new Exception("Type [" + type.Name + "] does not provide a parameterless constructor.");
        _typeListString += type.FullName + " ";
        _typeList.Add(type);
        _typeMappings.Add(type, _typeList.Count - 1);
      }
      // Re-compute the Sha1 for the type list
      using (var sha1 = SHA1.Create())
      {
        TypeListSha1 = sha1.ComputeHash(Encoding.ASCII.GetBytes(_typeListString));
      }
    }

    public static void UpdateTypeMap(string[] types)
    {
      var typeStringToTypes = _typeMappings.ToDictionary(pair => pair.Key.FullName, pair => pair.Key);
      types = types.Where(typeStringToTypes.ContainsKey).ToArray();
      _typeMappings = types.Select((s, i) => new {Item = s, Index = i})
        .ToDictionary(s => typeStringToTypes[s.Item], s => s.Index);
      _typeList = types.Select(t => typeStringToTypes[t]).ToList();
      // TODO: Really need to keep a corrections list around for each server connection.
    }

    public static int GetTypeId(Type type) => _typeMappings.GetValueOrDefault(type, -1);

    public static Type GetTypeFromId(int id)
    {
      if (id < 0 || id >= _typeList.Count) return null;
      return _typeList[id];
    }
  }
}