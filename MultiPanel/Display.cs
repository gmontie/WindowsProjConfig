using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiPanel
{
    public partial class Display : Panel
    {
        #region Constants
        const String ThisType = "Custom Display Page";
        #endregion

        #region Enumeration
        public enum Direction : int
        { 
            Send, 
            Receive,
        };
        #endregion

        #region Variables
        static int Counter = 0;
        #endregion

        //----------------------------------------------------------------------
        //
        //
        public Display()
        {
            InitializeComponent();

            // Force Defaults for each Display Page
            PagesIndex = -1;
            PageId = -1;
            PageName = "Display_" + Counter.ToString();
            Title = "Title_" + Counter.ToString();
            Text = "Text_" + Counter.ToString();

            Counter++;
        }

        //----------------------------------------------------------------------
        //
        //
        public void CompleteInitialization()
        {
            if (Text == null)
                Text = "Page_" + PagesIndex.ToString();
            if (Title == null)
                Title = "Title_" + PagesIndex.ToString();
        }

        #region Attributes
        //----------------------------------------------------------------------
        //
        //
        public String GetThisType { get { return ThisType;} }

        //----------------------------------------------------------------------
        //
        //
        public int PagesIndex { get; set; }

        //----------------------------------------------------------------------
        //
        //
        public int PageId { get; set; }

        //----------------------------------------------------------------------
        //
        //
        public Direction CommunicatonType { get; set; }

        //----------------------------------------------------------------------
        //
        //
        public String PageName { get; set; }

        //----------------------------------------------------------------------
        //
        //
        public override String Text { get; set; }

        //----------------------------------------------------------------------
        //
        //
        public String Title { get; set; }
        #endregion
    }
}
