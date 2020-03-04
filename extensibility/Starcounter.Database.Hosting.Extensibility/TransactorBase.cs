using System;
using System.Threading.Tasks;

namespace Starcounter.Database.Hosting.Extensibility
{
    public abstract class TransactorBase : ITransactor
    {
        readonly ITransactor _inner;

        protected ITransactor InnerTransactor { get => _inner; }
        
        protected TransactorBase(ITransactor innerTransactor) 
            => _inner = innerTransactor ?? throw new ArgumentNullException(nameof(innerTransactor));

        public virtual void Transact(Action<IDatabaseContext> action, TransactOptions options = null) 
            => _inner.Transact(db => ExecuteCallback(db, action), options);

        public virtual T Transact<T>(Func<IDatabaseContext, T> function, TransactOptions options = null) 
            => _inner.Transact(db => ExecuteCallback(db, function), options);

        public virtual Task TransactAsync(Action<IDatabaseContext> action, TransactOptions options = null) 
            => _inner.TransactAsync(db => ExecuteCallback(db, action), options);

        public virtual Task TransactAsync(Func<IDatabaseContext, Task> function, TransactOptions options = null) 
            => _inner.TransactAsync(db => ExecuteCallback(db, function), options);

        public virtual Task<T> TransactAsync<T>(Func<IDatabaseContext, T> function, TransactOptions options = null) 
            => _inner.TransactAsync(db => ExecuteCallback(db, function), options);

        public virtual Task<T> TransactAsync<T>(Func<IDatabaseContext, Task<T>> function, TransactOptions options = null) 
            => _inner.TransactAsync(db => ExecuteCallback(db, function), options);

        public virtual bool TryTransact(Action<IDatabaseContext> action, TransactOptions options = null) 
            => _inner.TryTransact(db => ExecuteCallback(db, action), options);

        protected virtual void ExecuteCallback(IDatabaseContext db, Action<IDatabaseContext> action)
        {
            var context = EnterContext(db);
            try 
            {
                action(context);
            }
            finally
            {
                LeaveContext(context);
            }
        }

        protected virtual T ExecuteCallback<T>(IDatabaseContext db, Func<IDatabaseContext, T> function) 
        {
            var context = EnterContext(db);
            try 
            {
                return function(context);
            }
            finally
            {
                LeaveContext(context);
            }
        }

        protected virtual Task ExecuteCallback(IDatabaseContext db, Func<IDatabaseContext, Task> function)
        {
            var context = EnterContext(db);
            try 
            {
                return function(context);
            }
            finally
            {
                LeaveContext(context);
            }
        }

        protected virtual Task<T> ExecuteCallback<T>(IDatabaseContext db, Func<IDatabaseContext, Task<T>> function)
        {
            var context = EnterContext(db);
            try 
            {
                return function(context);
            }
            finally
            {
                LeaveContext(context);
            }
        }

        /// <summary>
        /// Invoked when a transaction has been created and bound to the current thread,
        /// in a task that execute with a kernel context. The user delegate have not been
        /// invoked yet.  
        /// </summary>
        /// <param name="db">The default database context</param>
        /// <returns>A database context that will be passed to the delegate.</returns>
        protected virtual IDatabaseContext EnterContext(IDatabaseContext db) => db;

        /// <summary>
        /// Invoked right after the user delegate has been executed, but when we are still
        /// within the scope of the transaction and the kernel context.
        /// </summary>
        /// <param name="db">The database context returned by EnterContext.</param>
        protected virtual void LeaveContext(IDatabaseContext db) {}
    }
}