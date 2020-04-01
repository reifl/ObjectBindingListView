using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectBoundBindingList.DataRepresentation;
using ObjectBoundBindingList.LinqExtension;
using ObjectBoundBindingList.Parser;
using ObjectBoundBindingList.Tokenizer;

namespace TokenizerTest
{
    [TestClass]
    public class TokenizerTest
    {
        static string[] GetCode()
        {
            return new string[]
            {
        @"using System;
            //using static TokenizerTest.TokenizerTest;
 
        namespace DynamicNS
        {
            public static class DynamicCode
            {
                public static void DynamicMethod()
                {
                    //var x = new testClass();
                    Console.WriteLine(""Hello, world!"");
                }
            }
        }"
            };
        }

        [TestMethod]
        public void Tokenize()
        {
            

            var tokenizer = new Tokenizer();
            var x = tokenizer.Tokenize("x123 BETWEEN 0 AND 15");
            Assert.IsTrue(true);
        }

        /*[TestMethod]
        public void Tokenize2()
        {
            var tokenizer = new Tokenizer();
            var x = tokenizer.Tokenize("x123 LIKE '%test 12344 \\'blub\\''");
            Assert.IsTrue(true);
        }


        [TestMethod]
        public void fff()
        {
            IList<testClass> list = new List<testClass>();
            list.Add(new testClass
            {
                x123 = "blub",
                xf = 1
            });
            list.Add(new testClass
            {
                x123 = "blub1",
                xf = 2
            });
            list.Add(new testClass
            {
                x123 = "blub2",
                xf = 3
            });
            list.Add(new testClass
            {
                x123 = "blub3",
                xf = 4
            });

            var x = list.Where("NOT xf = 1");
            var y = x.ToList();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Tokenize3()
        {
            var tokenizer = new Tokenizer();
            var x = tokenizer.Tokenize("(xf <= 3 OR xf > 4 AND (xf < 3 OR xf = 5))");
            var y = new DslParser();
            var dr = y.Parse(x);
            Assert.IsTrue(true);
        }*/

        private object GetPropertyValue(string value, object t)
        {
            var type = t.GetType();
            var properties = type.GetProperties();
            var prop = properties.Where(x => x.Name.ToLower() == value.ToLower()).FirstOrDefault();
            if (prop == null)
                throw new ArgumentOutOfRangeException("Cannot find " + value + " Property.");
            return prop.GetValue(t);
        }

        public class testClass
        {
            public testClass()
            {
                System.Diagnostics.Debugger.Break();
            }
            public string x123 { get; set; }
            public int xf { get; set; }
        }
    }
}
