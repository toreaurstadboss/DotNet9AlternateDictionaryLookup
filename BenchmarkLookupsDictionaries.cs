using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;
using Perfolizer.Horology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LookupDictionaryOptimized
{

    [Config(typeof(FastRunConfig))]
    [MemoryDiagnoser(displayGenColumns: true)]
    [HideColumns("Median", "Job", "RatioSD", "Error", "Gen0", "Alloc Ratio")]
    public class BenchmarkLookupsDictionaries
    {

        [Params(100, 1_000, 10_000)]
        public int Count { get; set; }

        private Dictionary<string, int> _dictionary;

        private string _names = string.Empty;

        
        public BenchmarkLookupsDictionaries()
        {
            _dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) {
                { "Paul", 11 },
                { "John", 22 },
                { "Jack", 33 }
            };
            _names = string.Join(',', _dictionary.Keys);
        }

        [Benchmark(Baseline = true)]
        public int LookupsUsingOnlyStrings()
        {
            int sum = 0;
            for (int i = 0; i < Count; i++)
            {
                foreach (var name in _names.Split(','))
                {
                    string key = name;
                    int val = _dictionary[name];
                    sum += val;
                }
            }
            //Console.WriteLine($"Benchmark of {nameof(LookupsUsingOnlyStrings)} returned a sum equal to: {sum}  (Benchmarks should return the same sum here !)");
            return sum;
        }

        [Benchmark]
        public int LookupsUsingSpansAndStringKeys()
        {
            int sum = 0;
            MemoryExtensions.SpanSplitEnumerator<char> ranges = _names.AsSpan().Split(',');

            for (int i = 0;i < Count; i++)
            {                
                foreach (var range in ranges)
                {
                    //regular lookup of dictionary requires a key to look it up, need to get a key from the SpanSplitEnumerator<string>, a string allocation happens below
                    string key = _names.AsSpan(range).ToString();
                    int val = _dictionary[key];
                    sum += val; 
                }              
            }
            //Console.WriteLine($"Benchmark of {nameof(LookupsUsingSpansAndStringKeys)} returned a sum equal to: {sum}  (Benchmarks should return the same sum here !)");
            return sum;
        }

        [Benchmark]
        public int LookupsUsingAlternateLookupsAndSpans()
        {
            int sum = 0;
            MemoryExtensions.SpanSplitEnumerator<char> ranges = _names.AsSpan().Split(',');
            for (int i = 0; i < Count; i++)
            {
                Dictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> lookup = _dictionary.GetAlternateLookup<ReadOnlySpan<char>>();

                foreach (var range in ranges)
                {
                    ReadOnlySpan<char> key  = _names.AsSpan(range).Trim();
                    int val = lookup[key];
                    sum += val;
                }
            }
            //Console.WriteLine($"Benchmark of {nameof(LookupsUsingAlternateLookupsAndSpans)} returned a sum equal to: {sum}  (Benchmarks should return the same sum here !)");
            return sum;
        }


        private class FastRunConfig : ManualConfig
        {
            public FastRunConfig()
            {
                AddJob(Job.Default
                    .WithIterationTime(TimeInterval.Second) //time for each iteration in milliseconds 
                    .WithLaunchCount(1) //number of launch counts
                    .WithWarmupCount(1) //number of warmup counts 
                    .WithIterationCount(1)); //number of actual iterations
            }
        }


    }
}
