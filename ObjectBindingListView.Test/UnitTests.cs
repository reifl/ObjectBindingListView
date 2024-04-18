using NUnit.Framework;
using ObjectBindingListView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace ObjectBindingListView.Test
{
    [TestFixture]
    public class BindingListViewTests
    {
        private ObjectListView<TestItem> _view;
        private List<TestItem> _data;

        [SetUp]
        public void SetUp()
        {
            _data = new List<TestItem>
        {
            new TestItem { Name = "John", Age = 30, Height = 175.5, BirthDate = new DateTime(1990, 1, 1), IsActive = true, Role = UserRole.Admin },
            new TestItem { Name = "Alice", Age = 25, Height = 160.3, BirthDate = new DateTime(1995, 5, 5), IsActive = false, Role = UserRole.User },
            new TestItem { Name = "Bob", Age = 40, Height = 180.7, BirthDate = new DateTime(1980, 10, 10), IsActive = true, Role = UserRole.User },
            new TestItem { Name = "Jane", Age = 35, Height = 168.0, BirthDate = new DateTime(1985, 3, 15), IsActive = false, Role = UserRole.Admin }
        };

            _view = new ObjectListView<TestItem>();
            _view.DataSource = _data;
        }

        [Test]
        public void Filter_StringContains()
        {
            _view.Filter = "[Name] LIKE '%o%'";

            Assert.AreEqual(2, _view.Count);
            Assert.IsTrue(_view.Cast<TestItem>().All(x => x.Name.Contains("o")));
        }

        [Test]
        public void Filter_IntegerEquals()
        {
            _view.Filter = "[Age] = 30";

            Assert.AreEqual(1, _view.Count);
            Assert.AreEqual("John", _view[0].Name);
        }

        [Test]
        public void Filter_FloatGreaterThan()
        {
            _view.Filter = "[Height] > 170";

            Assert.AreEqual(2, _view.Count);
            Assert.IsTrue(_view.Cast<TestItem>().All(x => x.Height > 170));
        }

        [Test]
        public void Filter_DateTimeLessThanOrEqual()
        {
            _view.Filter = "[BirthDate] <= #1990-01-01#";

            Assert.AreEqual(3, _view.Count);
            Assert.IsTrue(_view.Cast<TestItem>().All(x => x.BirthDate <= new DateTime(1990, 1, 1)));
        }

        [Test]
        public void Filter_BooleanTrue()
        {
            _view.Filter = "[IsActive] = true";

            Assert.AreEqual(2, _view.Count);
            Assert.IsTrue(_view.Cast<TestItem>().All(x => x.IsActive));
        }

        [Test]
        public void Filter_EnumEquals()
        {
            _view.Filter = "[Role] = 'Admin'";

            Assert.AreEqual(2, _view.Count);
            Assert.IsTrue(_view.Cast<TestItem>().All(x => x.Role == UserRole.Admin));
        }

        [Test]
        public void AddNewTest()
        {
            var newList = new ObjectListView<TestItem>();
            var dataSource = new List<TestItem>();
            newList.DataSource = dataSource;
            var prevCount = newList.Count;
            newList.AddNew();
            Assert.IsTrue(prevCount + 1 == newList.Count);
        }

        public class TestItem
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public double Height { get; set; }
            public DateTime BirthDate { get; set; }
            public bool IsActive { get; set; }
            public UserRole Role { get; set; }
        }

        public enum UserRole
        {
            Admin,
            User
        }
    }
}