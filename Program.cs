
using BenchmarkDotNet.Running;
using LookupDictionaryOptimized;

//DemoIterateDictionaryUsingSpans.RunDemo();

//BenchmarkRunner.Run<BenchmarkLookupsDictionaries>();

DemoAlternateLookup.RunDemo();

Console.WriteLine("Hit the any key to continue..");

Console.ReadKey();