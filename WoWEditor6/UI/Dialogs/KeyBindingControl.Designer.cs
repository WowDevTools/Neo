namespace WoWEditor6.UI.Dialogs
{
    partial class KeyBindingControl
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
            this.Button = new System.Windows.Forms.Button();
            this.Label = new WoWEditor6.UI.Dialogs.BorderColorLabel();
            this.SuspendLayout();
            // 
            // Button
            // 
            this.Button.Location = new System.Drawing.Point(3, 3);
            this.Button.Name = "Button";
            this.Button.Size = new System.Drawing.Size(117, 23);
            this.Button.TabIndex = 0;
            this.Button.UseVisualStyleBackColor = true;
            // 
            // Label
            // 
            this.Label.AutoEllipsis = true;
            this.Label.BorderColor = System.Drawing.Color.Black;
            this.Label.Location = new System.Drawing.Point(126, 5);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(124, 20);
            this.Label.TabIndex = 1;
            this.Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // KeyBindingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Label);
            this.Controls.Add(this.Button);
            this.Name = "KeyBindingControl";
            this.Size = new System.Drawing.Size(253, 30);
            this.ResumeLayout(false);

        }

        #endregion
        public BorderColorLabel Label;
        public System.Windows.Forms.Button Button;
    }
}
