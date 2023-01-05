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
    public partial class IOBox : UserControl
    {
        private Color BackgroundHighLight;
        private Color ForegroundHighLight;
        private Byte PortValue;

        //----------------------------------------------------------------------
        //
        //
        [System.ComponentModel.Browsable(true),
                System.ComponentModel.Category("IOBox8Bit"),
                System.ComponentModel.Description("The value for the IO Port.")]
        public Byte Value 
        {
            get { return PortValue; }
            set
            {
                PortValue = value;
                try
                {
                    base.Text = Value.ToString();
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.ToString());
                }
                UpdateTextFields();
                ConfigureIOGroup(Value);
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public override Color BackColor 
        {
            get { return base.BackColor; }
            set 
            { 
                base.BackColor = value;
                label1.BackColor = base.BackColor;
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
                label2.BackColor = BackgroundHighLight;
                label3.BackColor = BackgroundHighLight;
                HexPortaLabel.BackColor = BackgroundHighLight;
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
                label1.ForeColor = ForegroundHighLight;
                label2.ForeColor = ForegroundHighLight;
                label3.ForeColor = ForegroundHighLight;
                HexPortaLabel.ForeColor = ForegroundHighLight;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public IOBox()
        {
            InitializeComponent();
        }

        //----------------------------------------------------------------------
        //
        //
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                Value |= 0x01;
            else
                Value &= 0xFE;
            UpdateTextFields();
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                Value |= 0x02;
            else
                Value &= 0xFD;
            UpdateTextFields();
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                Value |= 0x04;
            else
                Value &= 0xFB;
            UpdateTextFields();
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
                Value |= 0x08;
            else
                Value &= 0xF7;
            UpdateTextFields();
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
                Value |= 0x10;
            else
                Value &= 0xEF;
            UpdateTextFields();
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
                Value |= 0x20;
            else
                Value &= 0xDF;
            UpdateTextFields();
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
                Value |= 0x40;
            else
                Value &= 0xBF;
            UpdateTextFields();
        }

        //----------------------------------------------------------------------
        //
        //
        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
                Value |= 0x80;
            else
                Value &= 0x7F;
            UpdateTextFields();
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
                            checkBox1.Checked = true;
                            pictureBox1.Image = global::RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 1:
                            checkBox2.Checked = true;
                            pictureBox2.Image = global::RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 2:
                            checkBox3.Checked = true;
                            pictureBox3.Image = global::RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 3:
                            checkBox4.Checked = true;
                            pictureBox4.Image = global::RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 4:
                            checkBox5.Checked = true;
                            pictureBox5.Image = global::RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 5:
                            checkBox6.Checked = true;
                            pictureBox6.Image = global::RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 6:
                            checkBox7.Checked = true;
                            pictureBox7.Image = global::RegisterControls.Properties.Resources.GreenLed;
                            break;
                        case 7:
                            checkBox8.Checked = true;
                            pictureBox8.Image = global::RegisterControls.Properties.Resources.GreenLed;
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            checkBox1.Checked = false;
                            pictureBox1.Image = global::RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 1:
                            checkBox2.Checked = false;
                            pictureBox2.Image = global::RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 2:
                            checkBox3.Checked = false;
                            pictureBox3.Image = global::RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 3:
                            checkBox4.Checked = false;
                            pictureBox4.Image = global::RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 4:
                            checkBox5.Checked = false;
                            pictureBox5.Image = global::RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 5:
                            checkBox6.Checked = false;
                            pictureBox6.Image = global::RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 6:
                            checkBox7.Checked = false;
                            pictureBox7.Image = global::RegisterControls.Properties.Resources.LedOff;
                            break;
                        case 7:
                            checkBox8.Checked = false;
                            pictureBox8.Image = global::RegisterControls.Properties.Resources.LedOff;
                            break;
                    }
                }
            }
        }

        //----------------------------------------------------------------------
        //
        //
        private void UpdateTextFields()
        {
            UInt16 aValue = Value;
            String hexOutput;

            hexOutput = "0x" + String.Format("{0:X}", aValue);
            HexPortaLabel.Text = hexOutput;
            Base10TextBox.Text = aValue.ToString();
        }

        private void IOBox8Bit_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            UpdateTextFields();
            ConfigureIOGroup(Value);
        }
    }
}
