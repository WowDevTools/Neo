namespace Neo.UI.Dialogs
{
    partial class ItemEditor
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
            this.itemEditorControl1 = new Neo.UI.Dialog.ItemEditorControl();
            this.itemEditorControl1.TabControl.SuspendLayout();
            this.itemEditorControl1.SuspendLayout();
            this.SuspendLayout();
            //
            // itemEditorControl1
            //
            this.itemEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.itemEditorControl1.Name = "itemEditorControl1";
            this.itemEditorControl1.Size = new System.Drawing.Size(699, 478);
            //
            // itemEditorControl1.TabControl
            //
            this.itemEditorControl1.TabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.itemEditorControl1.TabControl.ItemSize = new System.Drawing.Size(0, 1);
            this.itemEditorControl1.TabControl.Location = new System.Drawing.Point(156, 3);
            this.itemEditorControl1.TabControl.Name = "TabControl";
            this.itemEditorControl1.TabControl.SelectedIndex = 0;
            this.itemEditorControl1.TabControl.Size = new System.Drawing.Size(545, 479);
            this.itemEditorControl1.TabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.itemEditorControl1.TabControl.TabIndex = 132;
            this.itemEditorControl1.TabIndex = 0;
            //
            // ItemEditor
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 475);
            this.Controls.Add(this.itemEditorControl1);
            this.Name = "ItemEditor";
            this.Text = "Item Editor";
            this.itemEditorControl1.TabControl.ResumeLayout(false);
            this.itemEditorControl1.ResumeLayout(false);
            this.itemEditorControl1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Dialog.ItemEditorControl itemEditorControl1;
    }
}