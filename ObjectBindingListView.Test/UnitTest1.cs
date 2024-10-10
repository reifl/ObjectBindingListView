using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ObjectBindingListView.Test
{
    public class Tests
    {
        private List<TestObject> testObjects;


        [SetUp]
        public void Setup()
        {
            testObjects = new List<TestObject>();

            testObjects.Add(new TestObject
            {
                IntValue = 1,
                FloatValue = 1.0,
                StringValue = "Michael Mustermann",
                EnumValue = TestEnum.Value1,
                DateTimeValue = new DateTime(2000,01,01),
                NullableDateTime = null
            });
            testObjects.Add(new TestObject
            {
                IntValue = 2,
                FloatValue = 5.0,
                StringValue = "Michael Cool",
                EnumValue = TestEnum.Value1,
                DateTimeValue = new DateTime(2000, 01, 01),
                NullableDateTime = null
            });
            testObjects.Add(new TestObject
            {
                IntValue = 1,
                FloatValue = 1.0,
                StringValue = "Michael",
                EnumValue = TestEnum.Value1,
                DateTimeValue = new DateTime(1800, 01, 01),
                NullableDateTime = new DateTime(2000, 10, 10)
            });
            testObjects.Add(new TestObject
            {
                IntValue = 1,
                FloatValue = 15.4,
                StringValue = "Robert",
                EnumValue = TestEnum.Value1,
                DateTimeValue = new DateTime(2000, 01, 01),
                NullableDateTime = new DateTime(2000, 10, 10)
            });
            testObjects.Add(new TestObject
            {
                IntValue = 1,
                FloatValue = 1.0,
                StringValue = "Test",
                EnumValue = TestEnum.Value1,
                DateTimeValue = new DateTime(1990, 01, 01),
                NullableDateTime = new DateTime(2000, 10, 10)
            });
            testObjects.Add(new TestObject
            {
                IntValue = 1,
                FloatValue = 1.0,
                StringValue = "Michael Mustermann",
                EnumValue = TestEnum.Value1,
                DateTimeValue = new DateTime(2000, 01, 01),
                NullableDateTime = new DateTime(1800, 10, 10)
            });
            testObjects.Add(new TestObject
            {
                IntValue = 1,
                FloatValue = 1.0,
                StringValue = "Michael Mustermann",
                EnumValue = TestEnum.Value2,
                DateTimeValue = new DateTime(1950, 01, 01),
                NullableDateTime = new DateTime(1950, 10, 10)
            });
            testObjects.Add(new TestObject
            {
                IntValue = 1,
                FloatValue = 1.0,
                StringValue = "Robert Michael Mustermann",
                EnumValue = TestEnum.Value1,
                DateTimeValue = new DateTime(2000, 01, 01),
                NullableDateTime = null
            });
        }

        [Test]
        public void ContainsMichael()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[StringValue] LIKE '%Michael%'";
            Assert.AreEqual(objList.Count, 6);
        }

        [Test]
        public void ContainsMichaelInsensitive()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[StringValue] LIKE '%michael%'";
            Assert.AreEqual(objList.Count, 6);
        }

        [Test]
        public void EndsWith()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[StringValue] LIKE '%Mustermann'";
            Assert.AreEqual(objList.Count, 4);
        }

        [Test]
        public void EndsWithInsensitive()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[StringValue] LIKE '%mustermann'";
            Assert.AreEqual(objList.Count, 4);
        }

        [Test]
        public void StartsWith()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[StringValue] LIKE 'Robert%'";
            Assert.AreEqual(objList.Count, 2);
        }

        [Test]
        public void StartsWithInsensitive()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[StringValue] LIKE 'robert%'";
            Assert.AreEqual(objList.Count, 2);
        }

        [Test]
        public void NotStartsWithInsensitive()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "NOT [StringValue] LIKE 'robert%'";
            Assert.AreEqual(objList.Count, 6);
        }

        [Test]
        public void IntegerMatching()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[IntValue] = 1";
            Assert.AreEqual(objList.Count, 7);
        }

        [Test]
        public void IntegerGt()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[IntValue] > 1";
            Assert.AreEqual(objList.Count, 1);
        }

        [Test]
        public void IntegerGe()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[IntValue] >= 1";
            Assert.AreEqual(objList.Count, 8);
        }

        [Test]
        public void IntegerLt()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[IntValue] < 2";
            Assert.AreEqual(objList.Count, 7);
        }

        [Test]
        public void IntegerLe()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[IntValue] <= 2";
            Assert.AreEqual(objList.Count, 8);
        }

        [Test]
        public void OrCondition()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[IntValue] = 2 Or [StringValue] = 'Robert'";
            Assert.AreEqual(objList.Count, 2);
        }

        [Test]
        public void AndCondition()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[IntValue] = 1 AND [StringValue] = 'Robert'";
            Assert.AreEqual(objList.Count, 1);
        }

        [Test]
        public void EnumTest()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[EnumValue] = 1";
            Assert.AreEqual(objList.Count, 7);
        }

        [Test]
        public void DateTimeTest()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[DateTimeValue] < #04/04/1900 08:58:43#";
            Assert.AreEqual(objList.Count, 1);
        }

        [Test]
        public void NullableDateTimeTestIsNull()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[NullableDateTime] is null";
            Assert.AreEqual(objList.Count, 3);
        }

        [Test]
        public void NullableDateTimeTestEqNull()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[NullableDateTime] = null";
            Assert.AreEqual(objList.Count, 3);
        }

        [Test]
        public void NullableDateTimeTest()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[NullableDateTime] = #10/10/1950 00:00:00#";
            Assert.AreEqual(objList.Count, 1);
        }

        [Test]
        public void NullableDateTimeTestLessThan()
        {
            var objList = new ObjectBindingListView.ObjectListView<TestObject>();
            objList.DataSource = testObjects;

            objList.Filter = "[NullableDateTime] < #10/10/1950 00:00:00#";
            Assert.AreEqual(objList.Count, 1);
        }
    }


    class TestObject
    {
        public int IntValue { get; set; }
        public double FloatValue { get; set; }

        public string StringValue { get; set; }
        public TestEnum EnumValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public DateTime? NullableDateTime { get; set; }
    }

    enum TestEnum
    {
        Value1 = 1,
        Value2 = 2
    }
}