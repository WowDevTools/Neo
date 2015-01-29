using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWEditor6.UI.Settings
{
    public enum ToolbarFunction
    {
        Terrain,
        KeyBinding
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
