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
            this.components = new System.ComponentModel.Container();
            this.lbMenu = new System.Windows.Forms.ListBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.tbData = new System.Windows.Forms.TabPage();
            this.tbModelInfo = new System.Windows.Forms.TabPage();
            this.label50 = new System.Windows.Forms.Label();
            this.ModelId = new System.Windows.Forms.TextBox();
            this.btnShowModelId1 = new System.Windows.Forms.Button();
            this.modelRenderControl1 = new WoWEditor6.UI.Components.ModelRenderControl();
            this.tbGameObjectInfo = new System.Windows.Forms.TabPage();
            this.GameObjectName = new System.Windows.Forms.TextBox();
            this.Entry = new System.Windows.Forms.TextBox();
            this.GameObjectSize = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.Type = new System.Windows.Forms.ComboBox();
            this.label23 = new System.Windows.Forms.Label();
            this.tbcEditor = new System.Windows.Forms.TabControl();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbOptional = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Faction = new System.Windows.Forms.TextBox();
            this.Flags = new System.Windows.Forms.CheckedListBox();
            this.CastBarCaption = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.tbData2 = new System.Windows.Forms.TabPage();
            this.Data0 = new System.Windows.Forms.ComboBox();
            this.Data1 = new System.Windows.Forms.ComboBox();
            this.Data2 = new System.Windows.Forms.ComboBox();
            this.Data3 = new System.Windows.Forms.ComboBox();
            this.Data4 = new System.Windows.Forms.ComboBox();
            this.Data5 = new System.Windows.Forms.ComboBox();
            this.Data6 = new System.Windows.Forms.ComboBox();
            this.Data7 = new System.Windows.Forms.ComboBox();
            this.Data8 = new System.Windows.Forms.ComboBox();
            this.Data9 = new System.Windows.Forms.ComboBox();
            this.Data10 = new System.Windows.Forms.ComboBox();
            this.Data11 = new System.Windows.Forms.ComboBox();
            this.Data23 = new System.Windows.Forms.ComboBox();
            this.Data22 = new System.Windows.Forms.ComboBox();
            this.Data21 = new System.Windows.Forms.ComboBox();
            this.Data20 = new System.Windows.Forms.ComboBox();
            this.Data19 = new System.Windows.Forms.ComboBox();
            this.Data18 = new System.Windows.Forms.ComboBox();
            this.Data17 = new System.Windows.Forms.ComboBox();
            this.Data16 = new System.Windows.Forms.ComboBox();
            this.Data15 = new System.Windows.Forms.ComboBox();
            this.Data14 = new System.Windows.Forms.ComboBox();
            this.Data13 = new System.Windows.Forms.ComboBox();
            this.Data12 = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.IconeName = new System.Windows.Forms.ComboBox();
            this.ScriptName = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.AiName = new System.Windows.Forms.TextBox();
            this.tbData.SuspendLayout();
            this.tbModelInfo.SuspendLayout();
            this.tbGameObjectInfo.SuspendLayout();
            this.tbcEditor.SuspendLayout();
            this.tbOptional.SuspendLayout();
            this.tbData2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbMenu
            // 
            this.lbMenu.FormattingEnabled = true;
            this.lbMenu.Items.AddRange(new object[] {
            "GameObject Info",
            "Model Info",
            "Data",
            "Data2",
            "Optional"});
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
            // tbData
            // 
            this.tbData.Controls.Add(this.Data11);
            this.tbData.Controls.Add(this.Data10);
            this.tbData.Controls.Add(this.Data9);
            this.tbData.Controls.Add(this.Data8);
            this.tbData.Controls.Add(this.Data7);
            this.tbData.Controls.Add(this.Data6);
            this.tbData.Controls.Add(this.Data5);
            this.tbData.Controls.Add(this.Data4);
            this.tbData.Controls.Add(this.Data3);
            this.tbData.Controls.Add(this.Data2);
            this.tbData.Controls.Add(this.Data1);
            this.tbData.Controls.Add(this.Data0);
            this.tbData.Controls.Add(this.label18);
            this.tbData.Controls.Add(this.label17);
            this.tbData.Controls.Add(this.label16);
            this.tbData.Controls.Add(this.label15);
            this.tbData.Controls.Add(this.label14);
            this.tbData.Controls.Add(this.label13);
            this.tbData.Controls.Add(this.label12);
            this.tbData.Controls.Add(this.label10);
            this.tbData.Controls.Add(this.label9);
            this.tbData.Controls.Add(this.label8);
            this.tbData.Controls.Add(this.label7);
            this.tbData.Controls.Add(this.label6);
            this.tbData.Location = new System.Drawing.Point(4, 22);
            this.tbData.Name = "tbData";
            this.tbData.Size = new System.Drawing.Size(537, 453);
            this.tbData.TabIndex = 2;
            this.tbData.Text = "Data";
            this.tbData.UseVisualStyleBackColor = true;
            // 
            // tbModelInfo
            // 
            this.tbModelInfo.Controls.Add(this.label50);
            this.tbModelInfo.Controls.Add(this.ModelId);
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
            this.toolTip1.SetToolTip(this.label50, "Graphic model id sent to the client.");
            // 
            // ModelId
            // 
            this.ModelId.Location = new System.Drawing.Point(6, 427);
            this.ModelId.Name = "ModelId";
            this.ModelId.Size = new System.Drawing.Size(54, 20);
            this.ModelId.TabIndex = 13;
            this.ModelId.Text = "0";
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
            this.tbGameObjectInfo.Controls.Add(this.Flags);
            this.tbGameObjectInfo.Controls.Add(this.Faction);
            this.tbGameObjectInfo.Controls.Add(this.label3);
            this.tbGameObjectInfo.Controls.Add(this.label2);
            this.tbGameObjectInfo.Controls.Add(this.GameObjectName);
            this.tbGameObjectInfo.Controls.Add(this.Entry);
            this.tbGameObjectInfo.Controls.Add(this.GameObjectSize);
            this.tbGameObjectInfo.Controls.Add(this.label11);
            this.tbGameObjectInfo.Controls.Add(this.label1);
            this.tbGameObjectInfo.Controls.Add(this.label38);
            this.tbGameObjectInfo.Controls.Add(this.Type);
            this.tbGameObjectInfo.Controls.Add(this.label23);
            this.tbGameObjectInfo.Location = new System.Drawing.Point(4, 22);
            this.tbGameObjectInfo.Name = "tbGameObjectInfo";
            this.tbGameObjectInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tbGameObjectInfo.Size = new System.Drawing.Size(537, 453);
            this.tbGameObjectInfo.TabIndex = 0;
            this.tbGameObjectInfo.Text = "GameObject Info";
            this.tbGameObjectInfo.UseVisualStyleBackColor = true;
            // 
            // GameObjectName
            // 
            this.GameObjectName.BackColor = System.Drawing.SystemColors.Window;
            this.GameObjectName.Location = new System.Drawing.Point(73, 37);
            this.GameObjectName.Name = "GameObjectName";
            this.GameObjectName.Size = new System.Drawing.Size(456, 20);
            this.GameObjectName.TabIndex = 88;
            // 
            // Entry
            // 
            this.Entry.Location = new System.Drawing.Point(73, 7);
            this.Entry.Name = "Entry";
            this.Entry.Size = new System.Drawing.Size(456, 20);
            this.Entry.TabIndex = 1;
            // 
            // GameObjectSize
            // 
            this.GameObjectSize.BackColor = System.Drawing.SystemColors.Window;
            this.GameObjectSize.Location = new System.Drawing.Point(73, 97);
            this.GameObjectSize.Name = "GameObjectSize";
            this.GameObjectSize.Size = new System.Drawing.Size(456, 20);
            this.GameObjectSize.TabIndex = 94;
            this.GameObjectSize.Text = "1";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(1, 37);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 13);
            this.label11.TabIndex = 87;
            this.label11.Text = "Name";
            this.toolTip1.SetToolTip(this.label11, "Object\'s name.");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Entry";
            this.toolTip1.SetToolTip(this.label1, "Id of the gameobject template.");
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(1, 67);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(31, 13);
            this.label38.TabIndex = 101;
            this.label38.Text = "Type";
            this.toolTip1.SetToolTip(this.label38, "Object\'s type.");
            // 
            // Type
            // 
            this.Type.BackColor = System.Drawing.SystemColors.Window;
            this.Type.FormattingEnabled = true;
            this.Type.Items.AddRange(new object[] {
            "Door",
            "Button",
            "QuestGiver",
            "Chest",
            "Binder",
            "Generic",
            "Trap",
            "Chair",
            "SpellFocus",
            "Text",
            "Goober",
            "Transport",
            "AreaDamage",
            "Camera",
            "MapObject",
            "MoTransport",
            "DuelArbiter",
            "FishingNode",
            "Ritual",
            "Mailbox",
            "AuctionHouse",
            "Guardpost",
            "Spellcaster",
            "MeetingStone",
            "FlagStand",
            "FishingHole",
            "FlagDrop",
            "MiniGame",
            "LotteryKiosk",
            "CapturePoint",
            "AuraGenerator",
            "DungeonDifficulty",
            "BarberChair",
            "DestructibleBuilding",
            "GuildBank",
            "TrapDoor"});
            this.Type.Location = new System.Drawing.Point(73, 67);
            this.Type.Name = "Type";
            this.Type.Size = new System.Drawing.Size(456, 21);
            this.Type.TabIndex = 102;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(1, 97);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(27, 13);
            this.label23.TabIndex = 93;
            this.label23.Text = "Size";
            this.toolTip1.SetToolTip(this.label23, "Object\'s size must be set because graphic models can be resample.");
            // 
            // tbcEditor
            // 
            this.tbcEditor.Controls.Add(this.tbGameObjectInfo);
            this.tbcEditor.Controls.Add(this.tbModelInfo);
            this.tbcEditor.Controls.Add(this.tbData);
            this.tbcEditor.Controls.Add(this.tbData2);
            this.tbcEditor.Controls.Add(this.tbOptional);
            this.tbcEditor.ItemSize = new System.Drawing.Size(55, 18);
            this.tbcEditor.Location = new System.Drawing.Point(156, 3);
            this.tbcEditor.Name = "tbcEditor";
            this.tbcEditor.SelectedIndex = 0;
            this.tbcEditor.Size = new System.Drawing.Size(545, 479);
            this.tbcEditor.TabIndex = 112;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 103;
            this.label2.Text = "Faction";
            this.toolTip1.SetToolTip(this.label2, "Object\'s faction, if any.");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 104;
            this.label3.Text = "Flags";
            this.toolTip1.SetToolTip(this.label3, "Object\'s flags.");
            // 
            // tbOptional
            // 
            this.tbOptional.Controls.Add(this.AiName);
            this.tbOptional.Controls.Add(this.ScriptName);
            this.tbOptional.Controls.Add(this.label32);
            this.tbOptional.Controls.Add(this.label33);
            this.tbOptional.Controls.Add(this.IconeName);
            this.tbOptional.Controls.Add(this.CastBarCaption);
            this.tbOptional.Controls.Add(this.label5);
            this.tbOptional.Controls.Add(this.label4);
            this.tbOptional.Location = new System.Drawing.Point(4, 22);
            this.tbOptional.Name = "tbOptional";
            this.tbOptional.Padding = new System.Windows.Forms.Padding(3);
            this.tbOptional.Size = new System.Drawing.Size(537, 453);
            this.tbOptional.TabIndex = 3;
            this.tbOptional.Text = "Optional";
            this.tbOptional.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 108;
            this.label5.Text = "CastBarCaption";
            this.toolTip1.SetToolTip(this.label5, "Shows unique text in the object\'s casting bar when the object is used.");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 107;
            this.label4.Text = "IconeName";
            this.toolTip1.SetToolTip(this.label4, "Works exactly like creature_template IconName.");
            // 
            // Faction
            // 
            this.Faction.Location = new System.Drawing.Point(73, 127);
            this.Faction.Name = "Faction";
            this.Faction.Size = new System.Drawing.Size(456, 20);
            this.Faction.TabIndex = 105;
            // 
            // Flags
            // 
            this.Flags.FormattingEnabled = true;
            this.Flags.Items.AddRange(new object[] {
            "InUse",
            "Locked",
            "InteractCond",
            "Transport",
            "NotSelectable",
            "NoDespawn",
            "Triggered",
            "Damaged",
            "Destroyed"});
            this.Flags.Location = new System.Drawing.Point(73, 157);
            this.Flags.Name = "Flags";
            this.Flags.Size = new System.Drawing.Size(456, 64);
            this.Flags.TabIndex = 106;
            // 
            // CastBarCaption
            // 
            this.CastBarCaption.Location = new System.Drawing.Point(87, 37);
            this.CastBarCaption.Name = "CastBarCaption";
            this.CastBarCaption.Size = new System.Drawing.Size(444, 20);
            this.CastBarCaption.TabIndex = 110;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Data0";
            this.toolTip1.SetToolTip(this.label6, "The content of the data fields depends on the gameobject type.");
            this.label6.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Data1";
            this.label7.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1, 77);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Data2";
            this.label8.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1, 107);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Data3";
            this.label9.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1, 137);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Data4";
            this.label10.Visible = false;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(1, 167);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(36, 13);
            this.label12.TabIndex = 5;
            this.label12.Text = "Data5";
            this.label12.Visible = false;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(1, 197);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(36, 13);
            this.label13.TabIndex = 6;
            this.label13.Text = "Data6";
            this.label13.Visible = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(1, 227);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(36, 13);
            this.label14.TabIndex = 7;
            this.label14.Text = "Data7";
            this.label14.Visible = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(1, 257);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(36, 13);
            this.label15.TabIndex = 8;
            this.label15.Text = "Data8";
            this.label15.Visible = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(1, 287);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(36, 13);
            this.label16.TabIndex = 9;
            this.label16.Text = "Data9";
            this.label16.Visible = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(1, 317);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(42, 13);
            this.label17.TabIndex = 10;
            this.label17.Text = "Data10";
            this.label17.Visible = false;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(1, 347);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(42, 13);
            this.label18.TabIndex = 11;
            this.label18.Text = "Data11";
            this.label18.Visible = false;
            // 
            // tbData2
            // 
            this.tbData2.Controls.Add(this.Data23);
            this.tbData2.Controls.Add(this.Data22);
            this.tbData2.Controls.Add(this.Data21);
            this.tbData2.Controls.Add(this.Data20);
            this.tbData2.Controls.Add(this.Data19);
            this.tbData2.Controls.Add(this.Data18);
            this.tbData2.Controls.Add(this.Data17);
            this.tbData2.Controls.Add(this.Data16);
            this.tbData2.Controls.Add(this.Data15);
            this.tbData2.Controls.Add(this.Data14);
            this.tbData2.Controls.Add(this.Data13);
            this.tbData2.Controls.Add(this.Data12);
            this.tbData2.Controls.Add(this.label19);
            this.tbData2.Controls.Add(this.label20);
            this.tbData2.Controls.Add(this.label21);
            this.tbData2.Controls.Add(this.label22);
            this.tbData2.Controls.Add(this.label24);
            this.tbData2.Controls.Add(this.label25);
            this.tbData2.Controls.Add(this.label26);
            this.tbData2.Controls.Add(this.label27);
            this.tbData2.Controls.Add(this.label28);
            this.tbData2.Controls.Add(this.label29);
            this.tbData2.Controls.Add(this.label30);
            this.tbData2.Controls.Add(this.label31);
            this.tbData2.Location = new System.Drawing.Point(4, 22);
            this.tbData2.Name = "tbData2";
            this.tbData2.Padding = new System.Windows.Forms.Padding(3);
            this.tbData2.Size = new System.Drawing.Size(537, 453);
            this.tbData2.TabIndex = 4;
            this.tbData2.Text = "Data2";
            this.tbData2.UseVisualStyleBackColor = true;
            // 
            // Data0
            // 
            this.Data0.FormattingEnabled = true;
            this.Data0.Location = new System.Drawing.Point(73, 17);
            this.Data0.Name = "Data0";
            this.Data0.Size = new System.Drawing.Size(456, 21);
            this.Data0.TabIndex = 12;
            this.Data0.Visible = false;
            // 
            // Data1
            // 
            this.Data1.FormattingEnabled = true;
            this.Data1.Location = new System.Drawing.Point(73, 47);
            this.Data1.Name = "Data1";
            this.Data1.Size = new System.Drawing.Size(456, 21);
            this.Data1.TabIndex = 13;
            this.Data1.Visible = false;
            // 
            // Data2
            // 
            this.Data2.FormattingEnabled = true;
            this.Data2.Location = new System.Drawing.Point(73, 78);
            this.Data2.Name = "Data2";
            this.Data2.Size = new System.Drawing.Size(456, 21);
            this.Data2.TabIndex = 14;
            this.Data2.Visible = false;
            // 
            // Data3
            // 
            this.Data3.FormattingEnabled = true;
            this.Data3.Location = new System.Drawing.Point(73, 105);
            this.Data3.Name = "Data3";
            this.Data3.Size = new System.Drawing.Size(456, 21);
            this.Data3.TabIndex = 15;
            this.Data3.Visible = false;
            // 
            // Data4
            // 
            this.Data4.FormattingEnabled = true;
            this.Data4.Location = new System.Drawing.Point(73, 137);
            this.Data4.Name = "Data4";
            this.Data4.Size = new System.Drawing.Size(456, 21);
            this.Data4.TabIndex = 16;
            this.Data4.Visible = false;
            // 
            // Data5
            // 
            this.Data5.FormattingEnabled = true;
            this.Data5.Location = new System.Drawing.Point(73, 167);
            this.Data5.Name = "Data5";
            this.Data5.Size = new System.Drawing.Size(456, 21);
            this.Data5.TabIndex = 17;
            this.Data5.Visible = false;
            // 
            // Data6
            // 
            this.Data6.FormattingEnabled = true;
            this.Data6.Location = new System.Drawing.Point(73, 197);
            this.Data6.Name = "Data6";
            this.Data6.Size = new System.Drawing.Size(456, 21);
            this.Data6.TabIndex = 18;
            this.Data6.Visible = false;
            // 
            // Data7
            // 
            this.Data7.FormattingEnabled = true;
            this.Data7.Location = new System.Drawing.Point(73, 227);
            this.Data7.Name = "Data7";
            this.Data7.Size = new System.Drawing.Size(456, 21);
            this.Data7.TabIndex = 19;
            this.Data7.Visible = false;
            // 
            // Data8
            // 
            this.Data8.FormattingEnabled = true;
            this.Data8.Location = new System.Drawing.Point(73, 257);
            this.Data8.Name = "Data8";
            this.Data8.Size = new System.Drawing.Size(456, 21);
            this.Data8.TabIndex = 20;
            this.Data8.Visible = false;
            // 
            // Data9
            // 
            this.Data9.FormattingEnabled = true;
            this.Data9.Location = new System.Drawing.Point(73, 287);
            this.Data9.Name = "Data9";
            this.Data9.Size = new System.Drawing.Size(456, 21);
            this.Data9.TabIndex = 21;
            this.Data9.Visible = false;
            // 
            // Data10
            // 
            this.Data10.FormattingEnabled = true;
            this.Data10.Location = new System.Drawing.Point(73, 317);
            this.Data10.Name = "Data10";
            this.Data10.Size = new System.Drawing.Size(456, 21);
            this.Data10.TabIndex = 22;
            this.Data10.Visible = false;
            // 
            // Data11
            // 
            this.Data11.FormattingEnabled = true;
            this.Data11.Location = new System.Drawing.Point(73, 347);
            this.Data11.Name = "Data11";
            this.Data11.Size = new System.Drawing.Size(456, 21);
            this.Data11.TabIndex = 23;
            this.Data11.Visible = false;
            // 
            // Data23
            // 
            this.Data23.FormattingEnabled = true;
            this.Data23.Location = new System.Drawing.Point(73, 347);
            this.Data23.Name = "Data23";
            this.Data23.Size = new System.Drawing.Size(456, 21);
            this.Data23.TabIndex = 47;
            this.Data23.Visible = false;
            // 
            // Data22
            // 
            this.Data22.FormattingEnabled = true;
            this.Data22.Location = new System.Drawing.Point(73, 317);
            this.Data22.Name = "Data22";
            this.Data22.Size = new System.Drawing.Size(456, 21);
            this.Data22.TabIndex = 46;
            this.Data22.Visible = false;
            // 
            // Data21
            // 
            this.Data21.FormattingEnabled = true;
            this.Data21.Location = new System.Drawing.Point(73, 287);
            this.Data21.Name = "Data21";
            this.Data21.Size = new System.Drawing.Size(456, 21);
            this.Data21.TabIndex = 45;
            this.Data21.Visible = false;
            // 
            // Data20
            // 
            this.Data20.FormattingEnabled = true;
            this.Data20.Location = new System.Drawing.Point(73, 257);
            this.Data20.Name = "Data20";
            this.Data20.Size = new System.Drawing.Size(456, 21);
            this.Data20.TabIndex = 44;
            this.Data20.Visible = false;
            // 
            // Data19
            // 
            this.Data19.FormattingEnabled = true;
            this.Data19.Location = new System.Drawing.Point(73, 227);
            this.Data19.Name = "Data19";
            this.Data19.Size = new System.Drawing.Size(456, 21);
            this.Data19.TabIndex = 43;
            this.Data19.Visible = false;
            // 
            // Data18
            // 
            this.Data18.FormattingEnabled = true;
            this.Data18.Location = new System.Drawing.Point(73, 197);
            this.Data18.Name = "Data18";
            this.Data18.Size = new System.Drawing.Size(456, 21);
            this.Data18.TabIndex = 42;
            this.Data18.Visible = false;
            // 
            // Data17
            // 
            this.Data17.FormattingEnabled = true;
            this.Data17.Location = new System.Drawing.Point(73, 167);
            this.Data17.Name = "Data17";
            this.Data17.Size = new System.Drawing.Size(456, 21);
            this.Data17.TabIndex = 41;
            this.Data17.Visible = false;
            // 
            // Data16
            // 
            this.Data16.FormattingEnabled = true;
            this.Data16.Location = new System.Drawing.Point(73, 137);
            this.Data16.Name = "Data16";
            this.Data16.Size = new System.Drawing.Size(456, 21);
            this.Data16.TabIndex = 40;
            this.Data16.Visible = false;
            // 
            // Data15
            // 
            this.Data15.FormattingEnabled = true;
            this.Data15.Location = new System.Drawing.Point(73, 105);
            this.Data15.Name = "Data15";
            this.Data15.Size = new System.Drawing.Size(456, 21);
            this.Data15.TabIndex = 39;
            this.Data15.Visible = false;
            // 
            // Data14
            // 
            this.Data14.FormattingEnabled = true;
            this.Data14.Location = new System.Drawing.Point(73, 78);
            this.Data14.Name = "Data14";
            this.Data14.Size = new System.Drawing.Size(456, 21);
            this.Data14.TabIndex = 38;
            this.Data14.Visible = false;
            // 
            // Data13
            // 
            this.Data13.FormattingEnabled = true;
            this.Data13.Location = new System.Drawing.Point(73, 47);
            this.Data13.Name = "Data13";
            this.Data13.Size = new System.Drawing.Size(456, 21);
            this.Data13.TabIndex = 37;
            this.Data13.Visible = false;
            // 
            // Data12
            // 
            this.Data12.FormattingEnabled = true;
            this.Data12.Location = new System.Drawing.Point(73, 17);
            this.Data12.Name = "Data12";
            this.Data12.Size = new System.Drawing.Size(456, 21);
            this.Data12.TabIndex = 36;
            this.Data12.Visible = false;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(1, 347);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(42, 13);
            this.label19.TabIndex = 35;
            this.label19.Text = "Data23";
            this.label19.Visible = false;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(1, 317);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(42, 13);
            this.label20.TabIndex = 34;
            this.label20.Text = "Data22";
            this.label20.Visible = false;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(1, 287);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(42, 13);
            this.label21.TabIndex = 33;
            this.label21.Text = "Data21";
            this.label21.Visible = false;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(1, 257);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(42, 13);
            this.label22.TabIndex = 32;
            this.label22.Text = "Data20";
            this.label22.Visible = false;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(1, 227);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(42, 13);
            this.label24.TabIndex = 31;
            this.label24.Text = "Data19";
            this.label24.Visible = false;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(1, 197);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(42, 13);
            this.label25.TabIndex = 30;
            this.label25.Text = "Data18";
            this.label25.Visible = false;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(1, 167);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(42, 13);
            this.label26.TabIndex = 29;
            this.label26.Text = "Data17";
            this.label26.Visible = false;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(1, 137);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(42, 13);
            this.label27.TabIndex = 28;
            this.label27.Text = "Data16";
            this.label27.Visible = false;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(1, 107);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(42, 13);
            this.label28.TabIndex = 27;
            this.label28.Text = "Data15";
            this.label28.Visible = false;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(1, 77);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(42, 13);
            this.label29.TabIndex = 26;
            this.label29.Text = "Data14";
            this.label29.Visible = false;
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(1, 47);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(42, 13);
            this.label30.TabIndex = 25;
            this.label30.Text = "Data13";
            this.label30.Visible = false;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(1, 17);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(42, 13);
            this.label31.TabIndex = 24;
            this.label31.Text = "Data12";
            this.label31.Visible = false;
            // 
            // IconeName
            // 
            this.IconeName.FormattingEnabled = true;
            this.IconeName.Items.AddRange(new object[] {
            "Taxi",
            "Talk",
            "Attack",
            "Directions"});
            this.IconeName.Location = new System.Drawing.Point(87, 7);
            this.IconeName.Name = "IconeName";
            this.IconeName.Size = new System.Drawing.Size(444, 21);
            this.IconeName.TabIndex = 111;
            // 
            // ScriptName
            // 
            this.ScriptName.Location = new System.Drawing.Point(87, 97);
            this.ScriptName.Name = "ScriptName";
            this.ScriptName.Size = new System.Drawing.Size(444, 20);
            this.ScriptName.TabIndex = 114;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(1, 97);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(62, 13);
            this.label32.TabIndex = 113;
            this.label32.Text = "ScriptName";
            this.toolTip1.SetToolTip(this.label32, "The name of the script that this object uses, if any. This ties a script from a s" +
        "cripting engine to this object.");
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(1, 67);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(44, 13);
            this.label33.TabIndex = 112;
            this.label33.Text = "AiName";
            this.toolTip1.SetToolTip(this.label33, "This field is overridden by ScriptName field if both are set. Only \'SmartGameObje" +
        "ctAI\' can be used.");
            // 
            // AiName
            // 
            this.AiName.Location = new System.Drawing.Point(87, 67);
            this.AiName.Name = "AiName";
            this.AiName.Size = new System.Drawing.Size(444, 20);
            this.AiName.TabIndex = 115;
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
            this.tbData.ResumeLayout(false);
            this.tbData.PerformLayout();
            this.tbModelInfo.ResumeLayout(false);
            this.tbModelInfo.PerformLayout();
            this.tbGameObjectInfo.ResumeLayout(false);
            this.tbGameObjectInfo.PerformLayout();
            this.tbcEditor.ResumeLayout(false);
            this.tbOptional.ResumeLayout(false);
            this.tbOptional.PerformLayout();
            this.tbData2.ResumeLayout(false);
            this.tbData2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbMenu;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabPage tbData;
        private System.Windows.Forms.TabPage tbModelInfo;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.TextBox ModelId;
        private System.Windows.Forms.Button btnShowModelId1;
        private Components.ModelRenderControl modelRenderControl1;
        private System.Windows.Forms.TabPage tbGameObjectInfo;
        private System.Windows.Forms.TextBox GameObjectName;
        private System.Windows.Forms.TextBox Entry;
        private System.Windows.Forms.TextBox GameObjectSize;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.ComboBox Type;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TabControl tbcEditor;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox Flags;
        private System.Windows.Forms.TextBox Faction;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tbOptional;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox CastBarCaption;
        private System.Windows.Forms.TabPage tbData2;
        private System.Windows.Forms.ComboBox Data11;
        private System.Windows.Forms.ComboBox Data10;
        private System.Windows.Forms.ComboBox Data9;
        private System.Windows.Forms.ComboBox Data8;
        private System.Windows.Forms.ComboBox Data7;
        private System.Windows.Forms.ComboBox Data6;
        private System.Windows.Forms.ComboBox Data5;
        private System.Windows.Forms.ComboBox Data4;
        private System.Windows.Forms.ComboBox Data3;
        private System.Windows.Forms.ComboBox Data2;
        private System.Windows.Forms.ComboBox Data1;
        private System.Windows.Forms.ComboBox Data0;
        private System.Windows.Forms.ComboBox Data23;
        private System.Windows.Forms.ComboBox Data22;
        private System.Windows.Forms.ComboBox Data21;
        private System.Windows.Forms.ComboBox Data20;
        private System.Windows.Forms.ComboBox Data19;
        private System.Windows.Forms.ComboBox Data18;
        private System.Windows.Forms.ComboBox Data17;
        private System.Windows.Forms.ComboBox Data16;
        private System.Windows.Forms.ComboBox Data15;
        private System.Windows.Forms.ComboBox Data14;
        private System.Windows.Forms.ComboBox Data13;
        private System.Windows.Forms.ComboBox Data12;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox ScriptName;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.ComboBox IconeName;
        private System.Windows.Forms.TextBox AiName;
    }
}
