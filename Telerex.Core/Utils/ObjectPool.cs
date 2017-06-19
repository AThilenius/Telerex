using System.Collections.Generic;

namespace Telerex.Core.Utils
{
  public class ObjectPool<T> where T : struct
  {
    #region Fields / Properties

    public static ObjectPool<T> Instance => _instance = _instance ?? new ObjectPool<T>();
    private static ObjectPool<T> _instance;
    private readonly Queue<T> _pool = new Queue<T>();

    #endregion

    public T Get()
    {
      lock (_pool) if (_pool.Count > 0) return _pool.Dequeue();
      return new T();
    }

    public void Return(T item)
    {
      lock (_pool) _pool.Enqueue(item);
    }
  }
}