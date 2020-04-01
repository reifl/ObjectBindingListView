using ObjectBindingListView;
using ObjectBindingListView.Parsing.Tokenizer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestProjekt
{
    public partial class Form1 : Form
    {
        private IList<testClass> liste;
        public Form1()
        {
            InitializeComponent();
            liste = new List<testClass>();
            liste.Add(new testClass
            {
                x123 = "blub\'test",
                xf = 1,
                t = 1.5f,
                test = null
            });
            liste.Add(new testClass
            {
                x123 = "blub1",
                xf = 2,
                t = 2.5f,
                test = 1
            });
            liste.Add(new testClass
            {
                x123 = "test",
                xf = 3,
                t = 3.5f,
                test = "service"
            });
            liste.Add(new testClass
            {
                x123 = "   blub3   ",
                xf = 4,
                t = 4.5f,
                test = "b"
            });

            LinqExtension.MethodInfos.Add("test", typeof(Form1).GetMethod("containsTestString"));

            ObjectListView <testClass> objList = new ObjectListView<testClass>();
            objList.DataSource = liste;

            bindingSource1.DataSource = objList;

            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize("[x123] > 5");

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource1;
            bindingSource1.Filter = "test([x123])";
        }

        public static bool containsTestString(string x)
        {
            return x.Contains("test");
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            bindingSource1.Filter = "NOT xf = 4 AND NOT xf = 1";
        }
    }

    public class testClass
    {
        public string x123 { get; set; }
        public int xf { get; set; }
        public float t { get; set; }

        public object test { get; set; }
    }
}
