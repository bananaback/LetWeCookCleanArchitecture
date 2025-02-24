namespace LetWeCook.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }
        protected Entity() { } // Required by EF Core

        protected Entity(Guid id)
        {
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (Entity)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);
        public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
    }
}