using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridViewFilterStrip {

    public class GridFilter<T> {
        public Func<IEnumerable<T>, PropertyInfo, object, IEnumerable<T>> Filter { get; set; }
        public string DisplayString { get; set; }
    }



    public class FilterStrip<T> {

        // here we bind together, the data from the datagridview and the filter choices
        private class FilterStatus<T> {
            public GridFilter<T> GridFilter { get; set; }
            public StripDataEx StripDataEx { get; set; }
            public bool IsActive { get; set; }
        }

        DataGridContextMenu menuWorker = null;

        private Dictionary<PropertyInfo, List<GridFilter<T>>> Filters { get; set; } =
           new Dictionary<PropertyInfo, List<GridFilter<T>>>();

        //public FilterDictionary<T> FilterDictionary { get; set; } = new FilterDictionary<T>();

        private FilterStatus<T> currentFilter = null;

        public FilterStrip(DataGridView gridView) {
            menuWorker = new DataGridContextMenu(gridView, OpenStrip);
        }

        public void AddFilter(PropertyInfo propertyInfo, GridFilter<T> gridFilter) {
            if (Filters.TryGetValue(propertyInfo,out List<GridFilter<T>> result) == false) {
                Filters[propertyInfo] = new List<GridFilter<T>>();
            }
            Filters[propertyInfo].Add(gridFilter);
        }

        public void AddFilter(string propertyName, GridFilter<T> description) {
            AddFilter(typeof(T).GetProperty(propertyName), description);
        }

        public void AddStandardFilters() {
            foreach(PropertyInfo pi in typeof(T).GetProperties()) {
                AddFilter(pi, EqualFilter());
            }
        }

        private bool SaveNullCompare(object a, object b) {
            if (a == null && b == null)
                return true;
            if (a == null || b == null)
                return false;
            return a.Equals(b);
        }

        private GridFilter<T> EqualFilter() {
            return new GridFilter<T> {
                DisplayString = "{HeaderText} = ",
                Filter = new Func<IEnumerable<T>, PropertyInfo, object, IEnumerable<T>>(
                     delegate (IEnumerable<T> src, PropertyInfo getter, object cmp) {
                         if (getter == null)
                             return src;
                         return src.Where(p => cmp == null ? getter.GetValue(p) == null : SaveNullCompare(getter.GetValue(p), cmp));
                     })
            };
        }


        private string FormatDisplayString(FilterStatus<T> fs) {
            string result = fs.GridFilter.DisplayString;
            if (fs.StripDataEx.HeaderText != null) {
                result = result.Replace("{HeaderText}", fs.StripDataEx.HeaderText);
            }
            result += fs.StripDataEx.ItemData == null ? "NULL" : fs.StripDataEx.ItemData.ToString();
            return result;
        }

        private ToolStripMenuItem CreateMenuitem(string txt, GridFilter<T> gf, StripDataEx dataEx,bool clickable = false) {
            ToolStripMenuItem item = new ToolStripMenuItem(txt);
            item.Tag = new FilterStatus<T> {
                GridFilter = gf,
                StripDataEx = dataEx
            };
            if (clickable)
                item.Click += Item_Click;
            return item;
        }

        private ToolStripMenuItem CreateMenuitem(FilterStatus<T> fs) {
            string txt = FormatDisplayString(fs);
            ToolStripMenuItem item = new ToolStripMenuItem(txt);
            item.Tag = fs;
            item.Click += Item_Click;
            item.Checked = fs.IsActive;
            return item;
        }

        private ToolStripMenuItem HeaderMenuItem() {
            ToolStripMenuItem header = new ToolStripMenuItem("Filter by");
            header.BackColor = Color.LightGray;
            header.ForeColor = Color.DarkBlue;
            return header;
        }

        private ContextMenuStrip OpenStrip(StripDataEx stripDataEx) {

            List<ToolStripItem> tsList = new List<ToolStripItem>();


            if (currentFilter != null) {
                ToolStripMenuItem item =  CreateMenuitem("Clear Filter", null, stripDataEx,true);
                tsList.Add(item);
                item = CreateMenuitem(currentFilter);
                tsList.Add(item);
            }
       
            if (stripDataEx.Getter != null) {
                List<GridFilter<T>> filterDescLst = new List<GridFilter<T>>();
                if (Filters.TryGetValue(stripDataEx.Getter, out List<GridFilter<T>> result)) {
                    filterDescLst.AddRange(result);
                }
                if (filterDescLst.Count == 0) {
                    filterDescLst.Add(EqualFilter());
                }
                foreach (var filterdesc in filterDescLst) {
                    FilterStatus<T> status = new FilterStatus<T> {
                        GridFilter = filterdesc,
                        StripDataEx = stripDataEx,
                        IsActive = false
                    };
                    if (currentFilter == null ||
                        (currentFilter.GridFilter != status.GridFilter ||
                        currentFilter.StripDataEx.Getter != status.StripDataEx.Getter ||
                        currentFilter.StripDataEx.ItemData == status.StripDataEx.ItemData)) {
                        tsList.Add(CreateMenuitem(status));
                    } 
                }
            }
            // take a look what we have to far
            // when there is no current and a header click we have a count of 0
            if (tsList.Count > 0) {
                ContextMenuStrip strip = new ContextMenuStrip();
                tsList.Insert(0,HeaderMenuItem());
                //tsList.Insert(1, new ToolStripMenuItem("-"));
                strip.Items.AddRange(tsList.ToArray());
                return strip;
            }
            return null;
        }

        private bool TryGetIObjectFilterView(DataGridView grid, out IObjectFilterView<T> objectView) {
            objectView = null;
            // it can be the bound direct to the DataSource Property or via a BindingSource or even not 
            objectView = grid.DataSource as IObjectFilterView<T>;
            if (objectView == null) {
                BindingSource bs = grid.DataSource as BindingSource;
                if (bs != null)
                    objectView = bs.DataSource as IObjectFilterView<T>;
            }
            return objectView != null;
        }

        private void Item_Click(object sender, EventArgs e) {
            ToolStripItem item = sender as ToolStripItem;
            FilterStatus<T> filterStatus = item.Tag as FilterStatus<T>;
            if (TryGetIObjectFilterView(filterStatus.StripDataEx.Grid, out IObjectFilterView<T> objectView)) {
                if (filterStatus.GridFilter == null || filterStatus.GridFilter.Filter == null) {
                    objectView.RemoveFilter();
                    currentFilter = null;
                    return;
                }
                else {
                    if (currentFilter == filterStatus) {
                        objectView.RemoveFilter();
                        currentFilter = null;
                    }
                    else {
                        objectView.SetFilter(new ListFilterDescription<T> {
                            CompareObject = filterStatus.StripDataEx.ItemData,
                            FilterFunc = filterStatus.GridFilter.Filter,
                            PropertyInfo = filterStatus.StripDataEx.Getter
                        });
                        filterStatus.IsActive = true;
                        currentFilter = filterStatus;
                    }
                }
            }
        }
    }
}
