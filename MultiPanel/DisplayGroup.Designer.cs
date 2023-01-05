namespace MultiPanel
{
    partial class DisplayGroup
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DisplayGroup
            // 
            this.UseCompatibleTextRendering = true;
            this.TextChanged += new System.EventHandler(this.DisplayGroup_TextChanged);
            this.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.DisplayGroup_ControlAdded);
            this.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.DisplayGroup_ControlRemoved);
            this.components = new System.ComponentModel.Container();

            this.ResumeLayout(false);
        }
         #endregion
    }
}
