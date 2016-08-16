namespace WoWEditor6.UI.DbcEditors
{
    partial class ChrRacesEditorControl
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
            this.lbMenu = new System.Windows.Forms.ListBox();
            this.tbcEditor = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // lbMenu
            // 
            this.lbMenu.FormattingEnabled = true;
            this.lbMenu.Location = new System.Drawing.Point(3, 3);
            this.lbMenu.Name = "lbMenu";
            this.lbMenu.Size = new System.Drawing.Size(150, 121);
            this.lbMenu.TabIndex = 123;
            // 
            // tbcEditor
            // 
            this.tbcEditor.Location = new System.Drawing.Point(156, 3);
            this.tbcEditor.Name = "tbcEditor";
            this.tbcEditor.SelectedIndex = 0;
            this.tbcEditor.Size = new System.Drawing.Size(545, 479);
            this.tbcEditor.TabIndex = 124;
            // 
            // ChrRacesEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbcEditor);
            this.Controls.Add(this.lbMenu);
            this.Name = "ChrRacesEditorControl";
            this.Size = new System.Drawing.Size(699, 478);
            this.Load += new System.EventHandler(this.ChrRacesEditorControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbMenu;
        private System.Windows.Forms.TabControl tbcEditor;
    }
}
