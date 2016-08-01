using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace WoWEditor6.UI.Dialogs
{
    [Designer(typeof(CreatureEditorControlDesigner))]
    public partial class CreatureEditorControl : UserControl
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControl TabControl
        {
            get { return this.tbcEditor;  }
        }

        public CreatureEditorControl()
        {
            InitializeComponent();
            tbcEditor.Appearance = TabAppearance.FlatButtons;
            tbcEditor.ItemSize = new Size(0, 1);
            tbcEditor.SizeMode = TabSizeMode.Fixed;
        }

        private void btnShowModelId1_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(ModelId1.Text);
        }

        private void btnShowModelId2_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(ModelId2.Text);
        }

        private void btnShowModelId3_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(ModelId3.Text);
        }

        private void btnShowModelId4_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(ModelId4.Text);
        }

        private void showModelInRenderControl(string pModelId)
        {
            if(!string.IsNullOrEmpty(pModelId))
            {
                int displayId;
                if(Int32.TryParse(pModelId, out displayId))
                {
                    MessageBox.Show("" + displayId);
                    modelRenderControl1.SetCreatureDisplayEntry(displayId);  
                }
                    
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Storage.Database.WotLk.TrinityCore.Creature creature = new Storage.Database.WotLk.TrinityCore.Creature();
            creature.AiName = AiName.Text;
            creature.ArmorModifier = float.Parse(ArmorMod.Text);
            creature.BaseAttackTime = int.Parse(BaseAtkSpeed.Text);
            creature.BaseVariance = float.Parse(BaseAtkSpeedVariance.Text);
            creature.DamageModifier = float.Parse(DamageMod.Text);
            creature.DamageSchool = (Storage.Database.WotLk.TrinityCore.DamageSchool)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.DamageSchool), DamageSchool.Text);
            creature.DifficultyEntry1 = int.Parse(DiffEntry1.Text);
            creature.DifficultyEntry2 = int.Parse(DiffEntry2.Text);
            creature.DifficultyEntry3 = int.Parse(DiffEntry3.Text);
            creature.DynamicFlags = (Storage.Database.WotLk.TrinityCore.DynamicFlags)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.DynamicFlags), DynamicFlags.Text);
            creature.EntryId = int.Parse(Entry.Text);
            creature.Experience = int.Parse(Exp.Text);
            creature.ExperienceModifier = float.Parse(ExperienceMod.Text);
            creature.Faction = int.Parse(Faction.Text);
            creature.Family = (Storage.Database.WotLk.TrinityCore.Family)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.Family), Family.Text);
            creature.FlagsExtra = (Storage.Database.WotLk.TrinityCore.FlagsExtra)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.FlagsExtra), FlagsExtra.Text);
            creature.GossipMenuId = int.Parse(GossipMenuId.Text);
            creature.HealthModifier = float.Parse(HealthMod.Text);
            creature.HoverHeight = float.Parse(HoverHeight.Text);
            creature.IconName = IconName.Text;
            creature.InhabitType = (Storage.Database.WotLk.TrinityCore.InhabitType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.InhabitType), InhabitType.Text);
            creature.KillCredit1 = int.Parse(KillCredit1.Text);
            creature.KillCredit2 = int.Parse(KillCredit2.Text);
            creature.LootId = int.Parse(LootId.Text);
            creature.ManaModifier = float.Parse(ManaMod.Text);
            creature.MaxGold = int.Parse(MaxGold.Text);
            creature.MaxLevel = int.Parse(MaxLevel.Text);
            creature.MechanicImmuneMask = (Storage.Database.WotLk.TrinityCore.MechanicImmuneMask)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.MechanicImmuneMask), MechanicImmuneMask.Text);
            creature.MinGold = int.Parse(MinGold.Text);
            creature.MinLevel = int.Parse(MinLevel.Text);
            creature.ModelId1 = int.Parse(ModelId1.Text);
            creature.ModelId2 = int.Parse(ModelId2.Text);
            creature.ModelId3 = int.Parse(ModelId3.Text);
            creature.ModelId4 = int.Parse(ModelId4.Text);
            creature.MovementId = int.Parse(MovementId.Text);
            creature.MovementType = (Storage.Database.WotLk.TrinityCore.MovementType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.MovementType), MovementType.Text);
            creature.Name = NameCreature.Text;
            creature.NpcFlag = (Storage.Database.WotLk.TrinityCore.NpcFlag)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.NpcFlag), NpcFlag.Text);
            creature.PetSpellDataId = int.Parse(PetSpellDataId.Text);
            creature.PickPocketLoot = int.Parse(PickpocketLootId.Text);
            creature.RacialLeader = int.Parse(RacialLeader.Text);
            creature.RangeAttackTime = int.Parse(RangeAtkSpeed.Text);
            creature.RangeVariance = float.Parse(RangeAtkSpeedVariance.Text);
            creature.Rank = (Storage.Database.WotLk.TrinityCore.Rank)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.Rank), Rank.Text);
            creature.RegenHealth = int.Parse(RegenHealth.Text);
            creature.Resistance1 = int.Parse(ResHoly.Text);
            creature.Resistance2 = int.Parse(ResFire.Text);
            creature.Resistance3 = int.Parse(ResNature.Text);
            creature.Resistance4 = int.Parse(ResFrost.Text);
            creature.Resistance5 = int.Parse(ResShadow.Text);
            creature.Resistance6 = int.Parse(ResArcane.Text);
            creature.Scale = float.Parse(ScaleCreature.Text);
            creature.ScriptName = ScriptName.Text;
            creature.SkinLoot = int.Parse(SkinningLootId.Text);
            creature.SpeedRun = float.Parse(RunSpeed.Text);
            creature.SpeedWalk = float.Parse(WalkSpeed.Text);
            creature.Spell1 = int.Parse(McSpellId1.Text);
            creature.Spell2 = int.Parse(McSpellId2.Text);
            creature.Spell3 = int.Parse(McSpellId3.Text);
            creature.Spell4 = int.Parse(McSpellId4.Text);
            creature.Spell5 = int.Parse(McSpellId5.Text);
            creature.Spell6 = int.Parse(McSpellId6.Text);
            creature.Spell7 = int.Parse(McSpellId7.Text);
            creature.Spell8 = int.Parse(McSpellId8.Text);
            creature.SubName = SubName.Text;
            creature.TrainerClass = int.Parse(TrainerClass.Text);
            creature.TrainerRace = int.Parse(TrainerRace.Text);
            creature.TrainerSpell = int.Parse(TrainerSpell.Text);
            creature.TrainerType = (Storage.Database.WotLk.TrinityCore.TrainerType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.TrainerType), TrainerType.Text);
            creature.Type = (Storage.Database.WotLk.TrinityCore.CreatureType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.CreatureType), TypeCreature.Text);
            creature.TypeFlags = (Storage.Database.WotLk.TrinityCore.TypeFlags)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.TypeFlags), TypeFlags.Text);
            creature.UnitClass = (Storage.Database.WotLk.TrinityCore.UnitClass)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.UnitClass), UnitClass.Text);
            creature.UnitFlags = (Storage.Database.WotLk.TrinityCore.UnitFlags)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.UnitFlags), UnitFlags.Text);
            creature.UnitFlags2 = (Storage.Database.WotLk.TrinityCore.UnitFlags2)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.UnitFlags2), UnitFlags2.Text);
            creature.VehicleId = int.Parse(VehicleId.Text);
            creature.VerifiedBuild = int.Parse(VerifiedBuild.Text);

            if(Storage.Database.WotLk.TrinityCore.CreatureManager.Instance.GetCreatureByEntry(creature.EntryId) == null)
                Storage.Database.MySqlConnector.Instance.Query(creature.GetInsertSqlQuery());
            else
                Storage.Database.MySqlConnector.Instance.Query(creature.GetUpdateSqlQuery());
        }

        public void loadCreature(Storage.Database.WotLk.TrinityCore.Creature creature)
        {
            AiName.Text = creature.AiName;
            ArmorMod.Text = creature.ArmorModifier.ToString();
            BaseAtkSpeed.Text = creature.BaseAttackTime.ToString();
            BaseAtkSpeedVariance.Text = creature.BaseVariance.ToString();
            DamageMod.Text = creature.DamageModifier.ToString();
            DamageSchool.Text = creature.DamageSchool.ToString();
            DiffEntry1.Text = creature.DifficultyEntry1.ToString();
            DiffEntry2.Text = creature.DifficultyEntry2.ToString();
            DiffEntry3.Text = creature.DifficultyEntry3.ToString();
            DynamicFlags.Text = creature.DynamicFlags.ToString();
            Entry.Text = creature.EntryId.ToString();
            Exp.Text = creature.Experience.ToString();
            ExperienceMod.Text = creature.ExperienceModifier.ToString();
            Faction.Text = creature.Faction.ToString();
            Family.Text = creature.Family.ToString();
            FlagsExtra.Text = creature.FlagsExtra.ToString();
            GossipMenuId.Text = creature.GossipMenuId.ToString();
            HealthMod.Text = creature.HealthModifier.ToString();
            HoverHeight.Text = creature.HoverHeight.ToString();
            IconName.Text = creature.IconName.ToString();
            InhabitType.Text = creature.InhabitType.ToString();
            KillCredit1.Text = creature.KillCredit1.ToString();
            KillCredit2.Text = creature.KillCredit2.ToString();
            LootId.Text = creature.LootId.ToString();
            ManaMod.Text = creature.ManaModifier.ToString();
            MaxGold.Text = creature.MaxGold.ToString();
            MaxLevel.Text = creature.MaxLevel.ToString();
            MechanicImmuneMask.Text = creature.MechanicImmuneMask.ToString();
            MinGold.Text = creature.MinGold.ToString();
            MinLevel.Text = creature.MinLevel.ToString();
            ModelId1.Text = creature.ModelId1.ToString();
            ModelId2.Text = creature.ModelId2.ToString();
            ModelId3.Text = creature.ModelId3.ToString();
            ModelId4.Text = creature.ModelId4.ToString();
            MovementId.Text = creature.MovementId.ToString();
            MovementType.Text = creature.MovementType.ToString();
            NameCreature.Text = creature.Name.ToString();
            NpcFlag.Text = creature.NpcFlag.ToString();
            PetSpellDataId.Text = creature.PetSpellDataId.ToString();
            PickpocketLootId.Text = creature.PickPocketLoot.ToString();
            RacialLeader.Text = creature.RacialLeader.ToString();
            RangeAtkSpeed.Text = creature.RangeAttackTime.ToString();
            RangeAtkSpeedVariance.Text = creature.RangeVariance.ToString();
            Rank.Text = creature.Rank.ToString();
            RegenHealth.Text = creature.RegenHealth.ToString();
            ResHoly.Text = creature.Resistance1.ToString();
            ResFire.Text = creature.Resistance2.ToString();
            ResNature.Text = creature.Resistance3.ToString();
            ResFire.Text = creature.Resistance4.ToString();
            ResShadow.Text = creature.Resistance5.ToString();
            ResArcane.Text = creature.Resistance6.ToString();
            ScriptName.Text = creature.ScriptName.ToString();
            SkinningLootId.Text = creature.SkinLoot.ToString();
            RunSpeed.Text = creature.SpeedRun.ToString();
            WalkSpeed.Text = creature.SpeedWalk.ToString();
            McSpellId1.Text = creature.Spell1.ToString();
            McSpellId2.Text = creature.Spell2.ToString();
            McSpellId3.Text = creature.Spell3.ToString();
            McSpellId4.Text = creature.Spell4.ToString();
            McSpellId5.Text = creature.Spell5.ToString();
            McSpellId6.Text = creature.Spell6.ToString();
            McSpellId7.Text = creature.Spell7.ToString();
            McSpellId8.Text = creature.Spell8.ToString();
            SubName.Text = creature.SubName;
            TrainerClass.Text = creature.TrainerClass.ToString();
            TrainerRace.Text = creature.TrainerRace.ToString();
            TrainerSpell.Text = creature.TrainerSpell.ToString();
            TrainerType.Text = creature.TrainerType.ToString();
            TypeCreature.Text = creature.Type.ToString();
            TypeFlags.Text = creature.TypeFlags.ToString();
            UnitClass.Text = creature.UnitClass.ToString();
            UnitFlags.Text = creature.UnitFlags.ToString();
            UnitFlags2.Text = creature.UnitFlags2.ToString();
            VehicleId.Text = creature.VehicleId.ToString();
            VerifiedBuild.Text = creature.VerifiedBuild.ToString();
        }

        private void lbMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbcEditor.SelectedIndex = lbMenu.SelectedIndex;
        }

        private void btnShowModelId1_Click_1(object sender, EventArgs e)
        {

        }

        private void TypeCreature_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            int entry;

            if(LoadEntry.Text != "")
            {
                Storage.Database.WotLk.TrinityCore.Creature creature = new Storage.Database.WotLk.TrinityCore.Creature();
                entry = int.Parse(LoadEntry.Text);
              
                creature = Storage.Database.WotLk.TrinityCore.CreatureManager.Instance.GetCreatureByEntry(entry);

                if(creature != null)
                {
                    loadCreature(creature);
                }
            }
        }
    }

    internal class CreatureEditorControlDesigner : ControlDesigner
    {
        public override void Initialize(System.ComponentModel.IComponent component)
        {
            base.Initialize(component);

            var ctl = (this.Control as CreatureEditorControl).TabControl as TabControl;
            EnableDesignMode(ctl, "TabControl");
            foreach (TabPage page in ctl.TabPages) EnableDesignMode(page, page.Name);
        }
    }
}
