using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridViewFilterStrip {
    public class ToolStripDescription {
        public string GroupName { get; set; }
        // not used 20180623
        public int GroupOrder { get; set; }
        public ToolStripItem Item { get; set; }


    }

    public class ContextStripEventArgs : EventArgs {
        public StripDataEx StripDataEx { get; set; }
        public List<ToolStripDescription> ToolStripDescriptions = new List<ToolStripDescription>();
        public bool Added { get; set; }
    }

    public class DataGridContextMenuHelper {
        // with the help of
        // https://stackoverflow.com/questions/1718389/right-click-context-menu-for-datagridview/9641704#9641704


        //Func<StripDataEx, ContextMenuStrip> getStrip;

        public delegate void dStripDataNeeded(object sender, ContextStripEventArgs args);

        public event dStripDataNeeded StripDataNeeded = (s, a) => { };

        public DataGridContextMenuHelper(DataGridView grid) {
            grid.CellMouseDown += Grid_CellMouseDown;
            grid.KeyDown += Grid_KeyDown;
            grid.CellContextMenuStripNeeded += Grid_CellContextMenuStripNeeded;
        }

  

        private void Grid_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e) {
            DataGridView grid = (sender as DataGridView);
            StripDataEx data = new StripDataEx {
                RowIndex = e.RowIndex,
                ColIndex = e.ColumnIndex,
                Grid = grid
            };
            if (e.RowIndex != -1) {
                // if not header add the DataBoundItem
                try {
                    data.DataBoundItem = grid.Rows[e.RowIndex].DataBoundItem;
                }
                catch (Exception) {
                }
            }
            if (e.ColumnIndex != -1) {
                data.HeaderText = grid.Columns[e.ColumnIndex].HeaderText;
                if (data.DataBoundItem != null) {
                    data.Getter = data.DataBoundItem.GetType().GetProperty(grid.Columns[e.ColumnIndex].DataPropertyName);
                    if (data.Getter != null) {
                        data.ItemData = data.Getter.GetValue(data.DataBoundItem);
                    }
                }
            }
            ContextStripEventArgs args = new ContextStripEventArgs {
                StripDataEx = data, Added = false
            };
            StripDataNeeded(this, args);
            if (args.ToolStripDescriptions.Count > 0) {
                ContextMenuStrip strip = new ContextMenuStrip();
                string grpName = args.ToolStripDescriptions[0].GroupName;
                foreach (ToolStripDescription item in args.ToolStripDescriptions) {
                    if (grpName!= item.GroupName) {
                        strip.Items.Add(new ToolStripMenuItem("-"));
                        grpName = item.GroupName;
                    }
                    strip.Items.Add(item.Item);
                }
                e.ContextMenuStrip = strip;
            }
        }



        // Add Keyboard opening to Context menu

        private void Grid_KeyDown(object sender, KeyEventArgs e) {
            if ((e.KeyCode == Keys.F10 && e.Shift) || e.KeyCode == Keys.Apps) {
                e.SuppressKeyPress = true;
                DataGridViewCell currentCell = (sender as DataGridView).CurrentCell;
                if (currentCell != null) {
                    ContextMenuStrip cms = currentCell.ContextMenuStrip;
                    if (cms != null) {
                        Rectangle r = currentCell.DataGridView.GetCellDisplayRectangle(currentCell.ColumnIndex, currentCell.RowIndex, false);
                        Point p = new Point(r.X + r.Width, r.Y + r.Height);
                        cms.Show(currentCell.DataGridView, p);
                    }
                }
            }
        }


        private void Grid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e) {
            DataGridView dgv = sender as DataGridView;
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                if (e.ColumnIndex != -1 && e.RowIndex != -1) {
                    DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
                    if (!c.Selected) {
                        c.DataGridView.ClearSelection();
                        c.DataGridView.CurrentCell = c;
                        c.Selected = true;
                    }
                }
            }
        }
    }
}
