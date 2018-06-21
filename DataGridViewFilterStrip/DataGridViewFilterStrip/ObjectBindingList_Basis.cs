using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataGridViewFilterStrip {
    public partial class ObjectBindingList<T> : BindingList<T> {


        public ObjectBindingList() : base() { }

        public ObjectBindingList(IList<T> list) : base(list) { }


        private bool ísSorted;
        private ListSortDescription sortCore = new ListSortDescription(null, ListSortDirection.Ascending);


        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction) {
            Expression<Func<T, object>> sortExpression = (it => prop.GetValue(it));

            IOrderedQueryable<T> sorted = null;
            if (direction == ListSortDirection.Ascending)
                sorted = base.Items.AsQueryable<T>().OrderBy(sortExpression);
            else
                sorted = base.Items.AsQueryable<T>().OrderByDescending(sortExpression);
            sortCore.PropertyDescriptor = prop;
            sortCore.SortDirection = direction;
            try {
                int i = 0;
                foreach (T item in sorted) {
                    this.Items[i] = item;
                    i++;
                }
                ísSorted = true;
            }
            catch (System.ArgumentException) {
                // if not IComparable is implemented
            }
        }

        protected override void RemoveSortCore() {
            sortCore.PropertyDescriptor = null;
            sortCore.SortDirection = ListSortDirection.Ascending;
            ísSorted = false;
        }



        protected override PropertyDescriptor SortPropertyCore => sortCore.PropertyDescriptor; 

        protected override ListSortDirection SortDirectionCore => sortCore.SortDirection; 

        protected override bool SupportsSortingCore => true;

        protected override bool IsSortedCore => ísSorted;

    }
}
