using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RegisterControls
{
    public partial class IOBox8Bit : UserControl
    {
        private Color BackgroundHighLight = Color.DarkBlue;
        private Color ForegroundHighLight = Color.Moccasin;
        private Color LabelsBackgound = Color.Black;
        private KeysConverter kc = new KeysConverter();
        private String TempText;
        private String AuxStr;
        private UInt16 InternalValue;
        private KeyValue KvPair;
        private Action<KeyValue> CallBack;

        //----------------------------------------------------------------------
        //
        //
        public IOBox8Bit()
        {
            InitializeComponent();
            InternalValue = 0;
            ConfigureIOGroup(InternalValue);
            UpdateTextFields();
            TitleLabel.Text = "IO Port Values @[?]";
        }

        //----------------------------------------------------------------------
        //
        //
        internal void SetCallBack(Action<KeyValue> Act)
        {
            CallBack = Act;
        }

        //----------------------------------------------------------------------
        //
        //
        [System.ComponentModel.Browsable(true),
                System.ComponentModel.Category("IOBox8Bit"),
                System.ComponentModel.Description("The value for the IO Port.")]
        public UInt16 Value 
        {
            get { return InternalValue; }
            set
            {
                if (InternalValue != value)
                {
                    InternalValue = value;
                    try
                    {
                        base.Text = InternalValue.ToString();
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.ToString());
                    }

                    ConfigureIOGroup(InternalValue);
                }
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public Byte Address 
        { 
            get { return KvPair.Address; } 
            set 
            {
                KvPair = new KeyValue(value, InternalValue);
                TitleLabel.Text = "IO Port Values @[" + KvPair.AddressStr + "]";
                
            } 
        }

        //----------------------------------------------------------------------
        //
        //
        private void UpdateTextFields()
        {
            String hexOutput;

            hexOutput = "0x" + String.Format("{0:X2}", InternalValue);
            HexPortaLabel.Text = hexOutput;
            Base10TextBox.Text = InternalValue.ToString();
        }

        #region Colors
        //----------------------------------------------------------------------
        //
        //
        public override Color BackColor 
        {
            get { return base.BackColor; }
            set 
            { 
                base.BackColor = value;
                TitleLabel.BackColor = base.BackColor;
                Base10TextBox.BackColor = base.BackColor;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public override Color ForeColor 
        {
            get { return base.ForeColor; }
            set 
            { 
                base.ForeColor = value;
                Base10TextBox.ForeColor = base.ForeColor;
                vertical8.ForeColor = base.ForeColor;
                vertical7.ForeColor = base.ForeColor;
                vertical6.ForeColor = base.ForeColor;
                vertical5.ForeColor = base.ForeColor;
                vertical4.ForeColor = base.ForeColor;
                vertical3.ForeColor = base.ForeColor;
                vertical2.ForeColor = base.ForeColor;
                vertical1.ForeColor = base.ForeColor;
                Base10TextBox.ForeColor = base.ForeColor;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public Color HighlightBackground 
        {
            get
            {
                return BackgroundHighLight;
            }
            set
            {
                BackgroundHighLight = value;
                panel1.BackColor = BackgroundHighLight;
                panel2.BackColor = BackgroundHighLight;
                panel3.BackColor = BackgroundHighLight;
                panel4.BackColor = BackgroundHighLight;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public Color DisplayTextForground 
        {
            get { return ForegroundHighLight; }
            set
            {
                ForegroundHighLight = value;
                TitleLabel.ForeColor = ForegroundHighLight;
                label2.ForeColor = ForegroundHighLight;
                label3.ForeColor = ForegroundHighLight;
                HexPortaLabel.ForeColor = ForegroundHighLight;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public Color LabelBackground
        {
            get{return LabelsBackgound;}
            set
            {
                LabelsBackgound = value;
                label2.BackColor = LabelsBackgound;
                label3.BackColor = LabelsBackgound;
                HexPortaLabel.BackColor = LabelsBackgound;
            }
        }
        #endregion

        //----------------------------------------------------------------------
        //
        //
        private void UpdateControl(UInt16 NewValue)
        {
            InternalValue = NewValue;
            KvPair.Value = NewValue;
            CallBack.Invoke(KvPair);
            ConfigureIOGroup(InternalValue);
            UpdateTextFields();
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                UpdateControl((UInt16)(InternalValue | 0x0001));
            else
                UpdateControl((UInt16)(InternalValue & 0x00FE));
        }


        //----------------------------------------------------------------------
        //
        //
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                UpdateControl((UInt16)(InternalValue | 0x0002));
            else
                UpdateControl((UInt16)(InternalValue & 0x00FD));
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                UpdateControl((UInt16)(InternalValue | 0x0004));
            else
                UpdateControl((UInt16)(InternalValue &= 0x00FB));
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
                UpdateControl((UInt16)(InternalValue | 0x0008));
            else
                UpdateControl((UInt16)(InternalValue & 0x00F7));
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
                UpdateControl((UInt16)(InternalValue | 0x0010));
            else
                UpdateControl((UInt16)(InternalValue & 0x00EF));
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
                UpdateControl((UInt16)(InternalValue | 0x0020));
            else
                UpdateControl((UInt16)(InternalValue & 0x00DF));
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
                UpdateControl((UInt16)(InternalValue | 0x0040));
            else
                UpdateControl((UInt16)(InternalValue & 0x00BF));
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
                UpdateControl((UInt16)(InternalValue | 0x0080));
            else
                UpdateControl((UInt16)(InternalValue & 0x007F));
        }

        //----------------------------------------------------------------------
        //
        //
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
                checkBox1.Checked = false;
            else
                checkBox1.Checked = true;
        }

        //----------------------------------------------------------------------
        //
        //
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
                checkBox2.Checked = false;
            else
                checkBox2.Checked = true;
        }

        //----------------------------------------------------------------------
        //
        //
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
                checkBox3.Checked = false;
            else
                checkBox3.Checked = true;
        }

        //----------------------------------------------------------------------
        //
        //
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
                checkBox4.Checked = false;
            else
                checkBox4.Checked = true;
        }

        //----------------------------------------------------------------------
        //
        //
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (checkBox5.Checked == true)
                checkBox5.Checked = false;
            else
                checkBox5.Checked = true;
        }

        //----------------------------------------------------------------------
        //
        //
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (checkBox6.Checked == true)
                checkBox6.Checked = false;
            else
                checkBox6.Checked = true;
        }

        //----------------------------------------------------------------------
        //
        //
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            if (checkBox7.Checked == true)
                checkBox7.Checked = false;
            else
                checkBox7.Checked = true;
        }

        //----------------------------------------------------------------------
        //
        //
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            if (checkBox8.Checked == true)
                checkBox8.Checked = false;
            else
                checkBox8.Checked = true;
        }

        //----------------------------------------------------------------------
        //
        //
        private void ConfigureIOGroup(int Mask)
        {
            int i;
            for (i = 0; i < 8; i++)
            {
                if ((Mask & (1 << i)) != 0)
                {
                    switch (i)
                    {
                        case 0:
                            pictureBox1.Image = RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 1:
                            pictureBox2.Image = RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 2:
                            pictureBox3.Image = RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 3:
                            pictureBox4.Image = RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 4:
                            pictureBox5.Image = RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 5:
                            pictureBox6.Image = RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 6:
                            pictureBox7.Image = RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 7:
                            pictureBox8.Image = RegisterControls.Properties.Resources.GreenLed;
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            pictureBox1.Image = RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 1:
                            pictureBox2.Image = RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 2:
                            pictureBox3.Image = RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 3:
                            pictureBox4.Image = RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 4:
                            pictureBox5.Image = RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 5:
                            pictureBox6.Image = RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 6:
                            pictureBox7.Image = RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 7:
                            pictureBox8.Image = RegisterControls.Properties.Resources.LedOff;
                            break;
                    }
                }
            }
        }

        //----------------------------------------------------------------------
        //
        //
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode != Keys.Return)
            {
                switch(e.KeyValue)
                {
                    case 48: TempText += '0'; break;
                    case 49: TempText += '1'; break;
                    case 50: TempText += '2'; break;
                    case 51: TempText += '3'; break;
                    case 52: TempText += '4'; break;
                    case 53: TempText += '5'; break;
                    case 54: TempText += '6'; break;
                    case 55: TempText += '7'; break;
                    case 56: TempText += '8'; break;
                    case 57: TempText += '9'; break;
                }
            }
            else
            {
                try
                {
                    UInt16 Nv = Convert.ToUInt16(TempText);
                    UpdateControl(Nv);
                }
                catch (FormatException Ex)
                {
                    MessageBox.Show(Ex.ToString(), TempText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch(Exception Ex)
                {
                    MessageBox.Show(Ex.ToString(), TempText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                TempText = "";
            }
        }

        //----------------------------------------------------------------------
        //
        //
        private void OnEnter(object sender, EventArgs e)
        {
            AuxStr = Base10TextBox.Text;
        }

        //----------------------------------------------------------------------
        //
        //
        private void OnLeave(object sender, EventArgs e)
        {
            if (Base10TextBox.Text != TempText)
                Base10TextBox.Text = AuxStr;
        }
    }
}
