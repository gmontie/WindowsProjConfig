using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace RegisterControls
{
    public class GridViewBinder
    {
        private System.Windows.Forms.DataGridView aGridView;

        //----------------------------------------------------------------------
        //
        //
        public GridViewBinder(ref System.Windows.Forms.DataGridView Gv)
        {
            StartingIndex = 0;
            EndingIndex = MemDefs.MEMORY_SIZE;
            Binding = new BindingSource();
            aGridView = Gv;
            aGridView.DataSource = Binding;
            aGridView.AutoGenerateColumns = true;
            aGridView.ForeColor = Color.Wheat;
        }

        //----------------------------------------------------------------------
        //
        //
        public GridViewBinder(ref System.Windows.Forms.DataGridView Gv, int Strt, int End) 
        {
            StartingIndex = Strt;
            EndingIndex = End;
            Binding = new BindingSource();
            aGridView = Gv;
            aGridView.DataSource = Binding;
            aGridView.AutoGenerateColumns = true;
            aGridView.ForeColor = Color.Wheat;
        }

        //----------------------------------------------------------------------
        //
        //
        public void SetForDisplay()
        {
            int i;
            for (i = 0; i < aGridView.ColumnCount; i++)
            {
                switch (i)
                {
                    case 0:
                        aGridView.Columns[0].Width = 30;
                        break;
                    case 1:
                        aGridView.Columns[1].MinimumWidth = 10;
                        aGridView.Columns[1].Width = 190;
                        break;
                    case 2:
                        aGridView.Columns[2].MinimumWidth = 10;
                        aGridView.Columns[2].Width = 40;
                        break;
                    default:
                        aGridView.Columns[i].Visible = false;
                        break;
                }
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public Boolean InRange(int Index)
        {
            Boolean Results = false;
            if ((StartingIndex <= Index)&&(Index < EndingIndex))
                Results = true;
            return Results;
        }

        //----------------------------------------------------------------------
        //
        //
        public void Add(ref Register Row)
        {
            try
            { 
                Binding.Add(Row); 
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void Add(ref BindingSource bSource)
        {
            aGridView.DataSource = bSource;
            Binding = bSource;
        }
        //----------------------------------------------------------------------
        //
        //
        public void ResetBindings(Boolean ToResetOrNot) { Binding.ResetBindings(ToResetOrNot); }

        //----------------------------------------------------------------------
        //
        //
        public BindingSource Binding { get; private set; }
        public int StartingIndex { get; private set; }
        public int EndingIndex { get; private set; }
    }
}
