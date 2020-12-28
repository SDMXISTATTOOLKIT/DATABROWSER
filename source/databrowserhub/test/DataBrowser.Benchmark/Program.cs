using System;
using BenchmarkDotNet.Running;
using DataBrowser.Benchmark.JsonStataConvert;

namespace DataBrowser.Benchmark
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<SdmxParserBenchmark>();
            new JsonStatConvert().ParseJson(); //TODO remove comment for debug step by step
            BenchmarkRunner.Run<JsonStatConvert>();
            Console.ReadLine();
        }
    }
}