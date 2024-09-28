using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LookupDictionaryOptimized
{
    public static class DemoIterateDictionaryUsingSpans
    {

        /// <summary>
        /// Demo from sourcecode from Patrich Smacchia presented on nDepend blog : https://blog.ndepend.com/alternate-lookup-for-dictionary-and-hashset-in-net-9/
        /// </summary>
        /// <returns></returns>
        public static bool RunDemo()
        {
            try
            {
                var dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) {
                { "Paul", 11 },
                { "John", 22 },
                { "Jack", 33 }
            };

                // .NET 9 : GetAlternateLookup()
                Dictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> lookup = dictionary.GetAlternateLookup<ReadOnlySpan<char>>();
                string names = "Paul, John,     Jack";

                MemoryExtensions.SpanSplitEnumerator<char> ranges = names.AsSpan().Split(',');

                foreach (var range in ranges)
                {
                    ReadOnlySpan<char> key = names.AsSpan(range).Trim();
                    int val = lookup[key];
                    Console.WriteLine(val);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }


    }
}
