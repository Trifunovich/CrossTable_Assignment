using System.Globalization;

//This is a reference solution for benchmarks
public class Solution_Original {
    record Order(string Product, DateTime Date, float Price);

    static readonly string cultureName = "en-US";
    static readonly CultureInfo cultureInfo = new CultureInfo(cultureName);

    public static void GenerateCrossTable(TextReader reader, TextWriter writer) {
        //Read lines with comma-separated values into a list of orders
        string input = reader.ReadToEnd();
        string[] lines = input.Split(Environment.NewLine);
        List<Order> orders = new();
        for(int i = 0; i < lines.Count(); i++) {
            string[] pieces = lines[i].Split(",");
            string name = pieces[0];
            DateTime date = DateTime.Parse(pieces[1], cultureInfo);
            float price = float.Parse(pieces[2], cultureInfo);
            orders.Add(new Order(name, date, price));
        }

        //Calculate cross-table summary values
        Dictionary<Tuple<string, int>, float> sums = new();
        foreach(Order order in orders) {
            var sumKey = Tuple.Create(order.Product, order.Date.Year);
            if(sums.ContainsKey(sumKey))
                sums[sumKey] += order.Price;
            else
                sums[sumKey] = order.Price;
        }

        //Calculate ordered lists of years and product names
        List<int> years = new();
        List<string> products = new();
        foreach(var key in sums.Keys) {
            if(!years.Contains(key.Item2))
                years.Add(key.Item2);
            if(!products.Contains(key.Item1))
                products.Add(key.Item1);
        }
        products.Sort();
        years.Sort();

        //Produce resulting table
        string header = GenerateTableLine(null, years.Cast<object>().ToArray());
        string delimeter = GenerateTableLine("-", Enumerable.Repeat('-', years.Count).Cast<object>().ToArray());
        List<string> rows = new();
        foreach(var product in products) {
            object[] cells = years.Select(year => sums.GetValueOrDefault(Tuple.Create(product, year), 0).ToString("c")).ToArray();
            string row = GenerateTableLine(product, cells);
            rows.Add(row);
        }
        writer.Write(header + delimeter + string.Concat(rows));
    }
    static string GenerateTableLine(object left, object[] values) {
        return $"|{left}|{string.Join("|", values)}|{Environment.NewLine}";
    }
}