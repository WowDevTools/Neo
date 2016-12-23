using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Neo.UI.Dialogs
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
	        this.tbcEditor.Appearance = TabAppearance.FlatButtons;
	        this.tbcEditor.ItemSize = new Size(0, 1);
	        this.tbcEditor.SizeMode = TabSizeMode.Fixed;
        }

        private void btnShowModelId1_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(this.ModelId1.Text);
        }

        private void btnShowModelId2_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(this.ModelId2.Text);
        }

        private void btnShowModelId3_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(this.ModelId3.Text);
        }

        private void btnShowModelId4_Click(object sender, EventArgs e)
        {
            showModelInRenderControl(this.ModelId4.Text);
        }

        private void showModelInRenderControl(string pModelId)
        {
            if(!string.IsNullOrEmpty(pModelId))
            {
                int displayId;
                if(int.TryParse(pModelId, out displayId))
                {
                    MessageBox.Show("" + displayId);
	                this.modelRenderControl1.SetCreatureDisplayEntry(displayId);
                }

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Storage.Database.WotLk.TrinityCore.Creature creature = new Storage.Database.WotLk.TrinityCore.Creature();
            creature.AiName = this.AiName.Text;
            creature.ArmorModifier = float.Parse(this.ArmorMod.Text);
            creature.BaseAttackTime = int.Parse(this.BaseAtkSpeed.Text);
            creature.BaseVariance = float.Parse(this.BaseAtkSpeedVariance.Text);
            creature.DamageModifier = float.Parse(this.DamageMod.Text);
            creature.DamageSchool = (Storage.Database.WotLk.TrinityCore.DamageSchool)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.DamageSchool), this.DamageSchool.Text);
            creature.DifficultyEntry1 = int.Parse(this.DiffEntry1.Text);
            creature.DifficultyEntry2 = int.Parse(this.DiffEntry2.Text);
            creature.DifficultyEntry3 = int.Parse(this.DiffEntry3.Text);
            creature.DynamicFlags = makeFlagOrBitmask(this.DynamicFlags, typeof(Storage.Database.WotLk.TrinityCore.DynamicFlags));
            creature.EntryId = int.Parse(this.Entry.Text);
            creature.Experience = int.Parse(this.Exp.Text);
            creature.ExperienceModifier = float.Parse(this.ExperienceMod.Text);
            creature.Faction = int.Parse(this.Faction.Text);
            creature.Family = (Storage.Database.WotLk.TrinityCore.Family)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.Family), this.Family.Text);
            creature.FlagsExtra = makeFlagOrBitmask(this.FlagsExtra, typeof(Storage.Database.WotLk.TrinityCore.FlagsExtra));
            creature.GossipMenuId = int.Parse(this.GossipMenuId.Text);
            creature.HealthModifier = float.Parse(this.HealthMod.Text);
            creature.HoverHeight = float.Parse(this.HoverHeight.Text);
            creature.IconName = this.IconName.Text;
            creature.InhabitType = (int)makeFlagOrBitmask(this.InhabitType, typeof(Storage.Database.WotLk.TrinityCore.InhabitType));
            creature.KillCredit1 = int.Parse(this.KillCredit1.Text);
            creature.KillCredit2 = int.Parse(this.KillCredit2.Text);
            creature.LootId = int.Parse(this.LootId.Text);
            creature.ManaModifier = float.Parse(this.ManaMod.Text);
            creature.MaxGold = int.Parse(this.MaxGold.Text);
            creature.MaxLevel = int.Parse(this.MaxLevel.Text);
            creature.MechanicImmuneMask = makeFlagOrBitmask(this.MechanicImmuneMask, typeof(Storage.Database.WotLk.TrinityCore.MechanicImmuneMask));
            creature.MinGold = int.Parse(this.MinGold.Text);
            creature.MinLevel = int.Parse(this.MinLevel.Text);
            creature.ModelId1 = int.Parse(this.ModelId1.Text);
            creature.ModelId2 = int.Parse(this.ModelId2.Text);
            creature.ModelId3 = int.Parse(this.ModelId3.Text);
            creature.ModelId4 = int.Parse(this.ModelId4.Text);
            creature.MovementId = int.Parse(this.MovementId.Text);
            creature.MovementType = (Storage.Database.WotLk.TrinityCore.MovementType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.MovementType), this.MovementType.Text);
            creature.Name = this.NameCreature.Text;
            creature.NpcFlag = makeFlagOrBitmask(this.NpcFlag, typeof(Storage.Database.WotLk.TrinityCore.NpcFlag));
            creature.PetSpellDataId = int.Parse(this.PetSpellDataId.Text);
            creature.PickPocketLoot = int.Parse(this.PickpocketLootId.Text);
            creature.RacialLeader = int.Parse(this.RacialLeader.Text);
            creature.RangeAttackTime = int.Parse(this.RangeAtkSpeed.Text);
            creature.RangeVariance = float.Parse(this.RangeAtkSpeedVariance.Text);
            creature.Rank = (Storage.Database.WotLk.TrinityCore.Rank)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.Rank), this.Rank.Text);
            creature.RegenHealth = int.Parse(this.RegenHealth.Text);
            creature.Resistance1 = int.Parse(this.ResHoly.Text);
            creature.Resistance2 = int.Parse(this.ResFire.Text);
            creature.Resistance3 = int.Parse(this.ResNature.Text);
            creature.Resistance4 = int.Parse(this.ResFrost.Text);
            creature.Resistance5 = int.Parse(this.ResShadow.Text);
            creature.Resistance6 = int.Parse(this.ResArcane.Text);
            creature.Scale = float.Parse(this.ScaleCreature.Text);
            creature.ScriptName = this.ScriptName.Text;
            creature.SkinLoot = int.Parse(this.SkinningLootId.Text);
            creature.SpeedRun = float.Parse(this.RunSpeed.Text);
            creature.SpeedWalk = float.Parse(this.WalkSpeed.Text);
            creature.Spell1 = int.Parse(this.McSpellId1.Text);
            creature.Spell2 = int.Parse(this.McSpellId2.Text);
            creature.Spell3 = int.Parse(this.McSpellId3.Text);
            creature.Spell4 = int.Parse(this.McSpellId4.Text);
            creature.Spell5 = int.Parse(this.McSpellId5.Text);
            creature.Spell6 = int.Parse(this.McSpellId6.Text);
            creature.Spell7 = int.Parse(this.McSpellId7.Text);
            creature.Spell8 = int.Parse(this.McSpellId8.Text);
            creature.SubName = this.SubName.Text;
            creature.TrainerClass = int.Parse(this.TrainerClass.Text);
            creature.TrainerRace = int.Parse(this.TrainerRace.Text);
            creature.TrainerSpell = int.Parse(this.TrainerSpell.Text);
            creature.TrainerType = (Storage.Database.WotLk.TrinityCore.TrainerType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.TrainerType), this.TrainerType.Text);
            creature.Type = (Storage.Database.WotLk.TrinityCore.CreatureType)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.CreatureType), this.TypeCreature.Text);
            creature.TypeFlags = makeFlagOrBitmask(this.TypeFlags, typeof(Storage.Database.WotLk.TrinityCore.TypeFlags));
            creature.UnitClass = (Storage.Database.WotLk.TrinityCore.UnitClass)Enum.Parse(typeof(Storage.Database.WotLk.TrinityCore.UnitClass), this.UnitClass.Text);
            creature.UnitFlags = makeFlagOrBitmask(this.UnitFlags, typeof(Storage.Database.WotLk.TrinityCore.UnitFlags));
            creature.UnitFlags2 = makeFlagOrBitmask(this.UnitFlags2, typeof(Storage.Database.WotLk.TrinityCore.UnitFlags2));
            creature.VehicleId = int.Parse(this.VehicleId.Text);
            creature.VerifiedBuild = int.Parse(this.VerifiedBuild.Text);

            if(Storage.Database.WotLk.TrinityCore.CreatureManager.Instance.GetCreatureByEntry(creature.EntryId) == null)
            {
                Storage.Database.MySqlConnector.Instance.Query(creature.GetInsertSqlQuery());
                Storage.Database.WotLk.TrinityCore.CreatureManager.Instance.addCreatedCreature(creature);
                MessageBox.Show("Inserted");
            }
            else
            {
                Storage.Database.MySqlConnector.Instance.Query(creature.GetUpdateSqlQuery());
                MessageBox.Show("Updated");
            }
        }

        public void loadCreature(Storage.Database.WotLk.TrinityCore.Creature creature)
        {
	        this.AiName.Text = creature.AiName;
	        this.ArmorMod.Text = creature.ArmorModifier.ToString();
	        this.BaseAtkSpeed.Text = creature.BaseAttackTime.ToString();
	        this.BaseAtkSpeedVariance.Text = creature.BaseVariance.ToString();
	        this.DamageMod.Text = creature.DamageModifier.ToString();
	        this.DamageSchool.Text = creature.DamageSchool.ToString();
	        this.DiffEntry1.Text = creature.DifficultyEntry1.ToString();
	        this.DiffEntry2.Text = creature.DifficultyEntry2.ToString();
	        this.DiffEntry3.Text = creature.DifficultyEntry3.ToString();
            checkFlagOrBitmask(this.DynamicFlags, typeof(Storage.Database.WotLk.TrinityCore.DynamicFlags), creature.DynamicFlags);
	        this.Entry.Text = creature.EntryId.ToString();
	        this.Exp.Text = creature.Experience.ToString();
	        this.ExperienceMod.Text = creature.ExperienceModifier.ToString();
	        this.Faction.Text = creature.Faction.ToString();
	        this.Family.Text = creature.Family.ToString();
            checkFlagOrBitmask(this.FlagsExtra, typeof(Storage.Database.WotLk.TrinityCore.FlagsExtra), creature.FlagsExtra);
	        this.GossipMenuId.Text = creature.GossipMenuId.ToString();
	        this.HealthMod.Text = creature.HealthModifier.ToString();
	        this.HoverHeight.Text = creature.HoverHeight.ToString();
	        this.IconName.Text = creature.IconName.ToString();
            checkFlagOrBitmask(this.InhabitType, typeof(Storage.Database.WotLk.TrinityCore.InhabitType), (uint)creature.InhabitType);
	        this.KillCredit1.Text = creature.KillCredit1.ToString();
	        this.KillCredit2.Text = creature.KillCredit2.ToString();
	        this.LootId.Text = creature.LootId.ToString();
	        this.ManaMod.Text = creature.ManaModifier.ToString();
	        this.MaxGold.Text = creature.MaxGold.ToString();
	        this.MaxLevel.Text = creature.MaxLevel.ToString();
            checkFlagOrBitmask(this.MechanicImmuneMask, typeof(Storage.Database.WotLk.TrinityCore.MechanicImmuneMask), creature.MechanicImmuneMask);
	        this.MinGold.Text = creature.MinGold.ToString();
	        this.MinLevel.Text = creature.MinLevel.ToString();
	        this.ModelId1.Text = creature.ModelId1.ToString();
	        this.ModelId2.Text = creature.ModelId2.ToString();
	        this.ModelId3.Text = creature.ModelId3.ToString();
	        this.ModelId4.Text = creature.ModelId4.ToString();
	        this.MovementId.Text = creature.MovementId.ToString();
	        this.MovementType.Text = creature.MovementType.ToString();
	        this.NameCreature.Text = creature.Name.ToString();
            checkFlagOrBitmask(this.UnitFlags2, typeof(Storage.Database.WotLk.TrinityCore.UnitFlags2), creature.UnitFlags2);
	        this.NpcFlag.Text = creature.NpcFlag.ToString();
	        this.PetSpellDataId.Text = creature.PetSpellDataId.ToString();
	        this.PickpocketLootId.Text = creature.PickPocketLoot.ToString();
	        this.RacialLeader.Text = creature.RacialLeader.ToString();
	        this.RangeAtkSpeed.Text = creature.RangeAttackTime.ToString();
	        this.RangeAtkSpeedVariance.Text = creature.RangeVariance.ToString();
	        this.Rank.Text = creature.Rank.ToString();
	        this.RegenHealth.Text = creature.RegenHealth.ToString();
	        this.ResHoly.Text = creature.Resistance1.ToString();
	        this.ResFire.Text = creature.Resistance2.ToString();
	        this.ResNature.Text = creature.Resistance3.ToString();
	        this.ResFire.Text = creature.Resistance4.ToString();
	        this.ResShadow.Text = creature.Resistance5.ToString();
	        this.ResArcane.Text = creature.Resistance6.ToString();
	        this.ScriptName.Text = creature.ScriptName.ToString();
	        this.SkinningLootId.Text = creature.SkinLoot.ToString();
	        this.RunSpeed.Text = creature.SpeedRun.ToString();
	        this.WalkSpeed.Text = creature.SpeedWalk.ToString();
	        this.McSpellId1.Text = creature.Spell1.ToString();
	        this.McSpellId2.Text = creature.Spell2.ToString();
	        this.McSpellId3.Text = creature.Spell3.ToString();
	        this.McSpellId4.Text = creature.Spell4.ToString();
	        this.McSpellId5.Text = creature.Spell5.ToString();
	        this.McSpellId6.Text = creature.Spell6.ToString();
	        this.McSpellId7.Text = creature.Spell7.ToString();
	        this.McSpellId8.Text = creature.Spell8.ToString();
	        this.SubName.Text = creature.SubName;
	        this.TrainerClass.Text = creature.TrainerClass.ToString();
	        this.TrainerRace.Text = creature.TrainerRace.ToString();
	        this.TrainerSpell.Text = creature.TrainerSpell.ToString();
	        this.TrainerType.Text = creature.TrainerType.ToString();
	        this.TypeCreature.Text = creature.Type.ToString();
            checkFlagOrBitmask(this.TypeFlags, typeof(Storage.Database.WotLk.TrinityCore.TypeFlags),creature.TypeFlags);
	        this.UnitClass.Text = creature.UnitClass.ToString();
            checkFlagOrBitmask(this.UnitFlags, typeof(Storage.Database.WotLk.TrinityCore.UnitFlags), creature.UnitFlags);
            checkFlagOrBitmask(this.UnitFlags2, typeof(Storage.Database.WotLk.TrinityCore.UnitFlags2), creature.UnitFlags2);
	        this.VehicleId.Text = creature.VehicleId.ToString();
	        this.VerifiedBuild.Text = creature.VerifiedBuild.ToString();
        }

        private void lbMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
	        this.tbcEditor.SelectedIndex = this.lbMenu.SelectedIndex;
        }

        private uint makeFlagOrBitmask(CheckedListBox list, Type e)
        {
            if (!e.IsEnum)
            {
	            return 0;
            }

	        uint myFlags = 0x0;

            foreach (object item in list.CheckedItems)
            {
                myFlags += Convert.ToUInt32(Enum.Parse(e, item.ToString()));
            }

            return myFlags;
        }

        private void checkFlagOrBitmask(CheckedListBox list, Type e, uint value)
        {
            if (!e.IsEnum)
            {
	            return;
            }

	        foreach (int i in list.CheckedIndices)
            {
                list.SetItemCheckState(i, CheckState.Unchecked);
            }

            var values = Enum.GetValues(e);

            for(int i = values.Length-1;i>0;i--)
            {
                uint val = Convert.ToUInt32(values.GetValue(i));
                if(val <= value)
                {
                    list.SetItemCheckState(i,CheckState.Checked);
                    value -= val;
                }
            }
        }



        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (this.LoadEntry.Text != "")
            {
                if (Storage.Database.WotLk.TrinityCore.CreatureManager.Instance.GetCreatureByEntry(Convert.ToInt32(this.LoadEntry.Text)) == null)
                {
                    MessageBox.Show("There is no creature with this id.");
                }

                Storage.Database.WotLk.TrinityCore.Creature creatureLoaded = new Storage.Database.WotLk.TrinityCore.Creature();

                creatureLoaded = Storage.Database.WotLk.TrinityCore.CreatureManager.Instance.GetCreatureByEntry(int.Parse(this.LoadEntry.Text));

                if(creatureLoaded != null)
                {
                    loadCreature(creatureLoaded);
                }
            }
        }
    }

    internal class CreatureEditorControlDesigner : ControlDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            var ctl = (this.Control as CreatureEditorControl).TabControl as TabControl;
            EnableDesignMode(ctl, "TabControl");
            foreach (TabPage page in ctl.TabPages)
            {
	            EnableDesignMode(page, page.Name);
            }
        }
    }
}
