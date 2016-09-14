using System.Collections.Generic;

namespace WoWEditor6.Settings
{
    public enum ToolbarFunction
    {
        Terrain,
        KeyBinding,
        Save
    }

    public class ToolbarButton
    {
        public ToolbarFunction Function { get; set; }
        public string Tooltip { get; set; }
    }

    public class ToolbarButtons
    {
        public List<ToolbarButton> Buttons { get; set; }
    }
}
