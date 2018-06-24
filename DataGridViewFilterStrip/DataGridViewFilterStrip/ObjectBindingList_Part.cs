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
        IEnumerable<T> GetData(bool all);
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


       
        string filterString;


        public bool SupportsFiltering => false;

        public string Filter {
            get => filterString;
            set => ApplyFilter(value);
        }

     


        public void RemoveFilter() {
            RaiseListChangedEvents = false;
            List<T> lst = new List<T>(filteredList);
            lst.AddRange(base.Items);
            filteredList.Clear();
            this.ClearItems();
            foreach (var item in lst) {
                this.Add(item);
            }
            if (IsSortedCore) {
                ApplySortCore(this.SortPropertyCore, this.SortDirectionCore);
            }
            RaiseListChangedEvents = true;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private void ApplyFilter(string value) {
            throw new NotImplementedException("ApplyFilter");
        }


        #region IObjectFilterView



        List<T> filteredList = new List<T>();
        public ListFilterDescription<T> CurrentFilter { get; set; }

        public void SetFilter(ListFilterDescription<T> listFilterDescription) {
            if (CurrentFilter != null) {
                // we are already filtered
                RemoveFilter();
            }
            filterString = "$$CUSTOMCALL$$";
            CurrentFilter = listFilterDescription;
            List<T> src = new List<T>(base.Items);
            var result = listFilterDescription.FilterFunc(base.Items, listFilterDescription.PropertyInfo, listFilterDescription.CompareObject).ToList();
            filteredList = base.Items.Except(result).ToList();
            this.ClearItems();
            foreach (var item in result) {           
                this.Add(item);
            }
        }


        public IEnumerable<T> GetData(bool all) {
            List<T> tmp = new List<T>(base.Items);
            if (all && filteredList != null) {
                tmp.AddRange(filteredList);
            }
            return tmp;
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
