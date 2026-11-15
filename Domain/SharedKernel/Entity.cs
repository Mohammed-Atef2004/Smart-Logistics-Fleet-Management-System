namespace Domain.SharedKernel;

public abstract class Entity<TId> : IEquatable<Entity<TId>>, IAuditable, ISoftDeletable
{
    public TId Id { get; protected set; } = default!;

    // Audit / soft-delete state. The setters are *protected* (not private)
    // so derived aggregates manage their own lifecycle state directly
    // instead of shadow-casting duplicate properties over them (CS0108).
    // EF Core materializes these through the backing fields either way.
    public bool IsDeleted { get; protected set; }

    public DateTime? DeletedAtUtc { get; protected set; }

    public DateTime CreatedAt { get; protected set; }

    public DateTime? UpdatedAt { get; protected set; }

    public string? CreatedBy { get; protected set; }

    public string? UpdatedBy { get; protected set; }

    protected Entity() { }

    protected Entity(TId id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
        => obj is Entity<TId> other && EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public bool Equals(Entity<TId>? other)
        => other is not null && EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public override int GetHashCode()
        => Id?.GetHashCode() ?? 0;

    public void Delete()
    {
        DeletedAtUtc = DateTime.UtcNow;
        IsDeleted = true;
    }

    public void SetCreated(string user)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = user;
    }

    public void SetUpdated(string UpdatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        this.UpdatedBy = UpdatedBy;
    }

    public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
        => a is null ? b is null : a.Equals(b);

    public static bool operator !=(Entity<TId>? a, Entity<TId>? b)
        => !(a == b);
}
