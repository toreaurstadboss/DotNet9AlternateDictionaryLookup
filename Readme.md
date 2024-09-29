
# Alternate lookup for Dictionary in .NET 9 demo

This repo contains code that shows how an alternate lookup of dictionaries 
can be implemented in .NET 9. 

A generic alternate equality comparer is also included. 

The alternate equality comparer looks like this : 

```csharp
using System.Linq.Expressions;

namespace LookupDictionaryOptimized
{
    public class AlternateEqualityComparer<T, TKey> : IEqualityComparer<T>, IAlternateEqualityComparer<TKey, T>
        where T : new()
    {
        private readonly Expression<Func<T, TKey>> _keyAccessor;

        private TKey GetKey(T obj) => _keyAccessor.Compile().Invoke(obj);

        public AlternateEqualityComparer(Expression<Func<T, TKey>> keyAccessor)
        {
            _keyAccessor = keyAccessor;
        }

        public AlternateEqualityComparer<T, TKey> Instance
        {
            get
            {
                return new AlternateEqualityComparer<T, TKey>(_keyAccessor);
            }
        }

        T IAlternateEqualityComparer<TKey, T>.Create(TKey alternate)
        {
            //create a dummy default instance if the requested key is not contained in the dictionary
            return Activator.CreateInstance<T>();
        }

        public bool Equals(T? x, T? y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if ((x == null && y != null) || (x != null && y == null))
            {
                return false;
            }
            TKey xKey = GetKey(x!);
            TKey yKey = GetKey(y!);
            return xKey!.Equals(yKey);
        }

        public int GetHashCode(T obj) => GetKey(obj)?.GetHashCode() ?? default;

        public int GetHashCode(TKey alternate) => alternate?.GetHashCode() ?? default;

        public bool Equals(TKey alternate, T other)
        {
            if (alternate == null && other == null)
            {
                return true;
            }
            if ((alternate == null && other != null) || (alternate != null && other == null))
            {
                return false;
            }
            TKey otherKey = GetKey(other);
            return alternate!.Equals(otherKey);
        }
    }

}

```

You can then define a class and a generic alternate equality comparer like this:


```csharp
    /// <summary>
    /// Based from inspiration of nDepend blog article : https://blog.ndepend.com/alternate-lookup-for-dictionary-and-hashset-in-net-9/
    /// </summary>
    public static class DemoAlternateLookupV2
    {
        public static void RunGenericDemo()
        {
            var paul = new Person("Paul", "Jones");
            var joey = new Person("Joey", "Green");
            var laura = new Person("Laura", "Bridges");

            var mrX = new Person("Mr", "X"); //this object is not added to the dictionary

            AlternateEqualityComparer<Person, Guid> personComparer = new AlternateEqualityComparer<Person, Guid>(m => m.Id);

            var dict = new Dictionary<Person, int>(personComparer.Instance)
            {
                { paul, 11 },
                { joey, 22 },
                { laura, 33 }
            };

            var lauraId = laura.Id;
            //Dictionary<Person, int>.AlternateLookup<Guid> lookup = dict.GetAlternateLookup<Guid>();  Easier : just use var on left hand side

            var lookup = dict.GetAlternateLookup<Guid>();
            int lookedUpPersonId = lookup[lauraId];

            Console.WriteLine($"Retrieved a Dictionary<Person,Guid> value via alternate lookup key: {lauraId}.\nThe looked up value is: {lookedUpPersonId}");
            lookedUpPersonId.Should().Be(33);
            Console.WriteLine($"Expected value retrieved. OK.");

            Console.WriteLine("Testing also to look for a person not contained in the dictionary");

            bool lookedUpNonExistingPersonFound = lookup.ContainsKey(mrX.Id);
            Console.WriteLine($"Retrieved a Dictionary<Person,Guid> value via alternate lookup key: {mrX.Id}.\nThe looked up value found : {lookedUpNonExistingPersonFound}");

        }

    }

```

The generic alternate equality comparer requires a public parameterless constructor. 
Also, the provided keyExpression for the key -  the property of the class which will serve 
as the alternate lookup.

The Person class looks like this :

```csharp

 namespace LookupDictionaryOptimized
{
    public class Person
    {

        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public Person()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Id = Guid.Empty;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}


```


Output below:

```bash

Retrieved a Dictionary<Person,Guid> value via alternate lookup key: 5b2b1d28-c024-4b76-8cdd-2717c42dc7f8.
The looked up value is: 33
Expected value retrieved. OK.
Testing also to look for a person not contained in the dictionary
Retrieved a Dictionary<Person,Guid> value via alternate lookup key: 6ae6f259-14a6-4960-889b-15f33aab4ec0.
The looked up value found : False
Hit the any key to continue..


```

