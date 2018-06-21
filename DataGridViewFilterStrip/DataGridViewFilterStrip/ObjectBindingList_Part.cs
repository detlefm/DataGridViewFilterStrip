using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataGridViewFilterStrip {

    public class ListFilterDescription<T> {
        public Func<IEnumerable<T>, PropertyInfo, object, IEnumerable<T>> FilterFunc { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public object CompareObject { get; set; }
    }

    public interface IObjectFilterView<T> {
        void SetFilter(ListFilterDescription<T> listFilterDescription);
        void RemoveFilter();
    }

    public partial class ObjectBindingList<T> : IBindingListView, IObjectFilterView<T> {

        public class BindingListFilterEventArgs : EventArgs {
            public string FilterString { get; set; }
            public IEnumerable<T> Source { get; set; }
            public IEnumerable<T> Result { get; set; }
            public bool Cancel { get; set; } = false;
        }

        public delegate void dFilterCallback (object sender, BindingListFilterEventArgs args);

        public event dFilterCallback FilterCallback = (s, a) => { };

       
        string filterString;
        List<T> originalLst = null;

        public bool SupportsFiltering => true;

        public string Filter {
            get => filterString;
            set => ApplyFilter(value);
        }

        public void RemoveFilter() {
            RaiseListChangedEvents = false;
            this.ClearItems();
            foreach (var item in originalLst) {
                this.Add(item);
            }
            RaiseListChangedEvents = true;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private void ApplyFilter(string value) {
            string lastFilter = filterString;
            filterString = value;
            // Save the default value
            if (originalLst == null) {
                originalLst = new List<T>();
                originalLst.AddRange(base.Items);
            }
            BindingListFilterEventArgs args = new BindingListFilterEventArgs {
                Cancel = false, FilterString = value, Source = originalLst, Result = new List<T>()
            };
            FilterCallback.Invoke(this, args);
            if (args.Cancel == false) {
                this.ClearItems();
                foreach (var item in args.Result) {
                    this.Add(item);
                }
            } 
        }


        #region IObjectFilterView
     

        public void SetFilter(ListFilterDescription<T> listFilterDescription) {
            string lastFilter = filterString;
            filterString = "$$CUSTOMCALL$$";
            if (originalLst == null) {
                originalLst = new List<T>();
                originalLst.AddRange(base.Items);
            }
            this.ClearItems();
            var result = listFilterDescription.FilterFunc(originalLst, listFilterDescription.PropertyInfo, listFilterDescription.CompareObject);
            foreach (var item in result) {
                this.Add(item);
            }
        }
        #endregion


        #region NOT_USED_INTERFACE_IMPLEMENTATION
        ListSortDescriptionCollection listSortDescriptionCollection = null;

        public ListSortDescriptionCollection SortDescriptions => listSortDescriptionCollection;

        public bool SupportsAdvancedSorting => false;

        public void ApplySort(ListSortDescriptionCollection sorts) {
            listSortDescriptionCollection = sorts;
        }
    
        #endregion



    }
}
