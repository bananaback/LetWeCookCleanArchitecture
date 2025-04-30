using LetWeCook.Domain.Common;

namespace LetWeCook.Domain.ValueObjects
{
    public class Name : ValueObject
    {
        public string FirstName { get; } = string.Empty;
        public string LastName { get; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";

        private Name() { } // For EF Core

        public Name(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
        }
    }
}