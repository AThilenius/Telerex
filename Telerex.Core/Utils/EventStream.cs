using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Telerex.Core.Utils
{
  public class EventStream
  {
    #region Fields / Properties

    private readonly Dictionary<Type, dynamic> _subjects = new Dictionary<Type, dynamic>();

    #endregion

    public Subject<T> Of<T>()
    {
      var type = typeof (T);
      dynamic subject = null;
      lock (_subjects)
      {
        if (_subjects.TryGetValue(type, out subject)) return subject;
        subject = new Subject<T>();
        _subjects.Add(type, subject);
        return subject;
      }
    }
  }
}