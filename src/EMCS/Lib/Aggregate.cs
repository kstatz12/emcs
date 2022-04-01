using NHibernate;

namespace EMCS.Lib;

public interface IApply<TCommand, TEvent>
{
    TEvent Apply(TCommand cmd);
}

public abstract class Aggregate<TWriteModel>
{
    public TWriteModel WriteModel { get; }

    protected Aggregate(TWriteModel wm)
    {
        WriteModel = wm;
    }

    protected void ApplyChange(Action<TWriteModel> fn)
    {
        fn(this.WriteModel);
    }

    public TEvent Accept<TCommand, TEvent>(TCommand cmd)
    {
        if (this is IApply<TCommand, TEvent> app)
        {
            return app.Apply(cmd);
        }
        throw new InvalidOperationException("Cannot Find Apply Interface");
    }
}

public class Behavior<TAggregate, TWriteModel>
    where TWriteModel : AbstractWriteModel
    where TAggregate : Aggregate<TWriteModel>
{
    private readonly long key;
    private readonly IWriteRepository<TWriteModel> writeRepository;
    private readonly AggregateFactory<TAggregate, TWriteModel> aggregateFactory;
    private readonly ISessionFactory sessionFactory;

    public Behavior(
        long key,
        IWriteRepository<TWriteModel> writeRepository,
        AggregateFactory<TAggregate, TWriteModel> aggregateFactory,
        ISessionFactory sessionFactory)
    {
        this.key = key;
        this.writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
        this.aggregateFactory = aggregateFactory;
        this.sessionFactory = sessionFactory;
    }

    public async Task<TEvent> Exec<TCommand, TEvent>(TCommand cmd)
    {
        using var session = this.sessionFactory.OpenSession();
        using var tx = session.BeginTransaction();
        var wm = await this.writeRepository.Get(this.key, session);
        TAggregate agg;
        if (wm is not null)
        {
            agg = this.aggregateFactory.Create.Invoke(wm);
        }
        else
        {
            agg = this.aggregateFactory.CreateWithKey.Invoke(this.key);
        }
        var e = agg.Accept<TCommand, TEvent>(cmd);
        await this.writeRepository.Save(agg.WriteModel, session);
        return e;
    }
}

public class AggregateFactory<TAggregate, TWriteModel>
    where TWriteModel : AbstractWriteModel
{
    public Func<long, TAggregate> CreateWithKey { get; }
    public Func<TWriteModel, TAggregate> Create { get; }

    public AggregateFactory(Func<long, TAggregate> createWithKey, Func<TWriteModel, TAggregate> create)
    {
        CreateWithKey = createWithKey;
        Create = create;
    }
}
