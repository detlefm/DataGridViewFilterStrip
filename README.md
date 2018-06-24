# FilterStrip
Attachment to a DataGridView to provide a ContextMenuStrip for sorting and filtering a list of business objects.

Together with a DataGridContextMenuHelper it provides a ContextMenu where the DataGridView can be filtered by the value of the actual cell.
Additional  (user defined) filters can be attached to every DataProperty of the business model.

The filter mechanism is provided by a BindingList implementation which also provide core sorting by clicking on the header of a column.


When sorting not work please check the column properties, the SortMode must be "Automatic".
The designer sets by default the SortMode to "NotSortable" when the datatype of the column is Boolean.

## Usage

Please take a look into the TestApp.

The steps (without user defined filters) are:

```
 private FilterStrip<Person> filterStrip;

 // and in the method where you set the DataSource

 // init the data
    List<Person> lst = Person.CreateData();
    // create a DataGridContextMenuHelper and attach it to the DataGridView
    // this class provides a context menu for every purpose
    // in this example we use it only for the filter functionality
    contextMenuHelper = new DataGridContextMenuHelper(dataGridView1);
    // create a FilterStrip of Person and register it at the ContextMenuHelper
    filterStrip = new FilterStrip<Person>(contextMenuHelper);
    // personBindingSource is already the DataSource of the DataGridView
    // done by the visual creation of the personBindingSource in the Designer
    personBindingSource.DataSource = new ObjectBindingList<Person>(lst);

 ```
 With user defines filters:

```
 private FilterStrip<Person> filterStrip;

 // create your own filter
 private GridFilter<Person> GreaterName() {
    return new GridFilter<Person> {
        // {HeaderText} is a placeholder for the column header text
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

 // and in the method where you set the DataSource

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

 ```

