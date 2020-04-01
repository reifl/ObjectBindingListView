﻿using ObjectBoundBindingList;
using ObjectBoundBindingList.Parser;
using ObjectBoundBindingList.Tokenizer;
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
                x123 = "blub3",
                xf = 4,
                t = 4.5f,
                test = "b"
            });

            ObjectBindingList<testClass> objList = new ObjectBindingList<testClass>();
            objList.DataSource = liste;

            bindingSource1.DataSource = objList;


            var t = new Tokenizer();
            var tokens = t.Tokenize("test IS NULL AND ISNULL(test, 'blub')");
            var dslParser = new DslParser();
            var queryModel = dslParser.Parse(tokens);
            System.Diagnostics.Debugger.Break();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = bindingSource1;
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            
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
