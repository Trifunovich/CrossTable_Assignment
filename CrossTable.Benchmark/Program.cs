using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Validators;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Columns;

class Program {
    static void Main(string[] args) {
        //var b = new CrossTableBenchmark();
        //b.Setup();
        //b.Sulution();
        BenchmarkRunner.Run<CrossTableBenchmark>();
    }
}


[SimpleJob(RuntimeMoniker.Net90, warmupCount: 2, iterationCount: 3)]
[MeanColumn]
[MemoryDiagnoser]
public class CrossTableBenchmark {
    MemoryStream input;
    [GlobalSetup]
    public void Setup() {
        input = new MemoryStream();
        using StreamWriter writer = new StreamWriter(input, leaveOpen: true);
        const int rows = 40000;
        const int years = 20;
        const int products = 500;
        Random random = new Random(0);
        for(int i = 0; i < rows; i++) {
            writer.Write($"Product{random.Next(products)},01/01/{2021 - random.Next(years)},{random.Next(100)}");
            if(i != rows - 1)
                writer.WriteLine();
        }
    }

    [Benchmark(Baseline = true)]
    public void Solution_Original() => GenerateCrossTable(global::Solution_Original.GenerateCrossTable);

    [Benchmark]
    public void Solution() => GenerateCrossTable(global::CrossTable.Solution.GenerateCrossTable);


    protected void GenerateCrossTable(Action<TextReader, TextWriter> generate) {
        input.Position = 0;
        using TextWriter writer = new StreamWriter(new MemoryStream());
        using TextReader reader = new StreamReader(input, leaveOpen: true);
        generate(reader, writer);
    }
}