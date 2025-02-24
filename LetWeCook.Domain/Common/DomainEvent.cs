namespace LetWeCook.Domain.Common
{
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        protected DomainEvent()
        {
            OccurredOn = DateTime.UtcNow;
        }
    }

}