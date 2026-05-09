namespace Wex.Purchases.Domain.Entities;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity()
    {
    }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id is required.", nameof(id));
        }

        Id = id;
    }
}
