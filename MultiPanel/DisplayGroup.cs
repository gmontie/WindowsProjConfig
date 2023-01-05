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
    public partial class DisplayGroup : System.Windows.Forms.GroupBox
    {
        #region Constants
        const String ThisType = "Display Group Container";
        #endregion

        #region Enumeration
        public enum KeyType
        {
            Name,
            Text,
            Title
        };
        #endregion

        #region Variables
        private DisplayCollection GroupsCollection;
        #endregion

        #region Methods
        //----------------------------------------------------------------------
        //
        //
        public DisplayGroup()
        {
            InitializeComponent();
            GroupsCollection = new DisplayCollection(this);
            GroupsCollection.KeyType = KeyOn;
        }

        //----------------------------------------------------------------------
        //
        //
       //  protected override void OnControlAdded(ControlEventArgs e)
        // {
             //this.Controls.Add(Page);
        //     this.components.Add(Page);
       //  }
        private void DisplayGroup_ControlAdded(object sender, ControlEventArgs e)
        {
            Display Page = e.Control as Display;
            if (Page != null)
            {
                GroupsCollection.Add(ref Page);
            }
        }

        //----------------------------------------------------------------------
        //
        //
        private void DisplayGroup_ControlRemoved(object sender, ControlEventArgs e)
        {
            Display Page = e.Control as Display;
            if (Page != null)
            {
                GroupsCollection.Remove(Page);
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void CompleteInitialization()  {  GroupsCollection.CompleteInitialization(KeyOn);   }

        //----------------------------------------------------------------------
        //
        //
        public void PopulateComboBox(ref System.Windows.Forms.ComboBox TheComboBox)
        {
            foreach (Display Dp in GroupsCollection)
            {
                switch (KeyOn)
                {
                    case KeyType.Name:
                        TheComboBox.Items.Add(Dp.Name);
                        break;
                    case KeyType.Text:
                        TheComboBox.Items.Add(Dp.Text);
                        break;
                    case KeyType.Title:
                        TheComboBox.Items.Add(Dp.Title);
                        break;
                }
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void PopulateListBoxs(System.Windows.Forms.ListBox TheKeyBox, System.Windows.Forms.ListBox TheValueBox)
        {
            Dictionary<String, Display> TheList = GroupsCollection.GetNameToPage;
            foreach (KeyValuePair<String, Display> Pair in TheList)
            {
                String KeyString = Pair.Key;
                Display aPage = Pair.Value;
                TheKeyBox.Items.Add(KeyString);
                TheValueBox.Items.Add(aPage.PagesIndex.ToString());
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void SelectPage(String PageName) { GroupsCollection.SelectPage(PageName); }

        //----------------------------------------------------------------------
        //
        //
        public void SelectPage(int PageIndex)  { GroupsCollection.SelectPage(PageIndex);  }

        //----------------------------------------------------------------------
        //
        //
        private void DisplayGroup_TextChanged(object sender, EventArgs e) {  this.Text = "";  }
        #endregion

        #region Attributes
        //----------------------------------------------------------------------
        //
        //
        public KeyType KeyOn { get; set;   }
        #endregion
    }
}
