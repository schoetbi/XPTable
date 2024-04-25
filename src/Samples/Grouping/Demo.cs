using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using XPTable.Events;
using XPTable.Models;

//using EventTracer;

namespace Grouping
{
    public class Demo : System.Windows.Forms.Form
    {
        //Tracer _tracer = null;

        private XPTable.Models.Table table;
        private readonly System.ComponentModel.Container components = null;
        private readonly Bitmap _unread = null;
        private readonly Bitmap _read = null;

        public Demo()
        {
            InitializeComponent();
            _unread = new Bitmap("resources\\EmailUnRead.bmp");
            _read = new Bitmap("resources\\EmailRead.bmp");
        }

        private void Demo_Load(object sender, EventArgs e)
        {
            DoGroup();
        }

        private void DoGroup()
        {
            var table = this.table;       // The Table control on a form - already initialised
            //table.Font = new Font(table.Font.FontFamily, 12f);
            table.SelectionStyle = SelectionStyle.Grid;
            table.BeginUpdate();
            table.EnableWordWrap = true;    // If false, then Cell.WordWrap is ignored
            table.FamilyRowSelect = true;
            table.FullRowSelect = true;
            table.ShowSelectionRectangle = false;
            table.MultiSelect = true;

            table.GridLines = GridLines.Both;

            var col0 = new GroupColumn("", 20)
            {
                Editable = false,                      // Double clicking on this is to toggle the collapsed state
                Selectable = false,
                ToggleOnSingleClick = true
            };   // this is the NEW +/- column
            var col1 = new ImageColumn("", 20);
            var col2 = new TextColumn("From", 200);
            var col3 = new DateTimeColumn("Sent", 180)
            {
                /// 493
                ShowDropDownButton = false,
                DateTimeFormat = DateTimePickerFormat.Custom,
                CustomDateTimeFormat = "d/M/yyyy hh:mm",
                Alignment = ColumnAlignment.Right,
                AutoResizeMode = ColumnAutoResizeMode.Any
            };             //NumberColumn col4 = new NumberColumn("num", 60);
            //col4.ShowUpDownButtons = true;
            var col4 = new ButtonColumn("butt")
            {
                FlatStyle = true
            };

            table.ColumnModel = new ColumnModel(new Column[] { col0, col1, col2, col3, col4 });

            var model = new TableModel();
            //model.RowHeight = 24;

            AddEmailRows(model, true, "Dave ", "4/9/2007 12:34", "Here is the email subject", "Here is a preview of the text that is in the email. It wraps over too so you can see more of it");

            AddEmailRows(model, false, "Richard Richard Richard Richard Richard Richard Richard Richard Richard Richard Richard Richard Richard Richard", "5/4/2007 9:13", "An email abut something", "Another preview of another ficticious email. Not much to say really");

            AddEmailRows(model, true, "Andy", "13/2/2007 9:45", "Work stuff", "Can you get this finished by this afternoon please? Thanks");

            // Make and add the context menu:

            var menu = new ContextMenuStrip();
            var delete = new ToolStripMenuItem("Delete");
            delete.Click += new EventHandler(delete_Click);
            menu.Items.Add(delete);
            table.ContextMenuStrip = menu;

            // Add an event handler for the key event
            table.CellKeyUp += new CellKeyEventHandler(table_CellKeyUp);
            table.CellButtonClicked += new CellButtonEventHandler(table_CellButtonClicked);

            table.CellMouseDown += new CellMouseEventHandler(table_CellMouseDown);
            table.CellMouseUp += new CellMouseEventHandler(table_CellMouseUp);

            this.table.TableModel = model;

            this.table.EndUpdate();

            return;
            #region Tracer stuff
            //_tracer = new Tracer(table, OnEventTrace);
            //_tracer.HookEvent("BeginEditing");
            //_tracer.HookEvent("CellClick");
            //_tracer.HookEvent("CellDoubleClick");
            //_tracer.HookEvent("CellMouseDown");
            //_tracer.HookEvent("CellMouseEnter");
            //_tracer.HookEvent("CellMouseHover");
            //_tracer.HookEvent("CellMouseLeave");
            //_tracer.HookEvent("CellMouseMove");
            //_tracer.HookEvent("CellMouseUp");
            //_tracer.HookEvent("CellPropertyChanged");
            //_tracer.HookEvent("Click");
            //_tracer.HookEvent("DoubleClick");
            //_tracer.HookEvent("EditingStopped");
            //_tracer.HookEvent("Enter");
            //_tracer.HookEvent("GotFocus");
            //_tracer.HookEvent("LostFocus");
            //_tracer.HookEvent("MouseCaptureChanged");
            //_tracer.HookEvent("MouseClick");
            //_tracer.HookEvent("MouseDoubleClick");
            //_tracer.HookEvent("MouseDown");
            //_tracer.HookEvent("MouseEnter");
            //_tracer.HookEvent("MouseHover");
            //_tracer.HookEvent("MouseLeave");
            //_tracer.HookEvent("MouseUp");
            //            _tracer.HookAllEvents();
            #endregion
        }

        private CellMouseEventArgs _down = null;

