namespace WoWEditor6.UI.DbcEditors.Exemple
{
    partial class ExempleEditor
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
            this.exempleEditorControl1 = new WoWEditor6.UI.DbcEditors.Exemple.ExempleEditorControl();
            this.exempleEditorControl1.TabControl.SuspendLayout();
            this.exempleEditorControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // exempleEditorControl1
            // 
            this.exempleEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.exempleEditorControl1.Name = "exempleEditorControl1";
            this.exempleEditorControl1.Size = new System.Drawing.Size(699, 478);
            // 
            // exempleEditorControl1.TabControl
            // 
            this.exempleEditorControl1.TabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.exempleEditorControl1.TabControl.ItemSize = new System.Drawing.Size(0, 1);
            this.exempleEditorControl1.TabControl.Location = new System.Drawing.Point(131, 21);
            this.exempleEditorControl1.TabControl.Name = "TabControl";
            this.exempleEditorControl1.TabControl.SelectedIndex = 0;
            this.exempleEditorControl1.TabControl.Size = new System.Drawing.Size(550, 441);
            this.exempleEditorControl1.TabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.exempleEditorControl1.TabControl.TabIndex = 0;
            this.exempleEditorControl1.TabIndex = 0;
            // 
            // ExempleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 471);
            this.Controls.Add(this.exempleEditorControl1);
            this.Name = "ExempleEditor";
            this.Text = "ExempleEditor";
            this.exempleEditorControl1.TabControl.ResumeLayout(false);
            this.exempleEditorControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ExempleEditorControl exempleEditorControl1;
    }
}