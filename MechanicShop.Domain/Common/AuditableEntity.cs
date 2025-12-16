namespace MechanicShop.Domain.Common
{
    // Base class for entities that need audit information, inherits from Entity
    public abstract class AuditableEntity : Entity
    {
        public AuditableEntity() { }

        // Constructor that allows setting the Id and passes it to the base Entity class
        public AuditableEntity(Guid id) : base(id)
        { }

        // The UTC timestamp when the entity was created
        public DateTimeOffset CreatedAtUtc { get; set; }

        // The identifier (e.g., username or userId) of who created the entity
        public string? CreatedBy { get; set; }

        // The UTC timestamp when the entity was last modified
        public DateTimeOffset LastModifiedAtUtc { get; set; }

        // The identifier (e.g., username or userId) of who last modified the entity
        public string? LastModifiedBy { get; set; }
    }
}
