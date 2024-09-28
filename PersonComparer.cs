using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace LookupDictionaryOptimized
{
    internal class PersonComparer : IEqualityComparer<Person>, IAlternateEqualityComparer<Guid, Person>
    {
       
        private PersonComparer()
        {
        }

        public static PersonComparer Instance { get; } = new PersonComparer();

        public Person Create(Guid alternate)
        {
            throw new NotImplementedException(); //not supported in this demo
        }

        public bool Equals(Person? x, Person? y) => x != null && y != null && x.Id == y.Id && x.FirstName == x.FirstName && y.LastName == y.LastName;

        public bool Equals(Guid alternate, Person other) => alternate == other.Id;

        public int GetHashCode([DisallowNull] Person obj) => obj.Id.GetHashCode();

        public int GetHashCode(Guid alternate) => alternate.GetHashCode();
    }
}
