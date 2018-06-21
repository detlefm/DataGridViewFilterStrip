﻿using DataGridViewFilterStrip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }


        FilterStrip<Person> filterStrip;


        private GridFilter<Person> GreaterName() {
            return new GridFilter<Person> {
                DisplayString = "{HeaderText} of Person > ",
                Filter = new Func<IEnumerable<Person>, PropertyInfo, object, IEnumerable<Person>>(
                     delegate (IEnumerable<Person> src, PropertyInfo getter, object cmp) {
                         // if cmp is null, every item is greater
                         // this routine throws an exception if one of name is null
                         if (cmp == null)
                             return src;
                         return src.Where(p => ((string)cmp).CompareTo(p.Name) < 0);
                     })
            };
        }

        private void Form1_Load(object sender, EventArgs e) {
            filterStrip = new FilterStrip<Person>(dataGridView1);
            filterStrip.AddStandardFilters();
            filterStrip.AddFilter("Name", GreaterName());
            personBindingSource.DataSource = new ObjectBindingList<Person>(Person.CreateData());
        }
    }
}