using System;
using Starcounter.Database.ChangeTracking;

namespace Starcounter.Database.Hosting.Extensibility
{
    public abstract class ContextBase : IDatabaseContext
    {
        readonly IDatabaseContext _inner;

        protected IDatabaseContext InnerContext { get => _inner; }

        protected ContextBase(IDatabaseContext innerContext) 
            => _inner = innerContext ?? throw new ArgumentNullException(nameof(innerContext));

        public virtual IChangeTracker ChangeTracker => _inner.ChangeTracker;

        public virtual void Delete(object obj) => _inner.Delete(obj);

        public virtual new bool Equals(object objA, object objB) => _inner.Equals(objA, objB);

        public virtual T Get<T>(ulong oid) => _inner.Get<T>(oid);

        public virtual ulong GetOid(object databaseObject) => _inner.GetOid(databaseObject);

        public virtual T Insert<T>() where T : class => _inner.Insert<T>();

        public virtual ISqlResult<T> Sql<T>(string query, params object[] values) => _inner.Sql<T>(query, values);
    }
}