        private void table_CellMouseUp(object sender, CellMouseEventArgs e)
        {
            Console.WriteLine("UP   start {0}; end {1}", _down.Row, e.CellPos);
        }

        private void table_CellMouseDown(object sender, CellMouseEventArgs e)
        {
            _down = e;
            Console.WriteLine("DOWN start {0}", _down.Row);
        }

        #region Tracer methods
        private void OnEventTrace(object sender, object target, string eventName, EventArgs e)
        {
            var s = string.Format("{0} - args {1} - sender {2} - target {3}",
                                      eventName,
                                      GetString(e),
                                      sender ?? "null",
                                      target ?? "null");

            Console.WriteLine(s);
        }

        private string GetString(EventArgs args)
        {
            if (args is InvalidateEventArgs iargs)
            {
                var z = string.Format("{0} [{1}]", args, iargs.InvalidRect);
                return z;
            }
            else
            {
                return args.ToString();
            }
        }
        #endregion

        private void AddEmailRows(TableModel table, bool read, string from, string sent, string subject, string preview)
        {
            var row = new Row();
            //row.Alignment = RowAlignment.Top;
            row.Cells.Add(new Cell());       // always starts off showing all subrows
            row.Cells.Add(new Cell("", read ? _read : _unread));
            var fro = new Cell(null)
            {
                WordWrap = true
            }; //from);
            row.Cells.Add(fro);
            var cellSent = new Cell(DateTime.Parse(sent));
            if (sent == "5/4/2007 9:13")
            {
                cellSent.CellStyle = new CellStyle(ColumnAlignment.Left)
                {
                    LineAlignment = RowAlignment.Top
                };
            }
            row.Cells.Add(cellSent);
            row.Cells.Add(new Cell("hi"));
            row.RowStyle = new XPTable.Models.RowStyle
            {
                Alignment = RowAlignment.Top
            };
            table.Rows.Add(row);

            // Add a sub-row that shows just the email subject in grey (single line only)
            var subrow = new Row();
            subrow.Cells.Add(new Cell());   // Extra column for +/-
            subrow.Cells.Add(new Cell());
            subrow.RowStyle = new XPTable.Models.RowStyle
            {
                Alignment = RowAlignment.Bottom
            };
            var cell = new Cell(subject)
            {
                ForeColor = Color.Gray,
                ColSpan = 3
            };

            subrow.Cells.Add(cell);
            row.SubRows.Add(subrow);

            // Add a sub-row that shows just a preview of the email body in blue, and wraps too
            subrow = new Row();
            subrow.Cells.Add(new Cell());   // Extra column for +/-
            subrow.Cells.Add(new Cell());
            cell = new Cell(preview)
            {
                ForeColor = Color.Blue,
                ColSpan = 3,
                WordWrap = true
            };
            subrow.RowStyle = new XPTable.Models.RowStyle
            {
                Alignment = RowAlignment.Bottom
            };
            subrow.Cells.Add(cell);
            row.SubRows.Add(subrow);
        }

        /// <summary>
        /// Fired when any key up happens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void table_CellKeyUp(object sender, CellKeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var rows = e.Table.TableModel.Rows;
                rows.Remove(rows[e.Row]);
            }
        }

        /// <summary>
        /// Fired when the user clicks the delete menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void delete_Click(object sender, EventArgs e)
        {
            // Note that the event itself doesn't give us the cell or row
            // where it was clicked - we have to ask the Table for its SelectedRows
            // Bit of a faff to get our Table...
            var rows = table.TableModel.Rows;
            // Its possible more than one is selected
            foreach (var row in table.SelectedItems)
            {
                rows.Remove(row);
            }
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            table = new XPTable.Models.Table();
            ((System.ComponentModel.ISupportInitialize)table).BeginInit();
            SuspendLayout();
            // 
            // table
            // 
            table.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            table.Location = new System.Drawing.Point(12, 12);
            table.Name = "table";
            table.Size = new System.Drawing.Size(493, 257);
            table.TabIndex = 0;
            table.Text = "table1";
            // 
            // Demo
            // 
            ClientSize = new System.Drawing.Size(517, 281);
            Controls.Add(table);
            Name = "Demo";
            Text = "Grouping";
            Load += new System.EventHandler(Demo_Load);
            ((System.ComponentModel.ISupportInitialize)table).EndInit();
            ResumeLayout(false);

        }
        #endregion

        [STAThread]
        private static void Main()
        {
            //Application.EnableVisualStyles();
            Application.Run(new Demo());
        }

        private void table_CellButtonClicked(object sender, CellButtonEventArgs e)
        {
            DoMyClickThing(e.CellPos);
        }

        private void SomeOtherProcess()
        {
            // Decide what cell you are interested in
            DoMyClickThing(new CellPos(1, 3));
        }

        private void DoMyClickThing(CellPos cellpos)
        {
            // Off we go...
            table.ClearAllData();
        }
    }

    public class mybinder : DataSourceColumnBinder
    {
        public mybinder()
            : base()
        {
        }

        public override Column GetColumn(PropertyDescriptor prop, int index)
        {
            if (prop.Name == "colour")
            {
                return new NumberColumn(prop.Name);
            }
            else
            {
                return base.GetColumn(prop, index);
            }
        }
    }
}
