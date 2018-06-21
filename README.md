# DataGridViewFilterStrip
Attachment to a DataGridView to provide a ContextMenuStrip for sorting and filtering a list of business objects.

Out of the box it provides a ContextMenu where the DataGridView can be filtered by the value of the actual cell.
Additional  (user defined) filters can be attached to every DataProperty of the business model.

The filter mechanism is provided by a BindingList implementation which also provide core sorting by clicking on the header of a column.

Known issues:   
Actual it works only for fixed Lists. When an item is added or removed to the underlying list the BindingList implementation is not aware of this changes.  
When sorting not work please check the column properties, the SortMode must be "Automatic". The designer sets by default the SortMode to "NotSortable" when the datatype of the column is Boolean.   

## Usage

Please take a look into the TestApp.

The steps (without user defined filters) are:

```
 private FilterStrip<Person> filterStrip;
 
 // and in the method where you set the DataSource
 
 filterStrip = new FilterStrip<Person>(dataGridView1);
 ObjectBindingList<Person> bl = new ObjectBindingList<Person>(lstOfPerson);
 personBindingSource.DataSource = bl;
 
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
 
 filterStrip = new FilterStrip<Person>(dataGridView1);
 // register your filter
 filterStrip.AddFilter("Name", GreaterName());
 // add standard filters (equal) if you wish
 filterStrip.AddStandardFilters();
 ObjectBindingList<Person> bl = new ObjectBindingList<Person>(lstOfPerson);
 personBindingSource.DataSource = bl;
 
 ```
 
