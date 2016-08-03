namespace WoWEditor6.UI.Dialog
{
    partial class GameObjectEditorControl
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
            this.lbMenu = new System.Windows.Forms.ListBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.tbCombat = new System.Windows.Forms.TabPage();
            this.tbModelInfo = new System.Windows.Forms.TabPage();
            this.label50 = new System.Windows.Forms.Label();
            this.ModelId1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnShowModelId1 = new System.Windows.Forms.Button();
            this.modelRenderControl1 = new WoWEditor6.UI.Components.ModelRenderControl();
            this.tbGameObjectInfo = new System.Windows.Forms.TabPage();
            this.NameCreature = new System.Windows.Forms.TextBox();
            this.Entry = new System.Windows.Forms.TextBox();
            this.ScaleCreature = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.TypeCreature = new System.Windows.Forms.ComboBox();
            this.label23 = new System.Windows.Forms.Label();
            this.tbcEditor = new System.Windows.Forms.TabControl();
            this.tbModelInfo.SuspendLayout();
            this.tbGameObjectInfo.SuspendLayout();
            this.tbcEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbMenu
            // 
            this.lbMenu.FormattingEnabled = true;
            this.lbMenu.Items.AddRange(new object[] {
            "GameObject Info",
            "Model Info",
            "Data"});
            this.lbMenu.Location = new System.Drawing.Point(3, 3);
            this.lbMenu.Name = "lbMenu";
            this.lbMenu.Size = new System.Drawing.Size(150, 121);
            this.lbMenu.TabIndex = 123;
            this.lbMenu.SelectedIndexChanged += new System.EventHandler(this.lbMenu_SelectedIndexChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(3, 130);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(150, 23);
            this.btnSave.TabIndex = 125;
            this.btnSave.Text = "Save and execute to DB";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tbCombat
            // 
            this.tbCombat.Location = new System.Drawing.Point(4, 22);
            this.tbCombat.Name = "tbCombat";
            this.tbCombat.Size = new System.Drawing.Size(537, 453);
            this.tbCombat.TabIndex = 2;
            this.tbCombat.Text = "Data";
            this.tbCombat.UseVisualStyleBackColor = true;
            // 
            // tbModelInfo
            // 
            this.tbModelInfo.Controls.Add(this.label50);
            this.tbModelInfo.Controls.Add(this.ModelId1);
            this.tbModelInfo.Controls.Add(this.label6);
            this.tbModelInfo.Controls.Add(this.btnShowModelId1);
            this.tbModelInfo.Controls.Add(this.modelRenderControl1);
            this.tbModelInfo.Location = new System.Drawing.Point(4, 22);
            this.tbModelInfo.Name = "tbModelInfo";
            this.tbModelInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tbModelInfo.Size = new System.Drawing.Size(537, 453);
            this.tbModelInfo.TabIndex = 1;
            this.tbModelInfo.Text = "Model Info";
            this.tbModelInfo.UseVisualStyleBackColor = true;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(3, 393);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(55, 13);
            this.label50.TabIndex = 24;
            this.label50.Text = "Display ID";
            // 
            // ModelId1
            // 
            this.ModelId1.Location = new System.Drawing.Point(6, 427);
            this.ModelId1.Name = "ModelId1";
            this.ModelId1.Size = new System.Drawing.Size(54, 20);
            this.ModelId1.TabIndex = 13;
            this.ModelId1.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 408);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "1";
            // 
            // btnShowModelId1
            // 
            this.btnShowModelId1.Location = new System.Drawing.Point(66, 427);
            this.btnShowModelId1.Name = "btnShowModelId1";
            this.btnShowModelId1.Size = new System.Drawing.Size(54, 20);
            this.btnShowModelId1.TabIndex = 20;
            this.btnShowModelId1.Text = "Show";
            this.btnShowModelId1.UseVisualStyleBackColor = true;
            this.btnShowModelId1.Click += new System.EventHandler(this.btnShowModelId1_Click);
            // 
            // modelRenderControl1
            // 
            this.modelRenderControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.modelRenderControl1.Location = new System.Drawing.Point(3, 3);
            this.modelRenderControl1.Name = "modelRenderControl1";
            this.modelRenderControl1.Size = new System.Drawing.Size(531, 392);
            this.modelRenderControl1.TabIndex = 1;
            // 
            // tbGameObjectInfo
            // 
            this.tbGameObjectInfo.Controls.Add(this.NameCreature);
            this.tbGameObjectInfo.Controls.Add(this.Entry);
            this.tbGameObjectInfo.Controls.Add(this.ScaleCreature);
            this.tbGameObjectInfo.Controls.Add(this.label11);
            this.tbGameObjectInfo.Controls.Add(this.label1);
            this.tbGameObjectInfo.Controls.Add(this.label38);
            this.tbGameObjectInfo.Controls.Add(this.TypeCreature);
            this.tbGameObjectInfo.Controls.Add(this.label23);
            this.tbGameObjectInfo.Location = new System.Drawing.Point(4, 22);
            this.tbGameObjectInfo.Name = "tbGameObjectInfo";
            this.tbGameObjectInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tbGameObjectInfo.Size = new System.Drawing.Size(537, 453);
            this.tbGameObjectInfo.TabIndex = 0;
            this.tbGameObjectInfo.Text = "GameObject Info";
            this.tbGameObjectInfo.UseVisualStyleBackColor = true;
            // 
            // NameCreature
            // 
            this.NameCreature.BackColor = System.Drawing.SystemColors.Window;
            this.NameCreature.Location = new System.Drawing.Point(73, 32);
            this.NameCreature.Name = "NameCreature";
            this.NameCreature.Size = new System.Drawing.Size(456, 20);
            this.NameCreature.TabIndex = 88;
            // 
            // Entry
            // 
            this.Entry.Location = new System.Drawing.Point(73, 4);
            this.Entry.Name = "Entry";
            this.Entry.Size = new System.Drawing.Size(456, 20);
            this.Entry.TabIndex = 1;
            // 
            // ScaleCreature
            // 
            this.ScaleCreature.BackColor = System.Drawing.SystemColors.Window;
            this.ScaleCreature.Location = new System.Drawing.Point(73, 86);
            this.ScaleCreature.Name = "ScaleCreature";
            this.ScaleCreature.Size = new System.Drawing.Size(456, 20);
            this.ScaleCreature.TabIndex = 94;
            this.ScaleCreature.Text = "1";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(1, 35);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 13);
            this.label11.TabIndex = 87;
            this.label11.Text = "Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Entry";
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(3, 59);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(31, 13);
            this.label38.TabIndex = 101;
            this.label38.Text = "Type";
            // 
            // TypeCreature
            // 
            this.TypeCreature.BackColor = System.Drawing.SystemColors.Window;
            this.TypeCreature.FormattingEnabled = true;
            this.TypeCreature.Items.AddRange(new object[] {
            "None",
            "Beast",
            "Dragonkin",
            "Demon",
            "Elemental",
            "Giant",
            "Undead",
            "Humanoid",
            "Critter",
            "Mechanical",
            "Notspecified",
            "Totem",
            "NonCombatPet",
            "GasCloud"});
            this.TypeCreature.Location = new System.Drawing.Point(73, 59);
            this.TypeCreature.Name = "TypeCreature";
            this.TypeCreature.Size = new System.Drawing.Size(456, 21);
            this.TypeCreature.TabIndex = 102;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(3, 86);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(34, 13);
            this.label23.TabIndex = 93;
            this.label23.Text = "Scale";
            // 
            // tbcEditor
            // 
            this.tbcEditor.Controls.Add(this.tbGameObjectInfo);
            this.tbcEditor.Controls.Add(this.tbModelInfo);
            this.tbcEditor.Controls.Add(this.tbCombat);
            this.tbcEditor.ItemSize = new System.Drawing.Size(55, 18);
            this.tbcEditor.Location = new System.Drawing.Point(156, 3);
            this.tbcEditor.Name = "tbcEditor";
            this.tbcEditor.SelectedIndex = 0;
            this.tbcEditor.Size = new System.Drawing.Size(545, 479);
            this.tbcEditor.TabIndex = 112;
            // 
            // GameObjectEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tbcEditor);
            this.Controls.Add(this.lbMenu);
            this.Name = "GameObjectEditorControl";
            this.Size = new System.Drawing.Size(699, 478);
            this.tbModelInfo.ResumeLayout(false);
            this.tbModelInfo.PerformLayout();
            this.tbGameObjectInfo.ResumeLayout(false);
            this.tbGameObjectInfo.PerformLayout();
            this.tbcEditor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbMenu;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabPage tbCombat;
        private System.Windows.Forms.TabPage tbModelInfo;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.TextBox ModelId1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnShowModelId1;
        private Components.ModelRenderControl modelRenderControl1;
        private System.Windows.Forms.TabPage tbGameObjectInfo;
        private System.Windows.Forms.TextBox NameCreature;
        private System.Windows.Forms.TextBox Entry;
        private System.Windows.Forms.TextBox ScaleCreature;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.ComboBox TypeCreature;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TabControl tbcEditor;
    }
}
