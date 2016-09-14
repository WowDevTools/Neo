namespace WoWEditor6.UI.DbcEditors.ChrRaces
{
    partial class ChrRacesEditor
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chrRacesEditorControl1 = new WoWEditor6.UI.DbcEditors.ChrRacesEditorControl();
            this.SuspendLayout();
            // 
            // chrRacesEditorControl1
            // 
            this.chrRacesEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.chrRacesEditorControl1.Name = "chrRacesEditorControl1";
            this.chrRacesEditorControl1.Size = new System.Drawing.Size(699, 478);
            this.chrRacesEditorControl1.TabIndex = 0;
            // 
            // ChrRacesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 471);
            this.Controls.Add(this.chrRacesEditorControl1);
            this.Name = "ChrRacesEditor";
            this.Text = "ChrRacesEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private ChrRacesEditorControl chrRacesEditorControl1;
    }
}