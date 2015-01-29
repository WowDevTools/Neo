namespace WoWEditor6.UI.Dialogs
{
    partial class InputSettings
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
            this.components = new System.ComponentModel.Container();
            this.invertMouseBox = new System.Windows.Forms.CheckBox();
            this.bindingPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.helpButton = new System.Windows.Forms.Button();
            this.bindingToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // invertMouseBox
            // 
            this.invertMouseBox.AutoSize = true;
            this.invertMouseBox.Location = new System.Drawing.Point(13, 13);
            this.invertMouseBox.Name = "invertMouseBox";
            this.invertMouseBox.Size = new System.Drawing.Size(87, 17);
            this.invertMouseBox.TabIndex = 0;
            this.invertMouseBox.Text = "Invert mouse";
            this.invertMouseBox.UseVisualStyleBackColor = true;
            // 
            // bindingPanel
            // 
            this.bindingPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bindingPanel.Location = new System.Drawing.Point(0, 50);
            this.bindingPanel.Name = "bindingPanel";
            this.bindingPanel.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.bindingPanel.Size = new System.Drawing.Size(385, 320);
            this.bindingPanel.TabIndex = 1;
            // 
            // helpButton
            // 
            this.helpButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.helpButton.Location = new System.Drawing.Point(349, 21);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(24, 23);
            this.helpButton.TabIndex = 2;
            this.helpButton.Text = "?";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // bindingToolTip
            // 
            this.bindingToolTip.IsBalloon = true;
            this.bindingToolTip.ToolTipTitle = "Setting Keybindings";
            // 
            // InputSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 370);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.bindingPanel);
            this.Controls.Add(this.invertMouseBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.Name = "InputSettings";
            this.Text = "Keyboard & Mouse Settings";
            this.Load += new System.EventHandler(this.InputSettings_Load);
            this.Resize += new System.EventHandler(this.InputSettings_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox invertMouseBox;
        private System.Windows.Forms.FlowLayoutPanel bindingPanel;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.ToolTip bindingToolTip;
    }
}