using System;
using Starcounter.Database;
using System.Collections.Generic;
using Starcounter.Database.ChangeTracking;

class PreCommitHookOptions
{
    internal IDictionary<Type, Action<IDatabaseContext, Change>> Delegates { get; } 
        = new Dictionary<Type, Action<IDatabaseContext, Change>>();

    public void Hook<T>(Action<IDatabaseContext, Change> action) => Delegates.Add(typeof(T), action);
}