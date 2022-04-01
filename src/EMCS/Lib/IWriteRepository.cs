namespace EMCS.Lib;

public abstract class AbstractWriteModel
{
    public int Id { get; protected set; }
    public long Key { get; set; }

    protected AbstractWriteModel(long key)
    {
        this.Key = key;
    }
}

public interface IWriteRepository<TWriteModel>
    where TWriteModel : AbstractWriteModel
{
    Task<TWriteModel> Get(long key, NHibernate.ISession session);
    Task Save(TWriteModel wm, NHibernate.ISession session);
}

public class WriteRepository<TWriteModel> : IWriteRepository<TWriteModel>
    where TWriteModel : AbstractWriteModel
{
    public async Task<TWriteModel> Get(long key, NHibernate.ISession session)
    {
        return await session.QueryOver<TWriteModel>()
            .Where(x => x.Key == key)
            .SingleOrDefaultAsync();
    }

    public async Task Save(TWriteModel wm, NHibernate.ISession session)
    {
        await session.SaveOrUpdateAsync(wm);
    }
}
