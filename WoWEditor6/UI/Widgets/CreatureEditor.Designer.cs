namespace WoWEditor6.UI.Dialogs
{
    partial class CreatureEditor
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
            this.creatureEditorControl1 = new WoWEditor6.UI.Dialogs.CreatureEditorControl();
            this.creatureEditorControl1.TabControl.SuspendLayout();
            this.creatureEditorControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // creatureEditorControl1
            // 
            this.creatureEditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.creatureEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.creatureEditorControl1.Name = "creatureEditorControl1";
            this.creatureEditorControl1.Size = new System.Drawing.Size(1251, 558);
            // 
            // creatureEditorControl1.TabControl
            // 
            this.creatureEditorControl1.TabControl.Location = new System.Drawing.Point(587, 29);
            this.creatureEditorControl1.TabControl.Name = "TabControl";
            this.creatureEditorControl1.TabControl.SelectedIndex = 0;
            this.creatureEditorControl1.TabControl.Size = new System.Drawing.Size(663, 399);
            this.creatureEditorControl1.TabControl.TabIndex = 108;
            this.creatureEditorControl1.TabIndex = 0;
            // 
            // CreatureEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1251, 558);
            this.Controls.Add(this.creatureEditorControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CreatureEditor";
            this.Text = "Creature Editor";
            this.creatureEditorControl1.TabControl.ResumeLayout(false);
            this.creatureEditorControl1.ResumeLayout(false);
            this.creatureEditorControl1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private CreatureEditorControl creatureEditorControl1;
    }
}