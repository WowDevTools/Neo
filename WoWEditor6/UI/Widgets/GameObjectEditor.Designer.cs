namespace WoWEditor6.UI.Dialogs
{
    partial class GameObjectEditor
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
            this.gameObjectEditorControl1 = new WoWEditor6.UI.Dialog.GameObjectEditorControl();
            this.gameObjectEditorControl1.TabControl.SuspendLayout();
            this.gameObjectEditorControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gameObjectEditorControl1
            // 
            this.gameObjectEditorControl1.Location = new System.Drawing.Point(0, 0);
            this.gameObjectEditorControl1.Name = "gameObjectEditorControl1";
            this.gameObjectEditorControl1.Size = new System.Drawing.Size(699, 478);
            // 
            // gameObjectEditorControl1.TabControl
            // 
            this.gameObjectEditorControl1.TabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.gameObjectEditorControl1.TabControl.ItemSize = new System.Drawing.Size(0, 1);
            this.gameObjectEditorControl1.TabControl.Location = new System.Drawing.Point(156, 3);
            this.gameObjectEditorControl1.TabControl.Name = "TabControl";
            this.gameObjectEditorControl1.TabControl.SelectedIndex = 0;
            this.gameObjectEditorControl1.TabControl.Size = new System.Drawing.Size(545, 479);
            this.gameObjectEditorControl1.TabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.gameObjectEditorControl1.TabControl.TabIndex = 124;
            this.gameObjectEditorControl1.TabIndex = 3;
            // 
            // GameObjectEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 475);
            this.Controls.Add(this.gameObjectEditorControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "GameObjectEditor";
            this.Text = "GameObject Editor";
            this.gameObjectEditorControl1.TabControl.ResumeLayout(false);
            this.gameObjectEditorControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Dialog.GameObjectEditorControl gameObjectEditorControl1;
    }
}