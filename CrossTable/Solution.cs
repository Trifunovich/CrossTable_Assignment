using System.Buffers;
using System.Collections.Generic;
using System.Globalization;

//Transforms csv input into md table. See tests project for sample input and output.
public class Solution {
    record Order(string Product, DateTime Date, float Price);

    static readonly string cultureName = "en-US";
    static readonly CultureInfo cultureInfo = new CultureInfo(cultureName);
    
    public static void GenerateCrossTable(TextReader reader, TextWriter writer) {
        //Read lines with comma-separated values into a list of orders

        var lines = new List<string>();

        while (reader.Peek() >= 0)
        {
            var readCount = reader.ReadLine();
            if (readCount?.Length <= 0) continue;
            lines.Add(readCount);
        }
     
        
        List<Order> orders = new();
        for(int i = 0; i < lines.Count(); i++) {
            string[] pieces = lines[i].Split(",");
            string name = pieces[0];
            DateTime date = DateTime.Parse(pieces[1], cultureInfo);
            float price = float.Parse(pieces[2], cultureInfo);
            orders.Add(new Order(name, date, price));
        }

        //Calculate cross-table summary values
        Dictionary<ValueTuple<string, int>, float> sums = new();
        foreach(Order order in orders) {
            var sumKey = ValueTuple.Create(order.Product, order.Date.Year);
            if(sums.ContainsKey(sumKey))
                sums[sumKey] += order.Price;
            else
                sums[sumKey] = order.Price;
        }

        //Calculate ordered lists of years and product names
        var years = 
            sums.
                Keys.Select(x => x.Item2).Distinct().Order().ToArray();

        var products = sums.
            Keys.Select(x => x.Item1).Order().Distinct().ToArray();

        //Produce resulting table
        string header = GenerateTableLine(null, years.Select(x => x.ToString()));
        string delimeter = GenerateTableLine("-", Enumerable.Repeat("-", years.Length));
        List<string> rows = new();
        foreach(var product in products)
        {
            var newCells = years
                .Select(year =>
                    sums.GetValueOrDefault(ValueTuple.Create(product, year), 0))
                .Select(x => x == 0 ? string.Empty : x.ToString("c", cultureInfo));
            
            string row = GenerateTableLine(product, newCells.ToArray());
            rows.Add(row);
        }
        writer.Write(header + delimeter + string.Concat(rows));
    }
    static string GenerateTableLine(string? left, IEnumerable<string> values) { 
        return $"|{left}|{string.Join("|", values)}|{Environment.NewLine}";
    }


}