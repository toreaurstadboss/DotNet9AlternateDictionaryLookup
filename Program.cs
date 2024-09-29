
using BenchmarkDotNet.Running;
using LookupDictionaryOptimized;

//DemoIterateDictionaryUsingSpans.RunDemo();

//BenchmarkRunner.Run<BenchmarkLookupsDictionaries>();

//DemoAlternateLookup.RunDemo();

DemoAlternateLookupV2.RunGenericDemo();

Console.WriteLine("Hit the any key to continue..");

Console.Read();