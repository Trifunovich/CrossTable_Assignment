using System.Buffers;
using System.Globalization;
using System.Text;

//Transforms csv input into md table. See tests project for sample input and output.
namespace CrossTable;

public class Solution {
    record struct Order(string Product, DateTime Date, float Price);

    static readonly string cultureName = "en-US";
    static readonly CultureInfo cultureInfo = new CultureInfo(cultureName);
    private static StringBuilder rowBuilder = new();

    public static void GenerateCrossTable(TextReader reader, TextWriter writer)
    {
        // Read lines with comma-separated values into a list of orders
        var orders = new List<Order>();
        var buffer = ArrayPool<char>.Shared.Rent(1024);
        try
        {
            while (reader.ReadLine() is { } line)
            {
                if (line.Length == 0) continue;
                var pieces = line.Split(',');
                var name = pieces[0];
                var date = DateTime.Parse(pieces[1], cultureInfo);
                var price = float.Parse(pieces[2], cultureInfo);
                orders.Add(new Order(name, date, price));
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }

        // Calculate cross-table summary values
        var sums = new Dictionary<(string Product, int Year), float>();
        foreach (var order in orders)
        {
            var sumKey = (order.Product, order.Date.Year);
            if (sums.ContainsKey(sumKey))
                sums[sumKey] += order.Price;
            else
                sums[sumKey] = order.Price;
        }

        // Calculate ordered lists of years and product names
        var years = sums.Keys.Select(x => x.Year)
            .Distinct()
            .Order()
            .ToArray();

        rowBuilder.Clear();
        GenerateTableLine(null, years.Select(x => x.ToString()));
        GenerateTableLine("-", Enumerable.Repeat("-", years.Length));

        // Produce resulting table

        foreach (var p in sums
                     .Keys
                     .Select(x => x.Product)
                     .Distinct()
                     .Order())
        {
            var newCells = years
                .Select(year => sums.GetValueOrDefault((p, year), 0))
                .Select(x => x == 0 ? string.Empty : x.ToString("c", cultureInfo));
            GenerateTableLine(p, newCells);
        }

        writer.Write(rowBuilder.ToString());
    }

    private static void GenerateTableLine(string? left, IEnumerable<string> values)
    {
        rowBuilder.Append("|");
        rowBuilder.Append(left);
        rowBuilder.Append("|");
        rowBuilder.AppendJoin("|", values);
        rowBuilder.Append("|");
        rowBuilder.Append(Environment.NewLine);
    }


}