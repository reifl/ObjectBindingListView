using ObjectBindingListView;
using ObjectBindingListView.DataRepresentation;
using ObjectBindingListView.Parsing;
using ObjectBindingListView.Parsing.Tokenizer;
using ObjectBindingListView.Parsing.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<MyDataElement> elements = new List<MyDataElement>();
            var rand = new Random();
            for(int i=0; i<10000000; i++)
            {
                elements.Add(new MyDataElement
                {
                    id = i,
                    Type = i % 2 == 0 ? "Cat" : "Dog",
                    Gender = (Gender)(i % 2),
                    Age = i % 10,
                    SizeInM = (float)rand.NextDouble()
                });
            }
            var sw = new System.Diagnostics.Stopwatch();
            IEnumerable<MyDataElement> x = null;
            List<DslToken> tokens = null;
            DslQueryModel dslModel = null;
            sw.Start();
            
            for (int i = 0; i < 1000; i++)
            {
                var t = new Tokenizer();
                tokens = t.Tokenize("Age > 3 AND Age < 8 OR id=1").ToList();
            }
            sw.Stop();
            Console.WriteLine("Tokenizer Time: " + sw.Elapsed);
            sw.Reset();
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
                var dslParser = new Parser();
                dslModel = dslParser.Parse(tokens);
            }
            sw.Stop();
            Console.WriteLine("DslParser Time: " + sw.Elapsed);
            sw.Reset();
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
                var t = elements.BuildExpression(dslModel.MatchConditions);
                t.Compile();
            }
            sw.Stop();
            Console.WriteLine("BuildExpression: " + sw.Elapsed);
            sw.Reset();
            sw.Start();
            for(int i=0; i<1000; i++)
            {
                x = elements.Where("Age > 3 AND Age < 8 OR (id > 50 AND id < 1000)");
                
            }
            sw.Stop();
            Console.WriteLine("String Filter: " + sw.Elapsed + " found: " + x.Count().ToString());
            sw.Reset();
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
                x = elements.Where(y => y.Age > 3 && y.Age < 8 || (y.id > 50 && y.id < 1000));
            }
            sw.Stop();
            Console.WriteLine("Lambda Filter: " + sw.Elapsed + " found: " + x.Count().ToString());

            Console.ReadLine();
        }
    }
    enum Gender
    {
        male,
        female
    }
    class MyDataElement
    {
        public int id;
        public string Type;
        public Gender Gender;
        public int Age;
        public float SizeInM;
    }
}
