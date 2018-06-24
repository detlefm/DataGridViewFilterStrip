using DataGridViewFilterStrip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestData;

namespace TestApp {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }


        private FilterStrip<Person> filterStrip;
        private DataGridContextMenuHelper contextMenuHelper;


        private GridFilter<Person> GreaterName() {
            return new GridFilter<Person> {
                // {HeaderText} is a placeholder and will replaced on runtime with the columns HeaderText
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

            // init the data
            List<Person> lst = Person.CreateData();
            // create a DataGridContextMenuHelper and attach it to the DataGridView
            // this class provides a context menu for every purpose
            // in this example we use it only for the filter functionality
            contextMenuHelper = new DataGridContextMenuHelper(dataGridView1);
            // create a FilterStrip of Person and register it at the ContextMenuHelper
            filterStrip = new FilterStrip<Person>(contextMenuHelper);
            // add a custom filter (optinal)
            filterStrip.AddFilter("Name", GreaterName());

            // because we add an user defined filter we have to add 
            // the StandardFilter (Equal filter) to every column
            // if we don't do this the Equal filter is missing on the 'Name' column
            filterStrip.AddStandardFilters();

            // personBindingSource is already the DataSource of the DataGridView
            // done by the visual creation of the personBindingSource in the Designer
            personBindingSource.DataSource = new ObjectBindingList<Person>(lst);
        }

       
        
    }
}
