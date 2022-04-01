using EMCS.Lib;

namespace EMCS.Domain;

public class FeedingEntity
{

}

public class DiaperEntity
{
    public DiaperEntity()
    {
        this.CreatedByUser = string.Empty;
    }
    public virtual DateTime CreatedDate { get; set; }
    public virtual string CreatedByUser { get; set; }
}

public class PumpEntity
{
    public virtual float Duration { get; set; }
}

public class BabyWriteModel : AbstractWriteModel
{
    public BabyWriteModel(long key) : base(key)
    {
    }

    public virtual string Name { get; set; }
    public virtual float Weight { get; set; }
}

public class Baby : Aggregate<BabyWriteModel>
{
    public Baby(BabyWriteModel wm) : base(wm)
    {
    }
}
