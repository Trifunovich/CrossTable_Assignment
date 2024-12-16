using NUnit.Framework;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace CrossTable.Tests {
    [SetCulture("en-US")]
    [TestFixture]
    public class Tests {
        [Test]
        public void Test2x2() {
            AssertCrossTable(
@"Bananas,09/12/2021,20
Apples,08/08/2020,10
Bananas,07/25/2021,30
Apples,05/07/2020,10
Apples,01/02/2021,60
Bananas,05/11/2020,40
Bananas,02/22/2021,55
Apples,02/04/2020,12",

@"||2020|2021|
|-|-|-|
|Apples|$32.00|$60.00|
|Bananas|$40.00|$105.00|
");
        }

        [Test]
        public void Test1x1() {
            AssertCrossTable(
@"Bananas,09/12/2021,20.17
Bananas,07/25/2021,30.22",

@"||2021|
|-|-|
|Bananas|$50.39|
");
        }

        [Test]
        public void Culture() {
            CultureInfo culture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("es-ES");
            try {
                AssertCrossTable(
@"Bananas,09/12/2021,20.17",

@"||2021|
|-|-|
|Bananas|$20.17|
");
            } finally {
                CultureInfo.CurrentCulture = culture;
            }        
        }

        [Test]
        public void EmptyCell() {
            AssertCrossTable(
@"Bananas,09/12/2020,20
Apples,08/08/2021,10
Apples,05/07/2020,10",

@"||2020|2021|
|-|-|-|
|Apples|$10.00|$10.00|
|Bananas|$20.00||
");
        }

//        [Test]
//        public void JsonGenerator() {
//            AssertCrossTable((reader, writer) => {
//                var generator = new JsonGenerator();
//                generator.GenerateTable(reader, writer);
//            },

//@"Bananas,01/05/2021,10.0
//Apples,02/09/2021,20.0
//Apples,01/03/2021,11.5
//Apples,07/04/2021,3
//Bananas,02/02/2020,5.0",

//@"{""Apples"":{""2021"":34.5},""Bananas"":{""2020"":5,""2021"":10}}"
//            );

//            AssertCrossTable((reader, writer) => {
//                var generator = new MDGenerator();
//                generator.GenerateTable(reader, writer);
//            },

//@"Bananas,01/05/2021,10.0
//Apples,02/09/2021,20.0
//Apples,01/03/2021,11.5
//Apples,07/04/2021,3
//Bananas,02/02/2020,5.0",

//@"||2020|2021|
//|-|-|-|
//|Apples||$34.50|
//|Bananas|$5.00|$10.00|
//"
//);
//        }

        [DebuggerStepThrough]
        protected virtual void AssertCrossTable(string input, string output) {
            //AssertCrossTable(Solution_Original.GenerateCrossTable, input, output);
            AssertCrossTable(Solution.GenerateCrossTable, input, output);
        }
        [DebuggerStepThrough]
        protected void AssertCrossTable(Action<TextReader, TextWriter> generate, string input, string output) {
            using var reader = new StringReader(input);
            var builder = new StringBuilder();
            using var writer = new StringWriter(builder);
            generate(reader, writer);
            string result = builder.ToString();
            Assert.AreEqual(output.Replace("\r\n", Environment.NewLine), result);
        }
    }
}