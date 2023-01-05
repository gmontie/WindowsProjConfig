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
    public partial class DisplayCollection : System.Collections.IEnumerable
    {
        #region Constants
        const String ThisType = "Group Container Iterator";
        #endregion

        #region Variables
        private Display CurrentPage;
        private int CountOfPages;
        private Dictionary<Display, int> PageToIndex;
        private Dictionary<int, Display> IndexToPage;
        private Dictionary<String, Display> NameToPage;
        private List<Display> Pages;
        #endregion

        #region Constructors
        //----------------------------------------------------------------------
        //
        //
        public DisplayCollection(Control ControlInQuestion)
        {
            InitializeComponent();
            PageToIndex = new Dictionary<Display, int>();
            IndexToPage = new Dictionary<int, Display>();
            NameToPage = null;
            Pages = new List<Display>();
        }
        #endregion

        #region Methods
        //----------------------------------------------------------------------
        //
        //
        public void Add(Control NewControl)
        {
            if (NewControl == null)
                throw new ArgumentNullException("value", "Tried to add a null value to the Page Collection.");

            Display NewDisplay = NewControl as Display;
            if (NewDisplay == null)
                throw new ArgumentException("Tried to add a non-Display Page control to the Page Collection", "value");

            NewDisplay.PageId = CountOfPages;
            NewDisplay.PagesIndex = CountOfPages;
            PageToIndex.Add(NewDisplay, NewDisplay.PagesIndex);
            IndexToPage.Add(NewDisplay.PagesIndex, NewDisplay);
            CountOfPages++;
            NewDisplay.SendToBack();
            Pages.Add(NewDisplay);
        }

        //----------------------------------------------------------------------
        //
        //
        internal void Add(ref Display Page)
        {
            if (Page == null)
                throw new ArgumentNullException("value", "Tried to add a null value to the Custom Container Collection.");

            Page.PageId = CountOfPages;
            Page.PagesIndex = CountOfPages;
            PageToIndex.Add(Page, Page.PagesIndex);
            IndexToPage.Add(Page.PagesIndex, Page);
            CountOfPages++;
            Page.SendToBack();
            Pages.Add(Page);
        }

        //----------------------------------------------------------------------
        //
        //
        public void AddRange(Control[] ArrayOfControls)
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
        public void Remove(Control ControlInQuestion)
        {
            if (ControlInQuestion == null)
                throw new ArgumentNullException("null value control", "Found null value object in the Custom Container Collection.");

            Display Page = ControlInQuestion as Display;
            if (Page == null)
                throw new ArgumentException("Found null value object in the Custom Container Collection.", "null value control");

            int PagesIndex = PageToIndex[Page];
            PageToIndex.Remove(Page);
            IndexToPage.Remove(PagesIndex);
            Pages.Remove(Page);
            if (NameToPage != null)
            {
                String Str = Page.Name; // Set defualt Value
                switch (KeyType)
                {
                    case MultiPanel.DisplayGroup.KeyType.Name:
                        Str = Page.Name;
                        break;
                    case MultiPanel.DisplayGroup.KeyType.Text:
                        Str = Page.Text;
                        break;
                    case MultiPanel.DisplayGroup.KeyType.Title:
                        Str = Page.Title;
                        break;

                }
                NameToPage.Remove(Str);
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void GetDisplayNames(ref System.Windows.Forms.ComboBox TheComboBox)
        {
            //if (!InitalizationComplete)
            //    throw new CSIExceptions.CSI_Exception("Not correctly Initialized", "You must call CompleteInitialization before using this Object");

            foreach (Display Page in Pages)
            {
                switch (KeyType)
                {
                    case MultiPanel.DisplayGroup.KeyType.Name:
                        TheComboBox.Items.Add(Page.Name);
                        break;
                    case MultiPanel.DisplayGroup.KeyType.Text:
                        TheComboBox.Items.Add(Page.Text);
                        break;
                    case MultiPanel.DisplayGroup.KeyType.Title:
                        TheComboBox.Items.Add(Page.Title);
                        break;
                }
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public int IndexOfKey(String StringKey)
        {
            int Results = -1;

            //if (!InitalizationComplete)
            //    throw new CSIExceptions.CSI_Exception("Not correctly Initialized", "You must call CompleteInitialization before using this Object");

            if (NameToPage != null)
            {
                Display Dp; // = NameToPage[Key]; // Get the control based on the String 
                if (NameToPage.TryGetValue(StringKey, out Dp))
                {
                    if (PageToIndex != null)
                    {
                        Results = PageToIndex[Dp]; // Get the index based on the Panel
                    }
                }
            }
            return Results;
        }

        //----------------------------------------------------------------------
        //
        //
        public Boolean Contains(String StringKey)
        {
            Display Dp = null;
            Boolean Results = false;

            //if (!InitalizationComplete)
            //    throw new CSIExceptions.CSI_Exception("Not correctly Initialized", "You must call CompleteInitialization before using this Object");

            if (NameToPage.Count > 0)
            {
                if (NameToPage.TryGetValue(StringKey, out Dp))
                    Results = true;
            }
            return Results;
        }

        //----------------------------------------------------------------------
        //
        //
        public Display Select(String StringKey)
        {
            Display Results = null;

           // if (!InitalizationComplete)
           //     throw new CSIExceptions.CSI_Exception("Not correctly Initialized", "You must call CompleteInitialization before using this Object");

            if (NameToPage.Count > 0)
            {
                NameToPage.TryGetValue(StringKey, out Results);
            }
            return Results;
        }

        //----------------------------------------------------------------------
        //
        //
        public Display Select(int PageIndex)
        {
            if (IndexToPage.Count > 0)
                return IndexToPage[PageIndex];

            return null;
        }

        //----------------------------------------------------------------------
        //
        //
        public void SelectPage(String StringKey)
        {
            Display aSelectedPage = null;

            //if (!InitalizationComplete)
            //    throw new CSIExceptions.CSI_Exception("Not correctly Initialized", "You must call CompleteInitialization before using this Object");

            if (NameToPage.Count > 0)
            {
                aSelectedPage = NameToPage[StringKey];
                if (CurrentPage != null)
                {
                    CurrentPage.Visible = false;
                    CurrentPage.SendToBack();
                }
                aSelectedPage.Visible = true;
                CurrentPage = aSelectedPage;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void SelectPage(int PageIndex)
        {
            Display aSelectedPage = null;

            if (IndexToPage.Count > 0)
            {
                if (IndexToPage.TryGetValue(PageIndex, out aSelectedPage))
                {
                    if(CurrentPage != null)
                    {
                        CurrentPage.Visible = false;
                    }                    
                    CurrentPage = aSelectedPage;
                    aSelectedPage.Visible = true;
                }
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void CompleteInitialization(MultiPanel.DisplayGroup.KeyType TheKeyType)
        {
            NameToPage = new Dictionary<String, Display>();
            KeyType = TheKeyType;

            foreach (Display Page in Pages)
            {
                Page.CompleteInitialization();

                switch (KeyType)
                {
                    case MultiPanel.DisplayGroup.KeyType.Name:
                        NameToPage.Add(Page.Name, Page);
                        break;
                    case MultiPanel.DisplayGroup.KeyType.Text:
                        NameToPage.Add(Page.Text, Page);
                        break;
                    case MultiPanel.DisplayGroup.KeyType.Title:
                        NameToPage.Add(Page.Title, Page);
                        break;
                }
            }
            InitalizationComplete = true;
        }

        //----------------------------------------------------------------------
        //
        //
        public System.Collections.IEnumerator GetEnumerator() { return new DisplayEnumerator(this); }
        #endregion

        #region Attributes
        //----------------------------------------------------------------------
        //
        //
        public Boolean HasPages { get { return (Pages.Count > 0); } }

        //----------------------------------------------------------------------
        //
        //
        public Dictionary<Display, int> ToIndex { get { return PageToIndex; } }

        //----------------------------------------------------------------------
        //
        //
        public Dictionary<int, Display> ToPage { get { return IndexToPage; } }

        //----------------------------------------------------------------------
        //
        //
        public String GetPagesName
        {
            get
            {
                if (CurrentPage != null)
                    return CurrentPage.Name;

                return "";
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public int GetPageId
        {
            get
            {
                int Results = 0;
                if (CurrentPage != null)
                    Results = CurrentPage.PageId;

                return Results;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public int SetPageId
        {
            set
            {
                if (CurrentPage != null)
                    CurrentPage.PageId = value;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public int GetPagesIndex
        {
            get
            {
                int Results = 0;
                if (CurrentPage != null)
                    Results = CurrentPage.PagesIndex;

                return Results;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public String SetPageTitle
        {
            set
            {
                if (CurrentPage != null)
                    CurrentPage.Text = value;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public String GetPageName
        {
            get
            {
                String Results = "";
                if (CurrentPage != null)
                    Results = CurrentPage.Name;
                return Results;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public Display SelectedPage { get; set; }

        //----------------------------------------------------------------------
        //
        //
        public Boolean GetSndDisplay(ref Display Dp)
        {
                Boolean Results = false;

                foreach (Display aDisplay in Pages)
                {
                    if(aDisplay.CommunicatonType == Display.Direction.Send)
                    {
                        Dp = aDisplay;
                        Results = true;
                    }
                }
                return Results; 
        }

        //----------------------------------------------------------------------
        //
        //
        public Boolean GetRcvDisplay(ref Display Dp)
        {
            Boolean Results = false;

            foreach (Display aDisplay in Pages)
            {
                if (aDisplay.CommunicatonType == Display.Direction.Receive)
                {
                    Dp = aDisplay;
                    Results = true;
                }
            }
            return Results;
        }

        //----------------------------------------------------------------------
        //
        //
        public String GetThisType { get { return ThisType; } }

        //----------------------------------------------------------------------
        //
        //
        public MultiPanel.DisplayGroup.KeyType KeyType { get; set; }

        //----------------------------------------------------------------------
        //
        //
        private Boolean InitalizationComplete { get; set; }

        //----------------------------------------------------------------------
        //
        //
        public Dictionary<String, Display> GetNameToPage { get { return NameToPage; } }
        #endregion

        #region Private IEnumerator Class
        // Declare an inner class that implements the IEnumerator interface. 
        private class DisplayEnumerator : System.Collections.IEnumerator
        {
            private int CurrentPosition = -1;
            private DisplayCollection TheList;

            //----------------------------------------------------------------------
            //
            //
            public void Reset() { CurrentPosition = 0; }

            //----------------------------------------------------------------------
            //
            //
            public DisplayEnumerator(DisplayCollection DisplaysCollecton) { this.TheList = DisplaysCollecton; }

            //----------------------------------------------------------------------
            //
            //
            // The IEnumerator interface requires a MoveNext method. 
            public bool MoveNext()
            {
                CurrentPosition++;
                if (CurrentPosition < TheList.CountOfPages)
                    return true;
                else
                    return false;
            }

            //----------------------------------------------------------------------
            //
            //
            // The IEnumerator interface requires a Current method. 
            public object Current { get { return TheList.Pages[CurrentPosition]; } }

            //----------------------------------------------------------------------
            //
            //
            public int Position { get { return CurrentPosition; } }
        }
        #endregion
    }
}
