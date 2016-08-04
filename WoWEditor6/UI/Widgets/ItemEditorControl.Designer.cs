namespace WoWEditor6.UI.Dialog
{
    partial class ItemEditorControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.LoadButton = new System.Windows.Forms.Button();
            this.LoadEntry = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.lbMenu = new System.Windows.Forms.ListBox();
            this.tbcEditor = new System.Windows.Forms.TabControl();
            this.tbItemInfo = new System.Windows.Forms.TabPage();
            this.tbModelInfo = new System.Windows.Forms.TabPage();
            this.tbCombat = new System.Windows.Forms.TabPage();
            this.tbStat = new System.Windows.Forms.TabPage();
            this.tbRequirement = new System.Windows.Forms.TabPage();
            this.tbSpells = new System.Windows.Forms.TabPage();
            this.tbFlags = new System.Windows.Forms.TabPage();
            this.tbcEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoadButton
            // 
            this.LoadButton.BackColor = System.Drawing.Color.Transparent;
            this.LoadButton.Location = new System.Drawing.Point(79, 159);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(71, 20);
            this.LoadButton.TabIndex = 131;
            this.LoadButton.Text = "Load";
            this.LoadButton.UseVisualStyleBackColor = false;
            // 
            // LoadEntry
            // 
            this.LoadEntry.Location = new System.Drawing.Point(3, 159);
            this.LoadEntry.Name = "LoadEntry";
            this.LoadEntry.Size = new System.Drawing.Size(71, 20);
            this.LoadEntry.TabIndex = 130;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(3, 130);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(150, 23);
            this.btnSave.TabIndex = 129;
            this.btnSave.Text = "Save and execute to DB";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // lbMenu
            // 
            this.lbMenu.FormattingEnabled = true;
            this.lbMenu.Items.AddRange(new object[] {
            "Item Info",
            "Model Info",
            "Combat",
            "Stats",
            "Requirement",
            "Spells",
            "Flags"});
            this.lbMenu.Location = new System.Drawing.Point(3, 3);
            this.lbMenu.Name = "lbMenu";
            this.lbMenu.Size = new System.Drawing.Size(150, 121);
            this.lbMenu.TabIndex = 128;
            // 
            // tbcEditor
            // 
            this.tbcEditor.Controls.Add(this.tbItemInfo);
            this.tbcEditor.Controls.Add(this.tbModelInfo);
            this.tbcEditor.Controls.Add(this.tbCombat);
            this.tbcEditor.Controls.Add(this.tbStat);
            this.tbcEditor.Controls.Add(this.tbRequirement);
            this.tbcEditor.Controls.Add(this.tbSpells);
            this.tbcEditor.Controls.Add(this.tbFlags);
            this.tbcEditor.Location = new System.Drawing.Point(156, 3);
            this.tbcEditor.Name = "tbcEditor";
            this.tbcEditor.SelectedIndex = 0;
            this.tbcEditor.Size = new System.Drawing.Size(545, 479);
            this.tbcEditor.TabIndex = 132;
            // 
            // tbItemInfo
            // 
            this.tbItemInfo.Location = new System.Drawing.Point(4, 22);
            this.tbItemInfo.Name = "tbItemInfo";
            this.tbItemInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tbItemInfo.Size = new System.Drawing.Size(537, 453);
            this.tbItemInfo.TabIndex = 0;
            this.tbItemInfo.Text = "Item Info";
            this.tbItemInfo.UseVisualStyleBackColor = true;
            // 
            // tbModelInfo
            // 
            this.tbModelInfo.Location = new System.Drawing.Point(4, 22);
            this.tbModelInfo.Name = "tbModelInfo";
            this.tbModelInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tbModelInfo.Size = new System.Drawing.Size(537, 453);
            this.tbModelInfo.TabIndex = 1;
            this.tbModelInfo.Text = "Model Info";
            this.tbModelInfo.UseVisualStyleBackColor = true;
            // 
            // tbCombat
            // 
            this.tbCombat.Location = new System.Drawing.Point(4, 22);
            this.tbCombat.Name = "tbCombat";
            this.tbCombat.Size = new System.Drawing.Size(537, 453);
            this.tbCombat.TabIndex = 2;
            this.tbCombat.Text = "Combat";
            this.tbCombat.UseVisualStyleBackColor = true;
            // 
            // tbStat
            // 
            this.tbStat.Location = new System.Drawing.Point(4, 22);
            this.tbStat.Name = "tbStat";
            this.tbStat.Size = new System.Drawing.Size(537, 453);
            this.tbStat.TabIndex = 3;
            this.tbStat.Text = "Stats";
            this.tbStat.UseVisualStyleBackColor = true;
            // 
            // tbRequirement
            // 
            this.tbRequirement.Location = new System.Drawing.Point(4, 22);
            this.tbRequirement.Name = "tbRequirement";
            this.tbRequirement.Size = new System.Drawing.Size(537, 453);
            this.tbRequirement.TabIndex = 4;
            this.tbRequirement.Text = "Requirement";
            this.tbRequirement.UseVisualStyleBackColor = true;
            // 
            // tbSpells
            // 
            this.tbSpells.Location = new System.Drawing.Point(4, 22);
            this.tbSpells.Name = "tbSpells";
            this.tbSpells.Size = new System.Drawing.Size(537, 453);
            this.tbSpells.TabIndex = 5;
            this.tbSpells.Text = "Spells";
            this.tbSpells.UseVisualStyleBackColor = true;
            // 
            // tbFlags
            // 
            this.tbFlags.Location = new System.Drawing.Point(4, 22);
            this.tbFlags.Name = "tbFlags";
            this.tbFlags.Size = new System.Drawing.Size(537, 453);
            this.tbFlags.TabIndex = 6;
            this.tbFlags.Text = "Flags";
            this.tbFlags.UseVisualStyleBackColor = true;
            // 
            // ItemEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbcEditor);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.LoadEntry);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lbMenu);
            this.Name = "ItemEditorControl";
            this.Size = new System.Drawing.Size(699, 478);
            this.tbcEditor.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.TextBox LoadEntry;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ListBox lbMenu;
        private System.Windows.Forms.TabControl tbcEditor;
        private System.Windows.Forms.TabPage tbItemInfo;
        private System.Windows.Forms.TabPage tbModelInfo;
        private System.Windows.Forms.TabPage tbCombat;
        private System.Windows.Forms.TabPage tbStat;
        private System.Windows.Forms.TabPage tbRequirement;
        private System.Windows.Forms.TabPage tbSpells;
        private System.Windows.Forms.TabPage tbFlags;
    }
}
