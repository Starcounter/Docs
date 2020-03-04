using Starcounter.Database;

public interface IDeleteAware
{
    void OnDelete(IDatabaseContext db);
}
