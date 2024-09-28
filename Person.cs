namespace LookupDictionaryOptimized
{
    public class Person
    {

        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;            
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
