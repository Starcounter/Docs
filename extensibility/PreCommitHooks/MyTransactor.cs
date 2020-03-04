using System;
using Starcounter.Database;
using Starcounter.Database.Hosting.Extensibility;
using Starcounter.Database.ChangeTracking;
using Microsoft.Extensions.Options;
using System.Linq;

class MyTransactor : TransactorBase
{
    class MyContext : ContextBase
    {
        public MyContext(IDatabaseContext context) : base(context) { }

        public void ExecutePreCommitHooks(PreCommitHookOptions options)
        {
            foreach (var change in ChangeTracker.Changes.Where(c => c.ChangeType != ChangeType.Delete))
            {
                var proxy = Get<object>(change.Id);
                var realType = proxy.GetType().BaseType;

                if (options.Delegates.TryGetValue(realType, out Action<IDatabaseContext, Change> action))
                {
                    action(this, change);
                }
            }
        }
    }

    readonly PreCommitHookOptions hookOptions;

    public MyTransactor(ITransactor transactor, IOptions<PreCommitHookOptions> preCommitHookOptions)
        : base(transactor)
        => hookOptions = preCommitHookOptions.Value;

    protected override IDatabaseContext EnterContext(IDatabaseContext db) => new MyContext(db);

    protected override void LeaveContext(IDatabaseContext db)
        => ((MyContext)db).ExecutePreCommitHooks(hookOptions);
}
