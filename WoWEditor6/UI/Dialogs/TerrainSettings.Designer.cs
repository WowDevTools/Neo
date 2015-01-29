namespace WoWEditor6.UI.Dialogs
{
    partial class TerrainSettings
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
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.label1 = new System.Windows.Forms.Label();
            this.colorPreviewPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(12, 114);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(200, 200);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Vertex Shading Color";
            // 
            // colorPreviewPanel
            // 
            this.colorPreviewPanel.Location = new System.Drawing.Point(219, 133);
            this.colorPreviewPanel.Name = "colorPreviewPanel";
            this.colorPreviewPanel.Size = new System.Drawing.Size(100, 22);
            this.colorPreviewPanel.TabIndex = 2;
            // 
            // TerrainSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 326);
            this.Controls.Add(this.colorPreviewPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.elementHost1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "TerrainSettings";
            this.Text = "Terrain changes";
            this.Load += new System.EventHandler(this.TerrainSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel colorPreviewPanel;
    }
}