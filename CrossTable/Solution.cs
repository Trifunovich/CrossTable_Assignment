using System.Buffers;
using System.Collections.Generic;
using System.Globalization;

//Transforms csv input into md table. See tests project for sample input and output.
public class Solution {
    record Order(string Product, DateTime Date, float Price);

    static readonly string cultureName = "en-US";
    static readonly CultureInfo cultureInfo = new CultureInfo(cultureName);
    
public static void GenerateCrossTable(TextReader reader, TextWriter writer)
    {
        // Read lines with comma-separated values into a list of orders
        var orders = new List<Order>();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line.Length == 0) continue;
            var pieces = line.Split(',');
            var name = pieces[0];
            var date = DateTime.Parse(pieces[1], cultureInfo);
            var price = float.Parse(pieces[2], cultureInfo);
            orders.Add(new Order(name, date, price));
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
        var years = sums.Keys.Select(x => x.Year).Distinct().OrderBy(x => x).ToArray();
        var products = sums.Keys.Select(x => x.Product).Distinct().OrderBy(x => x).ToArray();

        // Produce resulting table
        var header = GenerateTableLine(null, years.Select(x => x.ToString()));
        var delimiter = GenerateTableLine("-", Enumerable.Repeat("-", years.Length));
        var rows = new List<string>();
        foreach (var product in products)
        {
            var newCells = years
                .Select(year => sums.GetValueOrDefault((product, year), 0))
                .Select(x => x == 0 ? string.Empty : x.ToString("c", cultureInfo));
            var row = GenerateTableLine(product, newCells.ToArray());
            rows.Add(row);
        }
        writer.Write(header + delimiter + string.Concat(rows));
    }
    static string GenerateTableLine(string? left, IEnumerable<string> values) { 
        return $"|{left}|{string.Join("|", values)}|{Environment.NewLine}";
    }


}