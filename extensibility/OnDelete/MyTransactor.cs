using Starcounter.Database;
using Starcounter.Database.Hosting.Extensibility;

class MyTransactor : TransactorBase
{
    class MyContext : ContextBase
    {
        public MyContext(IDatabaseContext context) : base(context) { }

        public override void Delete(object obj)
        {
            if (obj is IDeleteAware d)
            {
                d.OnDelete(this);
            }

            base.Delete(obj);
        }
    }

    public MyTransactor(ITransactor transactor) : base(transactor) {}

    protected override IDatabaseContext EnterContext(IDatabaseContext db) => new MyContext(db);
}