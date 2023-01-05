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
    public partial class DisplayPanels : Control.ControlCollection
    {
        #region Constants
        const String ThisType = "Custom Constainer";
        #endregion

        #region Variables
        private Display CurrentPage;
        private int CurrentIndex;
        private int CountOfPages;
        private Dictionary<Display, int> PageToIndex;
        private Dictionary<int, Display> IndexToPage;
        private Dictionary<String, Display> NameToPage;
        private String CurrentPageName;
        #endregion

        public DisplayPanels(Control ControlInQuestion) : base(ControlInQuestion)
        {
            InitializeComponent();
            if (ControlInQuestion == null)
                throw new ArgumentNullException("owner", "Tried to create a Custom Container Collection with a null owner.");

            CountOfPages = 0;
            CurrentIndex = 0;
            PageToIndex = new Dictionary<Display, int>();
            IndexToPage = new Dictionary<int, Display>();
            NameToPage = new Dictionary<string, Display>();
        }

        #region Methods
        //----------------------------------------------------------------------
        //
        //
        public override void Add(Control NewControl)
        {
            if (NewControl == null)
                throw new ArgumentNullException("value", "Tried to add a null value to the Custom Container Collection.");
            Display NewDisplay = NewControl as Display;
            if (NewDisplay == null)
                throw new ArgumentException("Tried to add a non-Display Page control to the Custom Container Collection", "value");
            NewDisplay.PagesIndex = base.Count;
            NewDisplay.Title = "Page_" + CountOfPages.ToString();
            NewDisplay.SendToBack();
            NewDisplay.PageId = 0;
            base.Add(NewDisplay);
            PageToIndex.Add(NewDisplay, NewDisplay.PagesIndex);
            IndexToPage.Add(NewDisplay.PagesIndex, NewDisplay);
            NameToPage.Add(NewDisplay.Title, NewDisplay);
        }

        //----------------------------------------------------------------------
        //
        //
        public override void AddRange(Control[] ArrayOfControls)
        {
            foreach (Display aDisplay in ArrayOfControls)
            {
                aDisplay.PageId = 0;
                Add(aDisplay);
            }

        }

        //----------------------------------------------------------------------
        //
        //
        public override void Remove(Control ControlInQuestion)
        {
            if (ControlInQuestion == null)
                throw new ArgumentNullException("null value control", "Found null value object in the Custom Container Collection.");
            Display Page = ControlInQuestion as Display;
            if (Page == null)
                throw new ArgumentException("Found null value object in the Custom Container Collection.", "null value control");
            int PagesIndex = PageToIndex[Page];
            String Str = Page.Title;
            PageToIndex.Remove(Page);
            IndexToPage.Remove(PagesIndex);
            NameToPage.Remove(Str);
            base.Remove(Page);
        }

        //----------------------------------------------------------------------
        //
        //
        public override int IndexOfKey(String Key)
        {
            Display ctrl = NameToPage[Key]; // Get the control based on the string
            int Results = PageToIndex[ctrl]; // Get the index based on the Panel
            return Results;
        }

        //----------------------------------------------------------------------
        //
        //
        public Boolean Contains(String PageName)
        {
            Display Dp;
            Boolean Results = false;

            if (NameToPage.TryGetValue(PageName, out Dp))
                Results = true;
            return Results;
        }

        //----------------------------------------------------------------------
        //
        //
        public Display Select(String PageName)
        {
            return NameToPage[PageName];
        }

        //----------------------------------------------------------------------
        //
        //
        public Display Select(int PageIndex)
        {
            return IndexToPage[PageIndex];
        }

        //----------------------------------------------------------------------
        //
        //
        public void Reset()
        {
            CurrentIndex = 0;
            CurrentPage = base[0] as Display;
        }

        //----------------------------------------------------------------------
        //
        //
        public bool MoveNext()
        {
            Boolean Results = false;
            Control ContainedControl;

            CurrentIndex++;
            if (CurrentIndex < base.Count)
            {
                ContainedControl = base[CurrentIndex];
                CurrentPage = ContainedControl as Display;
                if (CurrentPage != null)
                    Results = true;
            }
            else
            {
                CurrentIndex = 7;
            }
            return Results;
        }
        #endregion

        #region Attributes
        //----------------------------------------------------------------------
        //
        //
        public Boolean HasPages
        {
            get
            {
                Boolean Results = (base.Count > 0);
                return Results;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public Dictionary<Display, int> GetPageToIndex
        { get { return PageToIndex; } }

        //----------------------------------------------------------------------
        //
        //
        public Dictionary<int, Display> GetIndexToPage
        { get { return IndexToPage; } }

        //----------------------------------------------------------------------
        //
        //
        public String GetPagesTitle
        { get { return CurrentPage.Title; } }

        //----------------------------------------------------------------------
        //
        //
        public int GetPageId
        { get { return CurrentPage.PageId; } }

        //----------------------------------------------------------------------
        //
        //
        public int SetPageId
        { set { CurrentPage.PageId = value; } }

        //----------------------------------------------------------------------
        //
        //
        public int GetPagesIndex
        { get { return CurrentPage.PagesIndex; } }

        //----------------------------------------------------------------------
        //
        //
        public String SetPageTitle
        { set { CurrentPage.Title = value; } }

        //----------------------------------------------------------------------
        //
        //
        public String GetPageName
        { get { return CurrentPage.Name; } }

        //----------------------------------------------------------------------
        //
        //
        public Display SelectedPage
        {
            get { return CurrentPage; }
            set
            {
                if (value != null)
                {
                    System.Collections.IEnumerator e = base.GetEnumerator();

                    CurrentPage = value;

                    while (e.MoveNext())
                    {
                        Object Obj = e.Current;
                        // Display NewDisplay = NewControl as Display;
                        Display Page = Obj as Display;
                        if (Obj != null)
                        {
                            if (object.ReferenceEquals(Obj, value))
                                Page.Visible = true;
                            else
                                Page.Visible = false;
                        }
                    } // foreach
                }
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public String GetThisType
        { get { return ThisType; } }
        #endregion

    }
}
