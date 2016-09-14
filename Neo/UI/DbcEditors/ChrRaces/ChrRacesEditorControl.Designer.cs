namespace Neo.UI.DbcEditors
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
            this.components = new System.ComponentModel.Container();
            this.lbMenu = new System.Windows.Forms.ListBox();
            this.tbcEditor = new System.Windows.Forms.TabControl();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.AddButton = new System.Windows.Forms.Button();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.UpButton = new System.Windows.Forms.Button();
            this.DownButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbMenu
            // 
            this.lbMenu.FormattingEnabled = true;
            this.lbMenu.Location = new System.Drawing.Point(3, 3);
            this.lbMenu.Name = "lbMenu";
            this.lbMenu.Size = new System.Drawing.Size(150, 342);
            this.lbMenu.TabIndex = 123;
            this.lbMenu.SelectedIndexChanged += new System.EventHandler(this.lbMenu_SelectedIndexChanged);
            // 
            // tbcEditor
            // 
            this.tbcEditor.Location = new System.Drawing.Point(156, 3);
            this.tbcEditor.Name = "tbcEditor";
            this.tbcEditor.SelectedIndex = 0;
            this.tbcEditor.Size = new System.Drawing.Size(545, 479);
            this.tbcEditor.TabIndex = 124;
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(3, 377);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(56, 23);
            this.AddButton.TabIndex = 125;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // RemoveButton
            // 
            this.RemoveButton.Location = new System.Drawing.Point(97, 377);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(56, 23);
            this.RemoveButton.TabIndex = 126;
            this.RemoveButton.Text = "Remove";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 406);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 59);
            this.button1.TabIndex = 127;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // UpButton
            // 
            this.UpButton.Location = new System.Drawing.Point(3, 351);
            this.UpButton.Name = "UpButton";
            this.UpButton.Size = new System.Drawing.Size(56, 23);
            this.UpButton.TabIndex = 128;
            this.UpButton.Text = "↑";
            this.UpButton.UseVisualStyleBackColor = true;
            this.UpButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // DownButton
            // 
            this.DownButton.Location = new System.Drawing.Point(97, 351);
            this.DownButton.Name = "DownButton";
            this.DownButton.Size = new System.Drawing.Size(56, 23);
            this.DownButton.TabIndex = 129;
            this.DownButton.Text = "↓";
            this.DownButton.UseVisualStyleBackColor = true;
            this.DownButton.Click += new System.EventHandler(this.DownButton_Click);
            // 
            // ChrRacesEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DownButton);
            this.Controls.Add(this.UpButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.RemoveButton);
            this.Controls.Add(this.AddButton);
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
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button UpButton;
        private System.Windows.Forms.Button DownButton;
    }
}
