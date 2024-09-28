﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LookupDictionaryOptimized
{

    /// <summary>
    /// Based upon nDepend blog : https://blog.ndepend.com/alternate-lookup-for-dictionary-and-hashset-in-net-9/
    /// </summary>
    public static class DemoAlternateLookup
    {
        public static void RunDemo()
        {
            var paul = new Person("Paul", "Jones");
            var joey = new Person("Joey", "Green");
            var laura = new Person("Laura", "Bridges");

            var dict = new Dictionary<Person, int>(PersonComparer.Instance)
            {
                { paul, 11 },
                { joey, 22 },
                { laura, 33 }
            };

            var paulId = paul.Id;
            Dictionary<Person, int>.AlternateLookup<Guid> lookup = dict.GetAlternateLookup<Guid>();
            int lookedUpPersonId = lookup[paulId];

            Console.WriteLine($"Retrieved a Dictionary<Person,Guid> value via alternate lookup key: {paulId}.\nThe looked up value is: {lookedUpPersonId}");
            lookedUpPersonId.Should().Be(11);
            Console.WriteLine($"Expected value retrieved. OK.");
        }

    }
}
