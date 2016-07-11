using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using XPTable.Models;
using XPTable.Events;
using System.IO;

//using EventTracer;

namespace Grouping
{
	public class Demo : System.Windows.Forms.Form
	{
        //Tracer _tracer = null;

        private XPTable.Models.Table table;
		private System.ComponentModel.Container components = null;
		Bitmap _unread = null;
		Bitmap _read = null;

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
            Table table = this.table;       // The Table control on a form - already initialised
            //table.Font = new Font(table.Font.FontFamily, 12f);
            table.SelectionStyle = SelectionStyle.Grid;
            table.BeginUpdate();
            table.EnableWordWrap = true;    // If false, then Cell.WordWrap is ignored
            table.FamilyRowSelect = true;
            table.FullRowSelect = true;
            table.ShowSelectionRectangle = false;
            table.MultiSelect = true;

            table.GridLines = GridLines.Both;

            GroupColumn col0 = new GroupColumn("", 20);   // this is the NEW +/- column
            col0.Editable = false;                      // Double clicking on this is to toggle the collapsed state
            col0.Selectable = false;
            col0.ToggleOnSingleClick = true;
            ImageColumn col1 = new ImageColumn("", 20);
            TextColumn col2 = new TextColumn("From", 200);
            DateTimeColumn col3 = new DateTimeColumn("Sent", 180); /// 493
            col3.ShowDropDownButton = false;
            col3.DateTimeFormat = DateTimePickerFormat.Custom;
            col3.CustomDateTimeFormat = "d/M/yyyy hh:mm";
            col3.Alignment = ColumnAlignment.Right;
            col3.AutoResizeMode = ColumnAutoResizeMode.Any;
            //NumberColumn col4 = new NumberColumn("num", 60);
            //col4.ShowUpDownButtons = true;
			ButtonColumn col4 = new ButtonColumn("butt");
            col4.FlatStyle = true;

			table.ColumnModel = new ColumnModel(new Column[] { col0, col1, col2, col3, col4 });

            TableModel model = new TableModel();
            //model.RowHeight = 24;

            AddEmailRows(model, true, "Dave ", "4/9/2007 12:34", "Here is the email subject", "Here is a preview of the text that is in the email. It wraps over too so you can see more of it");

            AddEmailRows(model, false, "Richard Richard Richard Richard Richard Richard Richard Richard Richard Richard Richard Richard Richard Richard", "5/4/2007 9:13", "An email abut something", "Another preview of another ficticious email. Not much to say really");

            AddEmailRows(model, true, "Andy", "13/2/2007 9:45", "Work stuff", "Can you get this finished by this afternoon please? Thanks");

            // Make and add the context menu:
            ContextMenu menu = new ContextMenu();
            MenuItem delete = new MenuItem("Delete");
            delete.Click += new EventHandler(delete_Click);
            menu.MenuItems.Add(delete);
            table.ContextMenu = menu;

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

        CellMouseEventArgs _down = null;
        void table_CellMouseUp(object sender, CellMouseEventArgs e)
        {
            Console.WriteLine("UP   start {0}; end {1}", _down.Row, e.CellPos);
        }

        void table_CellMouseDown(object sender, CellMouseEventArgs e)
        {
            _down = e;
            Console.WriteLine("DOWN start {0}", _down.Row);
        }

        #region Tracer methods
        private void OnEventTrace(object sender, object target, string eventName, EventArgs e)
        {
            string s = String.Format("{0} - args {1} - sender {2} - target {3}",
                                      eventName,
                                      GetString(e),
                                      sender ?? "null",
                                      target ?? "null");

            Console.WriteLine(s);
        }

        string GetString(EventArgs args)
        {
            if (args is InvalidateEventArgs)
            {
                InvalidateEventArgs iargs = (InvalidateEventArgs)args;
                string z = string.Format("{0} [{1}]", args, iargs.InvalidRect);
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
            Row row = new Row();
            //row.Alignment = RowAlignment.Top;
            row.Cells.Add(new Cell());       // always starts off showing all subrows
            row.Cells.Add(new Cell("", read ? _read : _unread));
            Cell fro = new Cell(null); //from);
            fro.WordWrap = true;
            row.Cells.Add(fro);
            Cell cellSent = new Cell(DateTime.Parse(sent));
            if (sent == "5/4/2007 9:13")
            {
                cellSent.CellStyle = new CellStyle(ColumnAlignment.Left);
                cellSent.CellStyle.LineAlignment = RowAlignment.Top;
            }
            row.Cells.Add(cellSent);
			row.Cells.Add(new Cell("hi"));
            row.RowStyle = new XPTable.Models.RowStyle();
            row.RowStyle.Alignment = RowAlignment.Top;
            table.Rows.Add(row);

            // Add a sub-row that shows just the email subject in grey (single line only)
            Row subrow = new Row();
            subrow.Cells.Add(new Cell());   // Extra column for +/-
            subrow.Cells.Add(new Cell());
            subrow.RowStyle = new XPTable.Models.RowStyle();
            subrow.RowStyle.Alignment = RowAlignment.Bottom;
            Cell cell = new Cell(subject);
            cell.ForeColor = Color.Gray;
            cell.ColSpan = 3;
            
            subrow.Cells.Add(cell);
            row.SubRows.Add(subrow);

            // Add a sub-row that shows just a preview of the email body in blue, and wraps too
            subrow = new Row();
            subrow.Cells.Add(new Cell());   // Extra column for +/-
            subrow.Cells.Add(new Cell());
            cell = new Cell(preview);
            cell.ForeColor = Color.Blue;
            cell.ColSpan = 3;
            cell.WordWrap = true;
            subrow.RowStyle = new XPTable.Models.RowStyle();
            subrow.RowStyle.Alignment = RowAlignment.Bottom;
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
                RowCollection rows = e.Table.TableModel.Rows;
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
            if (sender is MenuItem)
            {
                MenuItem item = (MenuItem)sender;
                if (item.Parent is ContextMenu)
                {
                    ContextMenu menu = (ContextMenu)item.Parent;
                    // SourceControl is automatically set to be the Table when
                    // the menu is added to the Table in Setup()
                    if (menu.SourceControl is Table)
                    {
                        Table t = (Table)menu.SourceControl;
                        RowCollection rows = t.TableModel.Rows;
                        // Its possible more than one is selected
                        foreach (Row row in t.SelectedItems)
                        {
                            rows.Remove(row);
                        }
                    }
                }
            }
        }

		#region Windows Form Designer generated code
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.table = new XPTable.Models.Table();
            ((System.ComponentModel.ISupportInitialize)(this.table)).BeginInit();
            this.SuspendLayout();
            // 
            // table
            // 
            this.table.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.table.Location = new System.Drawing.Point(12, 12);
            this.table.Name = "table";
            this.table.Size = new System.Drawing.Size(493, 257);
            this.table.TabIndex = 0;
            this.table.Text = "table1";
            // 
            // Demo
            // 
            this.ClientSize = new System.Drawing.Size(517, 281);
            this.Controls.Add(this.table);
            this.Name = "Demo";
            this.Text = "Grouping";
            this.Load += new System.EventHandler(this.Demo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.table)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

        [STAThread]
		static void Main() 
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
                return base.GetColumn(prop, index);
        }
    }
}
